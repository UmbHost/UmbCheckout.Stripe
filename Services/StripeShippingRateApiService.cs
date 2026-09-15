using Stripe;
using UmbCheckout.Stripe.Interfaces;
using ShippingRate = Stripe.ShippingRate;

namespace UmbCheckout.Stripe.Services
{
    /// <summary>
    /// A service which handles getting the Stripe Shipping Rates from the Stripe API
    /// </summary>
    internal class StripeShippingRateApiService : IStripeShippingRateApiService
    {
        private readonly StripeClientProvider _stripeClientProvider;

        public StripeShippingRateApiService(StripeClientProvider stripeClientProvider)
        {
            _stripeClientProvider = stripeClientProvider;
        }

        /// <inheritdoc />
        public async Task<StripeList<ShippingRate>> GetShippingRates()
        {
            var stripeClient = await _stripeClientProvider.CreateClientAsync();
            var service = new ShippingRateService(stripeClient);

            var shippingRates = await service.ListAsync();

            return shippingRates;
        }

        /// <inheritdoc />
        public async Task<ShippingRate> GetShippingRate(string id)
        {
            var stripeClient = await _stripeClientProvider.CreateClientAsync();
            var service = new ShippingRateService(stripeClient);

            var shippingRate = await service.GetAsync(id);

            return shippingRate;
        }
    }
}
