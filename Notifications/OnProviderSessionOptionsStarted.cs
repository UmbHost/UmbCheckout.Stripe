using Stripe.Checkout;
using Umbraco.Cms.Core.Notifications;

namespace UmbCheckout.Stripe.Notifications
{
    /// <summary>
    ///     Notification which is triggered after the initial session options are created.
    ///     Handlers run before the session is sent to Stripe, so their changes are used; a handler that throws stops checkout.
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
