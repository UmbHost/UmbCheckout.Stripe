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
        [Obsolete("Use GetShippingRates(CancellationToken) (the overload with a CancellationToken). Scheduled for removal in 17.1.")]
        Task<StripeList<ShippingRate>> GetShippingRates();

        /// <summary>Gets the Shipping Rates from the Stripe API, stopping when <paramref name="cancellationToken"/> is cancelled.</summary>
        /// <remarks>Implementations written before the token overloads existed ignore the token.</remarks>
#pragma warning disable CS0618 // forwards to the old overload so implementations written before the token overload keep working
        Task<StripeList<ShippingRate>> GetShippingRates(CancellationToken cancellationToken) => GetShippingRates();
#pragma warning restore CS0618

        /// <summary>
        /// Gets a Shipping Rate from the Stripe API
        /// </summary>
        /// <param name="id">Id of the Stripe Shipping Rate</param>
        /// <returns>The Stripe Shipping Rate from the Stripe API</returns>
        [Obsolete("Use GetShippingRate(string, CancellationToken) (the overload with a CancellationToken). Scheduled for removal in 17.1.")]
        Task<ShippingRate> GetShippingRate(string id);

        /// <summary>Gets a Shipping Rate from the Stripe API, stopping when <paramref name="cancellationToken"/> is cancelled.</summary>
#pragma warning disable CS0618 // forwards to the old overload so implementations written before the token overload keep working
        Task<ShippingRate> GetShippingRate(string id, CancellationToken cancellationToken) => GetShippingRate(id);
#pragma warning restore CS0618
    }
}
