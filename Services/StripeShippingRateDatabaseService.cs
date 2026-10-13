using Microsoft.Extensions.Logging;
using UmbCheckout.Stripe.Interfaces;
using UmbCheckout.Stripe.Notifications;
using UmbCheckout.Stripe.Pocos;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Infrastructure.Scoping;
using ShippingRate = UmbCheckout.Stripe.Models.ShippingRate;

namespace UmbCheckout.Stripe.Services
{
    /// <summary>
    /// A service which handles getting the Stripe Shipping Rates from the database
    /// </summary>
    internal class StripeShippingRateDatabaseService : IStripeShippingRateDatabaseService
    {
        private readonly IScopeProvider _scopeProvider;
        private readonly IUmbracoMapper _mapper;
        private readonly ILogger<StripeShippingRateDatabaseService> _logger;

        public StripeShippingRateDatabaseService(IScopeProvider scopeProvider, IUmbracoMapper mapper, ILogger<StripeShippingRateDatabaseService> logger)
        {
            _scopeProvider = scopeProvider;
            _mapper = mapper;
            _logger = logger;
        }
        
        /// <inheritdoc />
        public async Task<IEnumerable<ShippingRate>> GetShippingRates()
        {
            try
            { 
                using var scope = _scopeProvider.CreateScope(autoComplete: true);
                var results = await scope.Database.FetchAsync<UmbCheckoutStripeShipping>();

                return _mapper.MapEnumerable<UmbCheckoutStripeShipping, ShippingRate>(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Gets a Shipping Rate by id
        /// </summary>
        /// <param name="id">Id of the Stripe Shipping Rate</param>
        /// <returns>The Stripe Shipping Rate</returns>
        private async Task<ShippingRate?> GetShippingRate(long id)
        {
            try
            {
                using var scope = _scopeProvider.CreateScope(autoComplete: true);
                var results = await scope.Database.QueryAsync<UmbCheckoutStripeShipping>().SingleOrDefault(x => x.Id == id);

                return _mapper.Map<UmbCheckoutStripeShipping, ShippingRate>(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<ShippingRate?> GetShippingRate(Guid key)
        {
            try
            {
                using var scope = _scopeProvider.CreateScope(autoComplete: true);
                var results = await scope.Database.QueryAsync<UmbCheckoutStripeShipping>().SingleOrDefault(x => x.Key == key);

                return _mapper.Map<UmbCheckoutStripeShipping, ShippingRate>(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<ShippingRate?> GetShippingRate(string value)
        {
            try
            {
                using var scope = _scopeProvider.CreateScope(autoComplete: true);
                var results = await scope.Database.QueryAsync<UmbCheckoutStripeShipping>().SingleOrDefault(x => x.Value == value);

                return _mapper.Map<UmbCheckoutStripeShipping, ShippingRate>(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<ShippingRate?> CreateShippingRate(ShippingRate shippingRate)
        {
            try
            {
                using var scope = _scopeProvider.CreateScope(autoComplete: true);

                var shippingRatePoco = _mapper.Map<ShippingRate, UmbCheckoutStripeShipping>(shippingRate);

                var existingShippingRate = await GetShippingRate(shippingRate.Value);

                if (existingShippingRate == null)
                {
                    var result = (long)await scope.Database.InsertAsync(shippingRatePoco);
                    var updatedShippingRate = await GetShippingRate(result);
                    scope.Notifications.Publish(new OnShippingRateSavedNotification(updatedShippingRate));

                    return updatedShippingRate;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <inheritdoc />
        /// <remarks>
        /// An upsert keyed on <see cref="ShippingRate.Key"/>: the row with that Key gets the new name and Stripe ID (null when the
        /// Stripe ID belongs to another rate). With no such Key (a key-less update, or uSync from another environment) the row with
        /// that Stripe ID is renamed; with neither, the rate is inserted.
        /// </remarks>
        public async Task<ShippingRate?> UpdateShippingRate(ShippingRate shippingRate)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            UmbCheckoutStripeShipping? saved;
            try
            {
                // By Key first: the Stripe ID (Value) is what the editor may have changed.
                var existing = shippingRate.Key == Guid.Empty
                    ? null
                    : await scope.Database.QueryAsync<UmbCheckoutStripeShipping>().SingleOrDefault(x => x.Key == shippingRate.Key);
                var byValue = await scope.Database.QueryAsync<UmbCheckoutStripeShipping>().SingleOrDefault(x => x.Value == shippingRate.Value);

                if (existing != null)
                {
                    if (byValue != null && byValue.Id != existing.Id)
                    {
                        _logger.LogWarning("Shipping rate {ShippingRateKey} was not saved: Stripe shipping rate {StripeShippingRateId} belongs to another shipping rate", shippingRate.Key, shippingRate.Value);
                        return null;
                    }

                    existing.Name = shippingRate.Name;
                    existing.Value = shippingRate.Value;
                    await scope.Database.UpdateAsync(existing);
                    saved = existing;
                }
                else if (byValue != null)
                {
                    byValue.Name = shippingRate.Name;
                    if (byValue.Key == Guid.Empty)
                    {
                        byValue.Key = Guid.NewGuid();
                    }

                    await scope.Database.UpdateAsync(byValue);
                    saved = byValue;
                }
                else
                {
                    saved = _mapper.Map<ShippingRate, UmbCheckoutStripeShipping>(shippingRate)!;
                    saved.Id = 0;
                    if (saved.Key == Guid.Empty)
                    {
                        saved.Key = Guid.NewGuid();
                    }

                    await scope.Database.InsertAsync(saved);
                }
            }
            catch (System.Data.Common.DbException ex)
            {
                // A concurrent save took the same Key or Stripe ID.
                _logger.LogWarning(ex, "Shipping rate {ShippingRateKey} ({StripeShippingRateId}) could not be saved", shippingRate.Key, shippingRate.Value);
                return null;
            }

            var updatedShippingRate = _mapper.Map<UmbCheckoutStripeShipping, ShippingRate>(saved);
            scope.Notifications.Publish(new OnShippingRateSavedNotification(updatedShippingRate));
            return updatedShippingRate;
        }

        /// <inheritdoc />
        public async Task<bool> DeleteShippingRate(Guid key)
        {
            try
            {
                using var scope = _scopeProvider.CreateScope(autoComplete: true);

                var shippingRate = await GetShippingRate(key);
                var shippingRatePoco = _mapper.Map<ShippingRate, UmbCheckoutStripeShipping>(shippingRate);

                if (shippingRatePoco == null)
                {
                    return false;
                }

                _ = await scope.Database.DeleteAsync(shippingRatePoco);

                scope.Notifications.Publish(new OnShippingRateDeletedNotification(shippingRate));

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
