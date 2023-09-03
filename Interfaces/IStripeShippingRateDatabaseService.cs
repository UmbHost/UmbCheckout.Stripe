using UmbCheckout.Stripe.Models;

namespace UmbCheckout.Stripe.Interfaces
{
    /// <summary>
    /// A service which handles getting the Stripe Shipping Rates from the database
    /// </summary>
    public interface IStripeShippingRateDatabaseService
    {
        /// <summary>
        /// Gets the Shipping Rates
        /// </summary>
        /// <returns>An IEnumerable of Shipping Rates</returns>
        Task<IEnumerable<ShippingRate>> GetShippingRates();

        /// <summary>
        /// Gets a Shipping Rate
        /// </summary>
        /// <param name="key">Key of the Stripe Shipping Rate</param>
        /// <returns>The Stripe Shipping Rate</returns>
        Task<ShippingRate?> GetShippingRate(Guid key);

        /// <summary>
        /// Gets a Shipping Rate
        /// </summary>
        /// <param name="value">StripeId of the Stripe Shipping Rate</param>
        /// <returns>The Stripe Shipping Rate</returns>
        Task<ShippingRate?> GetShippingRate(string value);

        /// <summary>
        /// Creates a Shipping Rate
        /// </summary>
        /// <param name="shippingRate">The Stripe Shipping Rate</param>
        /// <returns>The created Stripe Shipping Rate</returns>
        Task<ShippingRate?> CreateShippingRate(ShippingRate shippingRate);

        /// <summary>
        /// Updates a Shipping Rate
        /// </summary>
        /// <param name="shippingRate">The Stripe Shipping Rate</param>
        /// <returns>The updated Stripe Shipping Rate</returns>
        Task<ShippingRate?> UpdateShippingRate(ShippingRate shippingRate);

        /// <summary>
        /// Deletes a Shipping Rate
        /// </summary>
        /// <param name="key">Key of the Stripe Shipping Rate</param>
        /// <returns>True if deleted</returns>
        Task<bool> DeleteShippingRate(Guid key);
    }
}
