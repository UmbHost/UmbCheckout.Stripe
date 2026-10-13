using Microsoft.Extensions.Logging;
using UmbCheckout.Shared.Persistence;
using UmbCheckout.Stripe.Interfaces;
using UmbCheckout.Stripe.Models;
using UmbCheckout.Stripe.Notifications;
using UmbHost.Licensing.Services;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Scoping;
using IScopeAccessor = Umbraco.Cms.Infrastructure.Scoping.IScopeAccessor;
using IScopeProvider = Umbraco.Cms.Infrastructure.Scoping.IScopeProvider;

namespace UmbCheckout.Stripe.Services
{
    /// <summary>
    /// A service which Gets or Updates the configuration
    /// </summary>
    /// <remarks>
    /// The stored row is cached for the life of the process (a fresh model is mapped from it on every read). A save clears it here and,
    /// once committed, on every load-balanced server (<see cref="Cache.UmbCheckoutStripeSettingsSavedCacheHandler"/>).
    /// </remarks>
    internal class StripeSettingsService : IStripeSettingsService
    {
        private readonly IScopeProvider _scopeProvider;
        private readonly ICoreScopeProvider _coreScopeProvider;
        private readonly IUmbracoMapper _mapper;
        private readonly ILogger<StripeSettingsService> _logger;
        private readonly SettingsCache<Pocos.UmbCheckoutStripeSettings> _cache;
        private readonly IScopeAccessor? _scopeAccessor;

        public StripeSettingsService(IScopeProvider scopeProvider, ILogger<StripeSettingsService> logger, IUmbracoMapper mapper, ICoreScopeProvider coreScopeProvider, LicenseService licenseService,
            SettingsCache<Pocos.UmbCheckoutStripeSettings> cache, IScopeAccessor? scopeAccessor = null)
        {
            _scopeProvider = scopeProvider;
            _logger = logger;
            _mapper = mapper;
            _coreScopeProvider = coreScopeProvider;
            _cache = cache;
            _scopeAccessor = scopeAccessor;
        }

        /// <inheritdoc />
        public async Task<UmbCheckoutStripeSettings?> GetStripeSettings()
        {
            return _mapper.Map<Pocos.UmbCheckoutStripeSettings, UmbCheckoutStripeSettings>(await ReadAsync());
        }

        /// <summary>The stored row: cached, or read (and cached unless the read is inside someone else's scope).</summary>
        private async Task<Pocos.UmbCheckoutStripeSettings?> ReadAsync()
        {
            if (_cache.TryGet(out var cached))
            {
                return cached;
            }

            // Taken before the query: a save that clears the cache meanwhile stops this (possibly older) row being cached.
            var generation = _cache.Generation;
            // Inside someone else's scope (a uSync import, a save) the row may be uncommitted and could still roll back: read it, don't cache it.
            var insideAnotherScope = _scopeAccessor?.AmbientScope is not null;
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            var result = await scope.Database.QueryAsync<Pocos.UmbCheckoutStripeSettings>().SingleOrDefault();
            if (!insideAnotherScope)
            {
                _cache.TrySet(result, generation);
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<bool> UpdateStripeSettings(UmbCheckoutStripeSettings stripeSettings)
        {
            try
            {
                using (_coreScopeProvider.CreateCoreScope(autoComplete: true))
                {
                    return await SaveAsync(stripeSettings);
                }
            }
            finally
            {
                // This server's copy, after the commit. Other servers, and a save inside an outer scope, are cleared by the Saved cache
                // handler, which Umbraco runs once the outermost scope has committed.
                _cache.Clear();
            }
        }

        private async Task<bool> SaveAsync(UmbCheckoutStripeSettings stripeSettings)
        {
            var stripeSettingsPoco = _mapper.Map<UmbCheckoutStripeSettings, Pocos.UmbCheckoutStripeSettings>(stripeSettings);
            if (stripeSettingsPoco == null)
            {
                return false;
            }

            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            var db = scope.Database;
            // From the database, not the cache: another server's save may not have reached this one's cache yet.
            var existing = await db.QueryAsync<Pocos.UmbCheckoutStripeSettings>().SingleOrDefault();
            bool saved;
            if (existing != null)
            {
                stripeSettingsPoco.Id = existing.Id;
                stripeSettingsPoco.Key = existing.Key;
                saved = await db.UpdateAsync(stripeSettingsPoco) != 0;
            }
            else
            {
                // The first save publishes Saved too, so uSync writes its file and every server's cache is cleared.
                saved = await db.InsertAsync(stripeSettingsPoco) is not null;
            }

            if (saved)
            {
                // The row as now stored (the cache still holds the previous one until this save commits).
                var stored = await db.QueryAsync<Pocos.UmbCheckoutStripeSettings>().SingleOrDefault();
                scope.Notifications.Publish(new OnStripeSettingsSavedNotification(_mapper.Map<Pocos.UmbCheckoutStripeSettings, UmbCheckoutStripeSettings>(stored)));
            }

            return saved;
        }
    }
}
