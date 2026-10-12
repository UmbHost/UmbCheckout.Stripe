namespace UmbCheckout.Stripe.Pocos
{
    /// <summary>A Stripe webhook event that has been handled (or is being handled), so a redelivery is recognised.</summary>
    internal sealed class UmbCheckoutStripeWebhookEvent
    {
        public string EventId { get; set; } = string.Empty;

        public DateTime ReceivedUtc { get; set; }
    }
}
