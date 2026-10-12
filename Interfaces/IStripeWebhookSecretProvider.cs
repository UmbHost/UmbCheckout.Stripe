namespace UmbCheckout.Stripe.Interfaces
{
    /// <summary>The webhook signing secret for the active mode (live only when live mode has been saved), as the Stripe client factory picks keys.</summary>
    public interface IStripeWebhookSecretProvider
    {
        /// <returns>The secret (empty when not configured) and whether it is the live one.</returns>
        Task<(string Secret, bool Live)> GetAsync(CancellationToken cancellationToken = default);
    }
}
