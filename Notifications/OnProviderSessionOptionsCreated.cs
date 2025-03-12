using Stripe.Checkout;
using Umbraco.Cms.Core.Notifications;

namespace UmbCheckout.Stripe.Notifications
{
    /// <summary>
    ///     Notification which is triggered after the session options have been created
    /// </summary>
    public class OnProviderSessionOptionsCreated : INotification
    {
        public SessionCreateOptions Options { get; set; }

        public OnProviderSessionOptionsCreated(SessionCreateOptions options)
        {
            Options = options;
        }
    }
}