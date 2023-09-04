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
        private readonly IStripeSettingsService _stripeSettingsService;

        public StripeShippingRateApiService(IOptionsMonitor<StripeSettings> stripeSettings, IStripeSettingsService stripeSettingsService)
        {
            _stripeSettingsService = stripeSettingsService;
            _stripeSettings = _stripeSettings = stripeSettings.CurrentValue;
        }

        /// <inheritdoc />
        public async Task<StripeList<ShippingRate>> GetShippingRates()
        {
            var apiKey = string.Empty;
            var stripeSettings = _stripeSettingsService.GetStripeSettings().Result;
            if (stripeSettings != null)
            {
                apiKey = stripeSettings.UseLiveApiDetails ? _stripeSettings.Live.ApiKey : _stripeSettings.Test.ApiKey;
            }

            var stripeClient = new StripeClient(apiKey);
            var service = new ShippingRateService(stripeClient);

            var shippingRates = await service.ListAsync();

            return shippingRates;
        }

        /// <inheritdoc />
        public async Task<ShippingRate> GetShippingRate(string id)
        {
            var apiKey = string.Empty;
            var stripeSettings = _stripeSettingsService.GetStripeSettings().Result;
            if (stripeSettings != null)
            {
                apiKey = stripeSettings.UseLiveApiDetails ? _stripeSettings.Live.ApiKey : _stripeSettings.Test.ApiKey;
            }

            var stripeClient = new StripeClient(apiKey);
            var service = new ShippingRateService(stripeClient);

            var shippingRate = await service.GetAsync(id);

            return shippingRate;
        }
    }
}
