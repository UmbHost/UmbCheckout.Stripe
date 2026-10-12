using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UmbCheckout.Stripe.Interfaces;
using UmbCheckout.Stripe.Notifications;
using UmbCheckout.Stripe.Persistence;
using UmbCheckout.Stripe.Pocos;
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

        public StripeShippingRateDatabaseService(IEFCoreScopeProvider<UmbCheckoutStripeDbContext> scopeProvider, IUmbracoMapper mapper, ILogger<StripeShippingRateDatabaseService> logger)
        {
            _scopeProvider = scopeProvider;
            _mapper = mapper;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ShippingRate>> GetShippingRatesAsync(CancellationToken cancellationToken = default)
        {
            using var scope = _scopeProvider.CreateScope();
            var rows = await scope.ExecuteWithContextAsync(db => db.ShippingRates.AsNoTracking().OrderBy(x => x.Id).ToListAsync(cancellationToken));
            scope.Complete();
            return _mapper.MapEnumerable<UmbCheckoutStripeShipping, ShippingRate>(rows);
        }

        /// <inheritdoc />
        public async Task<ShippingRate?> GetShippingRateAsync(Guid key, CancellationToken cancellationToken = default)
        {
            using var scope = _scopeProvider.CreateScope();
            var row = await scope.ExecuteWithContextAsync(db => db.ShippingRates.AsNoTracking().SingleOrDefaultAsync(x => x.Key == key, cancellationToken));
            scope.Complete();
            return _mapper.Map<UmbCheckoutStripeShipping, ShippingRate>(row);
        }

        /// <inheritdoc />
        public async Task<ShippingRate?> GetShippingRateAsync(string value, CancellationToken cancellationToken = default)
        {
            using var scope = _scopeProvider.CreateScope();
            var row = await scope.ExecuteWithContextAsync(db => db.ShippingRates.AsNoTracking().SingleOrDefaultAsync(x => x.Value == value, cancellationToken));
            scope.Complete();
            return _mapper.Map<UmbCheckoutStripeShipping, ShippingRate>(row);
        }

        /// <inheritdoc />
        public async Task<ShippingRate?> CreateShippingRateAsync(ShippingRate shippingRate, CancellationToken cancellationToken = default)
        {
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
