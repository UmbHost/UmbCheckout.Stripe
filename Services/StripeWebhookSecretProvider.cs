using Microsoft.Extensions.Options;
using UmbCheckout.Stripe.Interfaces;
using UmbCheckout.Stripe.Models;

namespace UmbCheckout.Stripe.Services
{
    /// <inheritdoc />
    internal sealed class StripeWebhookSecretProvider : IStripeWebhookSecretProvider
    {
        private readonly IOptionsMonitor<StripeSettings> _options;
        private readonly IStripeSettingsService _settingsService;

        public StripeWebhookSecretProvider(IOptionsMonitor<StripeSettings> options, IStripeSettingsService settingsService)
        {
            _options = options;
            _settingsService = settingsService;
        }

        public async Task<(string Secret, bool Live)> GetAsync(CancellationToken cancellationToken = default)
        {
            var live = (await _settingsService.GetStripeSettings())?.UseLiveApiDetails == true;
            var keys = live ? _options.CurrentValue.Live : _options.CurrentValue.Test;
            return (keys?.WebHookSecret ?? string.Empty, live);
        }
    }
}
