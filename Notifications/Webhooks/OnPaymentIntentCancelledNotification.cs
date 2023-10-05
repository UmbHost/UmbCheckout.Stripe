using Stripe;
using Umbraco.Cms.Core.Notifications;

namespace UmbCheckout.Stripe.Notifications.Webhooks
{
    public class OnPaymentIntentCancelledNotification : INotification
    {
        public Event? StripeEvent { get; set; }

        public OnPaymentIntentCancelledNotification(Event? stripeEvent)
        {
            StripeEvent = stripeEvent;
        }
    }
}
