using Microsoft.Extensions.Options;
using UmbCheckout.Stripe.Interfaces;
using UmbCheckout.Stripe.Models;

namespace UmbCheckout.Stripe.Services
{
    /// <inheritdoc />
    internal sealed class StripeWebhookSecretProvider(IOptionsMonitor<StripeSettings> options, IStripeSettingsService settingsService) : IStripeWebhookSecretProvider
    {
        public async Task<(string Secret, bool Live)> GetAsync(CancellationToken cancellationToken = default)
        {
            var live = (await settingsService.GetStripeSettingsAsync(cancellationToken))?.UseLiveApiDetails == true;
            var keys = live ? options.CurrentValue.Live : options.CurrentValue.Test;
            return (keys.WebHookSecret ?? string.Empty, live);
        }
    }
}
