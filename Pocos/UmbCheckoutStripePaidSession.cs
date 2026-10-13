namespace UmbCheckout.Stripe.Pocos
{
    /// <summary>A Stripe checkout session the webhook reported paid: the next basket read clears the basket it paid for.</summary>
    internal sealed class UmbCheckoutStripePaidSession
    {
        public string CheckoutSessionId { get; set; } = string.Empty;

        public DateTime PaidUtc { get; set; }
    }
}
