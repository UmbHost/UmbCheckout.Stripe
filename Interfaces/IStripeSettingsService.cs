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
        /// <returns>The configuration</returns>
        Task<Models.UmbCheckoutStripeSettings?> GetStripeSettings();

        /// <summary>
        /// Updates the Stripe settings
        /// </summary>
        /// <param name="stripeSettings">The Stripe settings</param>
        /// <returns>true if update was successful false if not</returns>
        Task<bool> UpdateStripeSettings(Models.UmbCheckoutStripeSettings stripeSettings);
    }
}
