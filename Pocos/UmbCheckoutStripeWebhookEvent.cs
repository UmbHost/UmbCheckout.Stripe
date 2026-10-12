namespace UmbCheckout.Stripe.Pocos
{
    /// <summary>A Stripe webhook event being handled (with a lease) or handled, so a redelivery is recognised.</summary>
    internal sealed class UmbCheckoutStripeWebhookEvent
    {
        public const int Processing = 0;
        public const int Done = 1;

        public string EventId { get; set; } = string.Empty;

        public DateTime ReceivedUtc { get; set; }

        /// <summary><see cref="Processing"/> or <see cref="Done"/>.</summary>
        public int Status { get; set; }

        /// <summary>While processing: when another delivery may take the event over (the first attempt is presumed dead).</summary>
        public DateTime? LeaseUntilUtc { get; set; }
    }
}
