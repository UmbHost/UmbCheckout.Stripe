using Microsoft.Extensions.Options;
using Stripe;
using UmbCheckout.Stripe.Interfaces;
using UmbCheckout.Stripe.Models;
using ShippingRate = Stripe.ShippingRate;

namespace UmbCheckout.Stripe.Services
{
    /// <summary>
    /// A service which handles getting the Stripe Shipping Rates from the Stripe API
    /// </summary>
    internal class StripeShippingRateApiService : IStripeShippingRateApiService
    {
        private readonly IUmbCheckoutStripeClientFactory _stripeClientFactory;

        public StripeShippingRateApiService(IUmbCheckoutStripeClientFactory stripeClientFactory)
        {
            _stripeClientFactory = stripeClientFactory;
        }

        /// <inheritdoc />
        public Task<StripeList<ShippingRate>> GetShippingRates() => GetShippingRates(CancellationToken.None);

        /// <inheritdoc />
        public async Task<StripeList<ShippingRate>> GetShippingRates(CancellationToken cancellationToken)
        {
            var service = new ShippingRateService(await _stripeClientFactory.CreateAsync(cancellationToken));

            return await service.ListAsync(cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public Task<ShippingRate> GetShippingRate(string id) => GetShippingRate(id, CancellationToken.None);

        /// <inheritdoc />
        public async Task<ShippingRate> GetShippingRate(string id, CancellationToken cancellationToken)
        {
            var service = new ShippingRateService(await _stripeClientFactory.CreateAsync(cancellationToken));

            return await service.GetAsync(id, cancellationToken: cancellationToken);
        }
    }
}
