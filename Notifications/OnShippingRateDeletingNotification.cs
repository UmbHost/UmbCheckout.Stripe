using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using UmbCheckout.Stripe.Models;

namespace UmbCheckout.Stripe.Notifications
{
    /// <summary>
    /// Published before a shipping rate is deleted. Cancel with
    /// <c>CancelOperation(new EventMessage(category, message, EventMessageType.Error))</c> to keep it; the message is shown to the person deleting.
    /// </summary>
    public sealed class OnShippingRateDeletingNotification(ShippingRate target, EventMessages messages)
        : CancelableObjectNotification<ShippingRate>(target, messages)
    {
        /// <summary>The shipping rate about to be deleted.</summary>
        public new ShippingRate Target => base.Target;
    }
}
