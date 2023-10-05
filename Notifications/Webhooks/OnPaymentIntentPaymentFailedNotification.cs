using Stripe;
using Umbraco.Cms.Core.Notifications;

namespace UmbCheckout.Stripe.Notifications.Webhooks
{
    public class OnPaymentIntentPaymentFailedNotification : INotification
    {
        public Event? StripeEvent { get; set; }

        public OnPaymentIntentPaymentFailedNotification(Event? stripeEvent)
        {
            StripeEvent = stripeEvent;
        }
    }
}
