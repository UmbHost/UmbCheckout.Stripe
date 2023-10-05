using Stripe;
using Umbraco.Cms.Core.Notifications;

namespace UmbCheckout.Stripe.Notifications.Webhooks
{
    public class OnChargeSucceededNotification : INotification
    {
        public Event? StripeEvent { get; set; }

        public OnChargeSucceededNotification(Event? stripeEvent)
        {
            StripeEvent = stripeEvent;
        }
    }
}
