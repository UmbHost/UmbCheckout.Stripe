namespace UmbCheckout.Stripe.Interfaces
{
    /// <summary>
    /// Remembers which Stripe webhook events have been handled. Stripe delivers each event at least once and retries for up
    /// to 3 days, so the webhook handles an event only the first time it arrives.
    /// </summary>
    public interface IStripeWebhookEventStore
    {
        /// <summary>Records the event as being handled; false when it was recorded before (a redelivery).</summary>
        Task<bool> TryBeginAsync(string eventId, CancellationToken cancellationToken = default);

        /// <summary>Removes the record so Stripe's retry is handled again (after a handler failed).</summary>
        Task ForgetAsync(string eventId, CancellationToken cancellationToken = default);
    }
}
