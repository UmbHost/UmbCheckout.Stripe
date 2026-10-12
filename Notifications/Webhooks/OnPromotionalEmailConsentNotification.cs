using Stripe;
using Umbraco.Cms.Core.Notifications;

namespace UmbCheckout.Stripe.Notifications.Webhooks
{
    public class OnPromotionalEmailConsentNotification : INotification
    {
        public Event? StripeEvent { get; set; }

        public OnPromotionalEmailConsentNotification(Event? stripeEvent)
        {
            StripeEvent = stripeEvent;
        }
    }
}
