using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using UmbCheckout.Stripe.Models;

namespace UmbCheckout.Stripe.Notifications
{
    /// <summary>
    /// Published before a shipping rate is created or updated. Change <see cref="Target"/> to change what is saved, or cancel with
    /// <c>CancelOperation(new EventMessage(category, message, EventMessageType.Error))</c>; the message is shown to the person saving.
    /// </summary>
    public sealed class OnShippingRateSavingNotification(ShippingRate target, ShippingRate? existing, EventMessages messages)
        : CancelableObjectNotification<ShippingRate>(target, messages)
    {
        /// <summary>The shipping rate about to be saved.</summary>
        public new ShippingRate Target => base.Target;

        /// <summary>The stored shipping rate, or null on create. For comparison; changes are ignored.</summary>
        public ShippingRate? Existing { get; } = existing;
    }
}
