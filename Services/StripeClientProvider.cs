using Microsoft.Extensions.Options;
using Stripe;
using UmbCheckout.Stripe.Interfaces;
using UmbCheckout.Stripe.Models;

namespace UmbCheckout.Stripe.Services
{
    /// <summary>
    /// Resolves the Stripe API details (test or live) and creates Stripe clients
    /// </summary>
    public sealed class StripeClientProvider
    {
        private readonly IOptionsMonitor<StripeSettings> _stripeSettings;
        private readonly IStripeSettingsService _stripeSettingsService;

        public StripeClientProvider(IOptionsMonitor<StripeSettings> stripeSettings, IStripeSettingsService stripeSettingsService)
        {
            _stripeSettings = stripeSettings;
            _stripeSettingsService = stripeSettingsService;
        }

        /// <summary>
        /// Gets the API details for the configured mode, defaulting to test mode when the Stripe settings have not been saved yet
        /// </summary>
        public async Task<(StripeApiDetails ApiDetails, string ConfigurationPath)> GetApiDetailsAsync()
        {
            var stripeSettings = await _stripeSettingsService.GetStripeSettings();
            var useLiveApiDetails = stripeSettings?.UseLiveApiDetails ?? false;
            var mode = useLiveApiDetails ? nameof(StripeSettings.Live) : nameof(StripeSettings.Test);

            return (useLiveApiDetails ? _stripeSettings.CurrentValue.Live : _stripeSettings.CurrentValue.Test,
                $"{Shared.Consts.PackageName}:{Consts.AppSettingsSectionName}:{mode}");
        }

        /// <summary>
        /// Creates a Stripe client using the API key for the configured mode
        /// </summary>
        /// <exception cref="InvalidOperationException">When no API key is configured</exception>
        public async Task<StripeClient> CreateClientAsync()
        {
            var (apiDetails, configurationPath) = await GetApiDetailsAsync();

            if (string.IsNullOrWhiteSpace(apiDetails.ApiKey))
            {
                throw new InvalidOperationException($"No Stripe API key has been configured, set {configurationPath}:{nameof(StripeApiDetails.ApiKey)} in the appsettings");
            }

            return new StripeClient(apiDetails.ApiKey);
        }
    }
}
