using Stripe.Checkout;
using Umbraco.Cms.Core.Notifications;

namespace UmbCheckout.Stripe.Notifications
{
    /// <summary>
    ///     Notification which is triggered after the initial session options are created
    /// </summary>
    public class OnProviderSessionOptionsStarted : INotification
    {
        public SessionCreateOptions Options { get; set; }

        public OnProviderSessionOptionsStarted(SessionCreateOptions options)
        {
            Options = options;
        }
    }
}
