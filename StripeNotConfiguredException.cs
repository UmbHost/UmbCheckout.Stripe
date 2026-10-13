namespace UmbCheckout.Stripe
{
    /// <summary>
    /// The Stripe API key for the active mode (test or live) is not configured, so no Stripe call can be made.
    /// Thrown by the Stripe client factory instead of Stripe.net's generic ArgumentException, so APIs can report it clearly.
    /// </summary>
    public class StripeNotConfiguredException : InvalidOperationException
    {
        public StripeNotConfiguredException(bool live)
            : base($"No Stripe API key is configured for {(live ? "live" : "test")} mode. Set UmbCheckout:Stripe:{(live ? "Live" : "Test")}:ApiKey.")
        {
            Live = live;
        }

        /// <summary>Whether the missing key is the live key.</summary>
        public bool Live { get; }
    }
}
