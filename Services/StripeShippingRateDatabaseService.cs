using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UmbCheckout.Stripe.Interfaces;
using UmbCheckout.Stripe.Notifications;
using UmbCheckout.Stripe.Persistence;
using UmbCheckout.Stripe.Pocos;
using UmbCheckout.Shared.Notifications;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Persistence.EFCore.Scoping;
using ShippingRate = UmbCheckout.Stripe.Models.ShippingRate;

namespace UmbCheckout.Stripe.Services
{
    /// <summary>
    /// A service which handles getting the Stripe Shipping Rates from the database
    /// </summary>
    internal class StripeShippingRateDatabaseService : IStripeShippingRateDatabaseService
    {
        private readonly IEFCoreScopeProvider<UmbCheckoutStripeDbContext> _scopeProvider;
        private readonly IUmbracoMapper _mapper;
        private readonly ILogger<StripeShippingRateDatabaseService> _logger;
        private readonly IEventAggregator _eventAggregator;
        private readonly IEventMessagesFactory _eventMessagesFactory;

        public StripeShippingRateDatabaseService(IEFCoreScopeProvider<UmbCheckoutStripeDbContext> scopeProvider, IUmbracoMapper mapper, ILogger<StripeShippingRateDatabaseService> logger,
            IEventAggregator eventAggregator, IEventMessagesFactory eventMessagesFactory)
        {
            _scopeProvider = scopeProvider;
            _mapper = mapper;
            _logger = logger;
            _eventAggregator = eventAggregator;
            _eventMessagesFactory = eventMessagesFactory;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ShippingRate>> GetShippingRatesAsync(CancellationToken cancellationToken = default)
        {
            using var scope = _scopeProvider.CreateScope();
            var rows = await scope.ExecuteWithContextAsync(db => db.ShippingRates.AsNoTracking().OrderBy(x => x.Id).ToListAsync(cancellationToken));
            scope.Complete();
            return await LoadedAsync(_mapper.MapEnumerable<UmbCheckoutStripeShipping, ShippingRate>(rows), cancellationToken);
        }

        /// <inheritdoc />
        public async Task<ShippingRate?> GetShippingRateAsync(Guid key, CancellationToken cancellationToken = default)
            => await LoadedAsync(await FindAsync(key, cancellationToken), cancellationToken);

        /// <inheritdoc />
        public async Task<ShippingRate?> GetShippingRateAsync(string value, CancellationToken cancellationToken = default)
            => await LoadedAsync(await FindAsync(value, cancellationToken), cancellationToken);

        /// <inheritdoc />
        public async Task<ShippingRate?> CreateShippingRateAsync(ShippingRate shippingRate, CancellationToken cancellationToken = default)
        {
            if (await _eventAggregator.PublishCancellingAsync(new OnShippingRateSavingNotification(shippingRate, null, _eventMessagesFactory.Get()), _eventMessagesFactory.Get()))
            {
                return null;
            }

            using var scope = _scopeProvider.CreateScope();
            var created = await scope.ExecuteWithContextAsync(db => InsertAsync(db, shippingRate, cancellationToken));
            if (created != null)
            {
                scope.Notifications.Publish(new OnShippingRateSavedNotification(created));
            }

            scope.Complete();
            return created;
        }

        /// <inheritdoc />
        public async Task<ShippingRate?> UpdateShippingRateAsync(ShippingRate shippingRate, CancellationToken cancellationToken = default)
        {
            // The row this update will change, matched as the update below matches it: by Key, then by Stripe ID.
            var stored = (shippingRate.Key == Guid.Empty ? null : await FindAsync(shippingRate.Key, cancellationToken))
                         ?? await FindAsync(shippingRate.Value, cancellationToken);
            if (await _eventAggregator.PublishCancellingAsync(new OnShippingRateSavingNotification(shippingRate, stored, _eventMessagesFactory.Get()), _eventMessagesFactory.Get()))
            {
                return null;
            }

            using var scope = _scopeProvider.CreateScope();
            ShippingRate? saved;
            try
            {
                saved = await scope.ExecuteWithContextAsync(async db =>
                {
                    // By Key first: the Stripe ID (Value) is what the editor may have changed.
                    var existing = shippingRate.Key == Guid.Empty
                        ? null
                        : await db.ShippingRates.SingleOrDefaultAsync(x => x.Key == shippingRate.Key, cancellationToken);

                    if (existing != null)
                    {
                        if (await db.ShippingRates.AnyAsync(x => x.Value == shippingRate.Value && x.Id != existing.Id, cancellationToken))
                        {
                            return null;
                        }

                        existing.Name = shippingRate.Name;
                        existing.Value = shippingRate.Value;
                        await db.SaveChangesAsync(cancellationToken);
                        return _mapper.Map<UmbCheckoutStripeShipping, ShippingRate>(existing);
                    }

                    // No such key (a key-less update, or uSync from another environment): the row for this Stripe rate, if any.
                    var byValue = await db.ShippingRates.SingleOrDefaultAsync(x => x.Value == shippingRate.Value, cancellationToken);
                    if (byValue == null)
                    {
                        return await InsertAsync(db, shippingRate, cancellationToken);
                    }

                    byValue.Name = shippingRate.Name;
                    byValue.Key ??= Guid.NewGuid();
                    await db.SaveChangesAsync(cancellationToken);
                    return _mapper.Map<UmbCheckoutStripeShipping, ShippingRate>(byValue);
                });
            }
            catch (DbUpdateException ex)
            {
                // A concurrent save took the same Key or Stripe ID.
                _logger.LogWarning(ex, "Shipping rate {ShippingRateKey} ({StripeShippingRateId}) could not be saved", shippingRate.Key, shippingRate.Value);
                return null;
            }

            if (saved != null)
            {
                scope.Notifications.Publish(new OnShippingRateSavedNotification(saved));
            }

            scope.Complete();
            return saved;
        }

        /// <inheritdoc />
        public async Task<bool> DeleteShippingRateAsync(Guid key, CancellationToken cancellationToken = default)
        {
            var stored = await FindAsync(key, cancellationToken);
            if (stored != null && await _eventAggregator.PublishCancellingAsync(new OnShippingRateDeletingNotification(stored, _eventMessagesFactory.Get()), _eventMessagesFactory.Get()))
            {
                return false;
            }

            using var scope = _scopeProvider.CreateScope();
            var deleted = await scope.ExecuteWithContextAsync(async db =>
            {
                var existing = await db.ShippingRates.SingleOrDefaultAsync(x => x.Key == key, cancellationToken);
                if (existing == null)
                {
                    return null;
                }

                db.ShippingRates.Remove(existing);
                await db.SaveChangesAsync(cancellationToken);
                return _mapper.Map<UmbCheckoutStripeShipping, ShippingRate>(existing);
            });

            if (deleted != null)
            {
                scope.Notifications.Publish(new OnShippingRateDeletedNotification(deleted));
            }

            scope.Complete();
            return deleted != null;
        }

        /// <summary>The stored rate with this key, without publishing Loaded.</summary>
        private async Task<ShippingRate?> FindAsync(Guid key, CancellationToken cancellationToken)
        {
            using var scope = _scopeProvider.CreateScope();
            var row = await scope.ExecuteWithContextAsync(db => db.ShippingRates.AsNoTracking().SingleOrDefaultAsync(x => x.Key == key, cancellationToken));
            scope.Complete();
            return _mapper.Map<UmbCheckoutStripeShipping, ShippingRate>(row);
        }

        /// <summary>The stored rate with this Stripe ID, without publishing Loaded.</summary>
        private async Task<ShippingRate?> FindAsync(string value, CancellationToken cancellationToken)
        {
            using var scope = _scopeProvider.CreateScope();
            var row = await scope.ExecuteWithContextAsync(db => db.ShippingRates.AsNoTracking().SingleOrDefaultAsync(x => x.Value == value, cancellationToken));
            scope.Complete();
            return _mapper.Map<UmbCheckoutStripeShipping, ShippingRate>(row);
        }

        /// <summary>Publishes Loaded for what a list read is about to return.</summary>
        private async Task<List<ShippingRate>> LoadedAsync(IEnumerable<ShippingRate> rates, CancellationToken cancellationToken)
        {
            if (UmbCheckoutHooks.LoadedSuppressed)
            {
                return rates.ToList();
            }

            var notification = new OnShippingRatesLoadedNotification(rates.ToList(), _eventMessagesFactory.Get());
            await _eventAggregator.PublishAsync(notification, cancellationToken);
            return notification.Items;
        }

        /// <summary>Publishes Loaded for a single read; null if there was nothing to return or a handler removed the rate.</summary>
        private async Task<ShippingRate?> LoadedAsync(ShippingRate? rate, CancellationToken cancellationToken)
        {
            if (rate == null)
            {
                return null;
            }

            // The same rate, possibly changed or replaced (matched by key); null when a handler removed it.
            var items = await LoadedAsync([rate], cancellationToken);
            return items.FirstOrDefault(r => ReferenceEquals(r, rate)) ?? items.FirstOrDefault(r => r.Key == rate.Key);
        }

        private async Task<ShippingRate?> InsertAsync(UmbCheckoutStripeDbContext db, ShippingRate shippingRate, CancellationToken cancellationToken)
        {
            if (await db.ShippingRates.AnyAsync(x => x.Value == shippingRate.Value, cancellationToken))
            {
                return null;
            }

            var row = new UmbCheckoutStripeShipping
            {
                Name = shippingRate.Name,
                Value = shippingRate.Value,
                Key = shippingRate.Key == Guid.Empty ? Guid.NewGuid() : shippingRate.Key,
            };
            db.ShippingRates.Add(row);
            await db.SaveChangesAsync(cancellationToken);
            return _mapper.Map<UmbCheckoutStripeShipping, ShippingRate>(row);
        }
    }
}
