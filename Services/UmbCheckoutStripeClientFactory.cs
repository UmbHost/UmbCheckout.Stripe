using Microsoft.Extensions.Options;
using Stripe;
using UmbCheckout.Stripe.Interfaces;
using UmbCheckout.Stripe.Models;

namespace UmbCheckout.Stripe.Services
{
    /// <inheritdoc />
    public class UmbCheckoutStripeClientFactory : IUmbCheckoutStripeClientFactory
    {
        private readonly IOptionsMonitor<StripeSettings> _stripeSettings;
        private readonly IStripeSettingsService _stripeSettingsService;

        public UmbCheckoutStripeClientFactory(IOptionsMonitor<StripeSettings> stripeSettings, IStripeSettingsService stripeSettingsService)
        {
            _stripeSettings = stripeSettings;
            _stripeSettingsService = stripeSettingsService;
        }

        /// <inheritdoc />
        public async Task<IStripeClient> CreateAsync()
        {
            var settings = await _stripeSettingsService.GetStripeSettings();
            // Live keys are used only when live mode has been explicitly saved.
            var keys = settings?.UseLiveApiDetails == true ? _stripeSettings.CurrentValue.Live : _stripeSettings.CurrentValue.Test;
            return new StripeClient(keys.ApiKey);
        }
    }
}
