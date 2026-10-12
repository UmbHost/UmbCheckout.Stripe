namespace UmbCheckout.Stripe.Interfaces
{
    /// <summary>
    /// A service which Gets or Updates the Stripe settings
    /// </summary>
    public interface IStripeSettingsService
    {
        /// <summary>
        /// Gets the Stripe settings
        /// </summary>
        /// <returns>The Stripe settings, or null before they are first saved</returns>
        Task<Models.UmbCheckoutStripeSettings?> GetStripeSettingsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Inserts or updates the Stripe settings
        /// </summary>
        /// <param name="stripeSettings">The Stripe settings</param>
        /// <returns>true when saved</returns>
        Task<bool> UpdateStripeSettingsAsync(Models.UmbCheckoutStripeSettings stripeSettings, CancellationToken cancellationToken = default);
    }
}
