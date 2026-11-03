using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using UmbCheckout.Stripe.Models;

namespace UmbCheckout.Stripe.Notifications
{
    /// <summary>
    /// Published after the Stripe settings are read, before they are returned; changes to <see cref="Target"/> reach the caller.
    /// Keep handlers cheap: it runs on every read.
    /// </summary>
    public sealed class OnStripeSettingsLoadedNotification(UmbCheckoutStripeSettings target, EventMessages messages)
        : ObjectNotification<UmbCheckoutStripeSettings>(target, messages)
    {
        /// <summary>The Stripe settings about to be returned.</summary>
        public new UmbCheckoutStripeSettings Target => base.Target;
    }
}
