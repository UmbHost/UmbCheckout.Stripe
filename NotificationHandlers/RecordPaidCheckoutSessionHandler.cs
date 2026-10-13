using Microsoft.Extensions.Logging;
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
    /// recorded when that payment succeeds. A failure is logged, not thrown: failing the webhook would make Stripe redeliver the
    /// event and re-run the site's other handlers (fulfilment, emails) for a backstop the success page usually covers.
    /// </summary>
    internal sealed class RecordPaidCheckoutSessionHandler(IStripePaidCheckoutSessions paidCheckoutSessions, ILogger<RecordPaidCheckoutSessionHandler> logger)
        : INotificationAsyncHandler<OnCheckoutSessionCompletedNotification>, INotificationAsyncHandler<OnPaymentSuccessNotification>
    {
        public Task HandleAsync(OnCheckoutSessionCompletedNotification notification, CancellationToken cancellationToken)
            => Session(notification.StripeEvent) is { PaymentStatus: "paid" or "no_payment_required" } session
                ? RecordAsync(session.Id, cancellationToken)
                : Task.CompletedTask;

        public Task HandleAsync(OnPaymentSuccessNotification notification, CancellationToken cancellationToken)
            => Session(notification.StripeEvent) is { } session
                ? RecordAsync(session.Id, cancellationToken)
                : Task.CompletedTask;

        private async Task RecordAsync(string checkoutSessionId, CancellationToken cancellationToken)
        {
            try
            {
                await paidCheckoutSessions.MarkPaidAsync(checkoutSessionId, cancellationToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Could not record checkout session {CheckoutSessionId} as paid; its basket is cleared only if the customer returns to the success page", checkoutSessionId);
            }
        }

        private static Session? Session(Event? stripeEvent)
            => stripeEvent?.Data?.Object is Session { Id.Length: > 0 } session ? session : null;
    }
}
