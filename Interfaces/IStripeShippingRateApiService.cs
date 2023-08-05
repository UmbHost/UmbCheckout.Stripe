using Stripe;

namespace UmbCheckout.Stripe.Interfaces
{
    /// <summary>
    /// A service which handles getting the Stripe Shipping Rates from the Stripe API
    /// </summary>
    public interface IStripeShippingRateApiService
    {
        /// <summary>
        /// Gets the Shipping Rates from the Stripe API
        /// </summary>
        /// <returns>A list of Shipping Rates from the Stripe API</returns>
        Task<StripeList<ShippingRate>> GetShippingRates();

        /// <summary>
        /// Gets a Shipping Rate from the Stripe API
        /// </summary>
        /// <param name="id">Id of the Stripe Shipping Rate</param>
        /// <returns>The Stripe Shipping Rate from the Stripe API</returns>
        Task<ShippingRate> GetShippingRate(string id);
    }
}
