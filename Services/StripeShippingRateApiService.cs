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
        private readonly StripeSettings _stripeSettings;

        public StripeShippingRateApiService(IOptionsMonitor<StripeSettings> stripeSettings)
        {
            _stripeSettings = _stripeSettings = stripeSettings.CurrentValue;
        }

        /// <inheritdoc />
        public async Task<StripeList<ShippingRate>> GetShippingRates()
        {
            var stripeClient = new StripeClient(_stripeSettings.ApiKey);
            var service = new ShippingRateService(stripeClient);

            var shippingRates = await service.ListAsync();

            return shippingRates;
        }

        /// <inheritdoc />
        public async Task<ShippingRate> GetShippingRate(string id)
        {
            var stripeClient = new StripeClient(_stripeSettings.ApiKey);
            var service = new ShippingRateService(stripeClient);

            var shippingRate = await service.GetAsync(id);

            return shippingRate;
        }
    }
}
