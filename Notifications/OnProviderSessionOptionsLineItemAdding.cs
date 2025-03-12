using Stripe.Checkout;
using Umbraco.Cms.Core.Notifications;

namespace UmbCheckout.Stripe.Notifications
{
    /// <summary>
    ///     Notification which is triggered before a stripe line item is added to the
    ///     line item collection that is sent to Stripe
    /// </summary>
    public class OnProviderSessionOptionsLineItemAdding : INotification
    {
        public SessionLineItemOptions LineItem { get; set; }

        public OnProviderSessionOptionsLineItemAdding(SessionLineItemOptions lineItem)
        {
            LineItem = lineItem;
        }
    }
}