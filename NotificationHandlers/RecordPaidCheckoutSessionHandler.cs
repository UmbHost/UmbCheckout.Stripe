using Stripe;
using Stripe.Checkout;
using UmbCheckout.Stripe.Interfaces;
using UmbCheckout.Stripe.Notifications.Webhooks;
using Umbraco.Cms.Core.Events;

namespace UmbCheckout.Stripe.NotificationHandlers
{
    /// <summary>
    /// Records paid checkout sessions from the webhook, so the customer's next basket read clears the paid basket even if they
    /// never return to the success page. A completed session is paid unless it waits on a delayed payment method, which is
    /// recorded when that payment succeeds. A failure propagates: the webhook answers 500 and Stripe retries.
    /// </summary>
    internal sealed class RecordPaidCheckoutSessionHandler(IStripePaidCheckoutSessions paidCheckoutSessions)
        : INotificationAsyncHandler<OnCheckoutSessionCompletedNotification>, INotificationAsyncHandler<OnPaymentSuccessNotification>
    {
        public Task HandleAsync(OnCheckoutSessionCompletedNotification notification, CancellationToken cancellationToken)
            => Session(notification.StripeEvent) is { PaymentStatus: "paid" or "no_payment_required" } session
                ? paidCheckoutSessions.MarkPaidAsync(session.Id, cancellationToken)
                : Task.CompletedTask;

        public Task HandleAsync(OnPaymentSuccessNotification notification, CancellationToken cancellationToken)
            => Session(notification.StripeEvent) is { } session
                ? paidCheckoutSessions.MarkPaidAsync(session.Id, cancellationToken)
                : Task.CompletedTask;

        private static Session? Session(Event? stripeEvent)
            => stripeEvent?.Data?.Object is Session { Id.Length: > 0 } session ? session : null;
    }
}
