using Stripe;
using Umbraco.Cms.Core.Notifications;

namespace UmbCheckout.Stripe.Notifications.Webhooks
{
    /// <summary>
    /// Published for Stripe's <c>checkout.session.async_payment_succeeded</c> webhook event. Handlers can run more than once: if any handler throws, the webhook
    /// answers 500 and Stripe redelivers the event, running every handler again, so make handlers idempotent (as Stripe requires).
    /// </summary>
    public class OnPaymentSuccessNotification : INotification
    {
        public Event? StripeEvent { get; set; }

        public OnPaymentSuccessNotification(Event? stripeEvent)
        {
            StripeEvent = stripeEvent;
        }
    }
}
