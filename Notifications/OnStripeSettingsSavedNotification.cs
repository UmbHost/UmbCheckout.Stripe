using UmbCheckout.Shared.Models;
using UmbCheckout.Stripe.Models;
using Umbraco.Cms.Core.Notifications;

namespace UmbCheckout.Stripe.Notifications
{
    /// <summary>
    /// Notification which is triggered after the Stripe settings are saved
    /// </summary>
    public class OnStripeSettingsSavedNotification : INotification
    {
        public UmbCheckoutStripeSettings? StripeSettings { get; }

        public OnStripeSettingsSavedNotification(UmbCheckoutStripeSettings? stripeSettings)
        {
            StripeSettings = stripeSettings;
        }
    }
}
