#if NET8_0_OR_GREATER
using Microsoft.Extensions.Options;
using UmbCheckout.Stripe.Notifications.Webhooks;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Sync;
using Umbraco.Cms.Core.Webhooks;

namespace UmbCheckout.Stripe.Webhooks
{
    [WebhookEvent("Payment Failed (Stripe)")]
    public class OnPaymentFailedWebhook(
        IWebhookFiringService webhookFiringService,
        IWebhookService webhookService,
        IOptionsMonitor<WebhookSettings> webhookSettings,
        IServerRoleAccessor serverRoleAccessor)
        : WebhookEventBase<OnPaymentFailedNotification>(webhookFiringService, webhookService, webhookSettings,
            serverRoleAccessor)
    {
        public override string Alias => "OnPaymentFailedWebhook";
    }
}

#endif