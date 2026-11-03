using Stripe.Checkout;
using Umbraco.Cms.Core.Notifications;

namespace UmbCheckout.Stripe.Notifications
{
    /// <summary>
    ///     Notification which is triggered after a stripe line item has been added to the
    ///     line item collection that is sent to Stripe.
    ///     Handlers run before the session is sent to Stripe, so their changes are used; a handler that throws stops checkout.
    /// </summary>
    public class OnProviderSessionOptionsLineItemAdded : INotification
    {
        public SessionLineItemOptions LineItem { get; set; }

        public OnProviderSessionOptionsLineItemAdded(SessionLineItemOptions lineItem)
        {
            LineItem = lineItem;
        }
    }
}