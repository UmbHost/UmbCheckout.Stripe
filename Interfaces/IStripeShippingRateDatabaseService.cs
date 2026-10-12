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
        Task<IEnumerable<ShippingRate>> GetShippingRatesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a Shipping Rate
        /// </summary>
        /// <param name="key">Key of the Stripe Shipping Rate</param>
        /// <returns>The Stripe Shipping Rate</returns>
        Task<ShippingRate?> GetShippingRateAsync(Guid key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a Shipping Rate
        /// </summary>
        /// <param name="value">StripeId of the Stripe Shipping Rate</param>
        /// <returns>The Stripe Shipping Rate</returns>
        Task<ShippingRate?> GetShippingRateAsync(string value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a Shipping Rate
        /// </summary>
        /// <param name="shippingRate">The Stripe Shipping Rate</param>
        /// <returns>The created Stripe Shipping Rate, or null if one with the same Stripe id already exists</returns>
        Task<ShippingRate?> CreateShippingRateAsync(ShippingRate shippingRate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates a Shipping Rate, matched on its Stripe id; creates it when it does not exist
        /// </summary>
        /// <param name="shippingRate">The Stripe Shipping Rate</param>
        /// <returns>The updated Stripe Shipping Rate</returns>
        Task<ShippingRate?> UpdateShippingRateAsync(ShippingRate shippingRate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a Shipping Rate
        /// </summary>
        /// <param name="key">Key of the Stripe Shipping Rate</param>
        /// <returns>True if deleted</returns>
        Task<bool> DeleteShippingRateAsync(Guid key, CancellationToken cancellationToken = default);
    }
}
