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
        public async Task<StripeList<ShippingRate>> GetShippingRates()
        {
            var service = new ShippingRateService(await _stripeClientFactory.CreateAsync());

            return await service.ListAsync();
        }

        /// <inheritdoc />
        public async Task<ShippingRate> GetShippingRate(string id)
        {
            var service = new ShippingRateService(await _stripeClientFactory.CreateAsync());

            return await service.GetAsync(id);
        }
    }
}
