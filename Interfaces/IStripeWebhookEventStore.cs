namespace UmbCheckout.Stripe.Interfaces
{
    /// <summary>The outcome of claiming a Stripe webhook event.</summary>
    public enum StripeWebhookEventClaim
    {
        /// <summary>This delivery should run the handlers (first delivery, or a takeover after an earlier attempt died).</summary>
        Claimed,

        /// <summary>The handlers already completed for this event: acknowledge the redelivery.</summary>
        AlreadyHandled,

        /// <summary>Another delivery is running the handlers now: answer non-2xx so Stripe retries later.</summary>
        InProgress
    }

    /// <summary>
    /// The result of claiming a Stripe webhook event: the outcome and, when <see cref="StripeWebhookEventClaim.Claimed"/>, the
    /// lease token that identifies this delivery's claim. Completing or releasing the event takes the token, so an attempt that
    /// outlived its lease can't complete or release the claim of the delivery that took the event over.
    /// </summary>
    public readonly record struct StripeWebhookEventClaimResult(StripeWebhookEventClaim Claim, string? LeaseToken)
    {
        /// <summary>A result that isn't a claim (already handled, or in progress elsewhere).</summary>
        public static StripeWebhookEventClaimResult Of(StripeWebhookEventClaim claim) => new(claim, null);
    }

    /// <summary>
    /// Records which Stripe webhook events are being handled or are done. Stripe delivers each event at least once and retries
    /// for up to 3 days; an event is claimed with a lease while its handlers run, and marked done when they succeed, so an
    /// attempt that crashes or times out is taken over once its lease expires instead of being lost.
    /// </summary>
    public interface IStripeWebhookEventStore
    {
        /// <summary>Claims the event for this delivery; a claim carries the lease token this delivery completes or releases with.</summary>
        Task<StripeWebhookEventClaimResult> TryClaimAsync(string eventId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Marks the event as handled, if <paramref name="leaseToken"/> still holds the claim; later redeliveries are acknowledged
        /// without running handlers.
        /// </summary>
        Task CompleteAsync(string eventId, string leaseToken, CancellationToken cancellationToken = default);

        /// <summary>
        /// Releases the claim after a handler failed, if <paramref name="leaseToken"/> still holds it, so Stripe's retry runs the
        /// handlers again. A claim taken over by another delivery is left to that delivery.
        /// </summary>
        Task ForgetAsync(string eventId, string leaseToken, CancellationToken cancellationToken = default);
    }
}
