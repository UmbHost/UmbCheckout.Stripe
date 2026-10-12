using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UmbCheckout.Stripe.Interfaces;
using UmbCheckout.Stripe.Models;
using UmbCheckout.Stripe.Notifications;
using UmbCheckout.Stripe.Persistence;
using UmbHost.Licensing.Services;
using UmbCheckout.Shared.Notifications;
using UmbCheckout.Shared.Persistence;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Cms.Persistence.EFCore.Scoping;

namespace UmbCheckout.Stripe.Services
{
    /// <summary>
    /// A service which Gets or Updates the Stripe settings
    /// </summary>
    internal class StripeSettingsService : IStripeSettingsService
    {
        private readonly IEFCoreScopeProvider<UmbCheckoutStripeDbContext> _scopeProvider;
        private readonly IUmbracoMapper _mapper;
        private readonly ILogger<StripeSettingsService> _logger;
        private readonly IEventAggregator _eventAggregator;
        private readonly IEventMessagesFactory _eventMessagesFactory;
        private readonly SettingsCache<Pocos.UmbCheckoutStripeSettings> _cache;
        private readonly IScopeAccessor? _scopeAccessor;

        public StripeSettingsService(IEFCoreScopeProvider<UmbCheckoutStripeDbContext> scopeProvider, ILogger<StripeSettingsService> logger, IUmbracoMapper mapper, LicenseService licenseService,
            IEventAggregator eventAggregator, IEventMessagesFactory eventMessagesFactory, SettingsCache<Pocos.UmbCheckoutStripeSettings> cache, IScopeAccessor? scopeAccessor = null)
        {
            _cache = cache;
            _scopeAccessor = scopeAccessor;
            _scopeProvider = scopeProvider;
            _logger = logger;
            _mapper = mapper;
            _eventAggregator = eventAggregator;
            _eventMessagesFactory = eventMessagesFactory;
        }

        /// <inheritdoc />
        public async Task<UmbCheckoutStripeSettings?> GetStripeSettingsAsync(CancellationToken cancellationToken = default)
        {
            var settings = await ReadAsync(cancellationToken);
            if (settings != null && !UmbCheckoutHooks.LoadedSuppressed)
            {
                await _eventAggregator.PublishAsync(new OnStripeSettingsLoadedNotification(settings, _eventMessagesFactory.Get()), cancellationToken);
            }

            return settings;
        }

        /// <summary>The stored Stripe settings, without publishing Loaded.</summary>
        /// <summary>The stored settings (cached), without publishing Loaded: a fresh model each time, mapped from the cached row.</summary>
        private async Task<UmbCheckoutStripeSettings?> ReadAsync(CancellationToken cancellationToken)
        {
            if (!_cache.TryGet(out var entity))
            {
                var generation = _cache.Generation;
                // Inside someone else's scope (a uSync import) the row may be uncommitted and could still roll back: read it, don't cache it.
                var insideAnotherScope = _scopeAccessor?.AmbientScope is not null;
                using var scope = _scopeProvider.CreateScope();
                entity = await scope.ExecuteWithContextAsync(db => db.StripeSettings.AsNoTracking().SingleOrDefaultAsync(cancellationToken));
                scope.Complete();
                if (!insideAnotherScope)
                {
                    _cache.TrySet(entity, generation);
                }
            }

            return _mapper.Map<Pocos.UmbCheckoutStripeSettings, UmbCheckoutStripeSettings>(entity);
        }

        /// <inheritdoc />
        public async Task<bool> UpdateStripeSettingsAsync(UmbCheckoutStripeSettings stripeSettings, CancellationToken cancellationToken = default)
        {
            var saving = new OnStripeSettingsSavingNotification(stripeSettings, await ReadAsync(cancellationToken), _eventMessagesFactory.Get());
            if (await _eventAggregator.PublishCancellingAsync(saving, _eventMessagesFactory.Get()))
            {
                return false;
            }

            var incoming = _mapper.Map<UmbCheckoutStripeSettings, Pocos.UmbCheckoutStripeSettings>(stripeSettings);
            if (incoming == null)
            {
                return false;
            }

            using (var scope = _scopeProvider.CreateScope())
            {
                var saved = await scope.ExecuteWithContextAsync(async db =>
                {
                    var existing = await db.StripeSettings.SingleOrDefaultAsync(cancellationToken);
                    if (existing == null)
                    {
                        incoming.Id = 0;
                        incoming.Key = incoming.Key is null || incoming.Key == Guid.Empty ? Guid.NewGuid() : incoming.Key;
                        db.StripeSettings.Add(incoming);
                        await db.SaveChangesAsync(cancellationToken);
                        return incoming;
                    }

                    incoming.Id = existing.Id;
                    incoming.Key = existing.Key ?? Guid.NewGuid();
                    db.Entry(existing).CurrentValues.SetValues(incoming);
                    await db.SaveChangesAsync(cancellationToken);
                    return existing;
                });

                scope.Notifications.Publish(new OnStripeSettingsSavedNotification(_mapper.Map<Pocos.UmbCheckoutStripeSettings, UmbCheckoutStripeSettings>(saved)));
                scope.Complete();
            } // commits here, unless this save is inside someone else's scope

            // This server's copy, after the commit. Other servers, and a save inside an outer scope, are cleared by the Saved cache handler,
            // which Umbraco runs once the outermost scope has committed.
            _cache.Clear();
            _logger.LogDebug("UmbCheckout Stripe settings saved");
            return true;
        }
    }
}
