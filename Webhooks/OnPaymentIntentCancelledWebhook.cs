#if NET8_0_OR_GREATER
using Microsoft.Extensions.Options;
using UmbCheckout.Stripe.Notifications.Webhooks;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Sync;
using Umbraco.Cms.Core.Webhooks;

namespace UmbCheckout.Stripe.Webhooks
{
    [WebhookEvent("Payment Intent Cancelled (Stripe)")]
    public class OnPaymentIntentCancelledWebhook(
        IWebhookFiringService webhookFiringService,
        IWebhookService webhookService,
        IOptionsMonitor<WebhookSettings> webhookSettings,
        IServerRoleAccessor serverRoleAccessor)
        : WebhookEventBase<OnPaymentIntentCancelledNotification>(webhookFiringService, webhookService, webhookSettings,
            serverRoleAccessor)
    {
        public override string Alias => "OnPaymentIntentCancelledWebhook";
    }
}

#endif