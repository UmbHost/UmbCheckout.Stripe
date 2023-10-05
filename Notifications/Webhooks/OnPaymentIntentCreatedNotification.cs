using Stripe;
using Umbraco.Cms.Core.Notifications;

namespace UmbCheckout.Stripe.Notifications.Webhooks
{
    public class OnPaymentIntentCreatedNotification : INotification
    {
        public Event? StripeEvent { get; set; }

        public OnPaymentIntentCreatedNotification(Event? stripeEvent)
        {
            StripeEvent = stripeEvent;
        }
    }
}
