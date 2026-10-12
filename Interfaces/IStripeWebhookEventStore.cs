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
    /// Records which Stripe webhook events are being handled or are done. Stripe delivers each event at least once and retries
    /// for up to 3 days; an event is claimed with a lease while its handlers run, and marked done when they succeed, so an
    /// attempt that crashes or times out is taken over once its lease expires instead of being lost.
    /// </summary>
    public interface IStripeWebhookEventStore
    {
        /// <summary>Claims the event for this delivery.</summary>
        Task<StripeWebhookEventClaim> TryClaimAsync(string eventId, CancellationToken cancellationToken = default);

        /// <summary>Marks the event as handled; later redeliveries are acknowledged without running handlers.</summary>
        Task CompleteAsync(string eventId, CancellationToken cancellationToken = default);

        /// <summary>Releases the claim after a handler failed, so Stripe's retry runs the handlers again.</summary>
        Task ForgetAsync(string eventId, CancellationToken cancellationToken = default);
    }
}
