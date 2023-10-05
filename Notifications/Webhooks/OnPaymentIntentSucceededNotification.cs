using Stripe;
using Umbraco.Cms.Core.Notifications;

namespace UmbCheckout.Stripe.Notifications.Webhooks
{
    public class OnPaymentIntentSucceededNotification : INotification
    {
        public Event? StripeEvent { get; set; }

        public OnPaymentIntentSucceededNotification(Event? stripeEvent)
        {
            StripeEvent = stripeEvent;
        }
    }
}
