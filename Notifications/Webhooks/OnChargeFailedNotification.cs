using Stripe;
using Umbraco.Cms.Core.Notifications;

namespace UmbCheckout.Stripe.Notifications.Webhooks
{
    public class OnChargeFailedNotification : INotification
    {
        public Event? StripeEvent { get; set; }

        public OnChargeFailedNotification(Event? stripeEvent)
        {
            StripeEvent = stripeEvent;
        }
    }
}
