using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using UmbCheckout.Stripe.Models;

namespace UmbCheckout.Stripe.Notifications
{
    /// <summary>
    /// Published before the Stripe settings are saved. Change <see cref="Target"/> to change what is saved, or cancel with
    /// <c>CancelOperation(new EventMessage(category, message, EventMessageType.Error))</c>; the message is shown to the person saving.
    /// </summary>
    public sealed class OnStripeSettingsSavingNotification(UmbCheckoutStripeSettings target, UmbCheckoutStripeSettings? existing, EventMessages messages)
        : CancelableObjectNotification<UmbCheckoutStripeSettings>(target, messages)
    {
        /// <summary>The Stripe settings about to be saved.</summary>
        public new UmbCheckoutStripeSettings Target => base.Target;

        /// <summary>The stored Stripe settings, or null when none have been saved yet. For comparison; changes are ignored.</summary>
        public UmbCheckoutStripeSettings? Existing { get; } = existing;
    }
}
