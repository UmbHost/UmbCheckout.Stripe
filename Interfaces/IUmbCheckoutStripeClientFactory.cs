using Stripe;

namespace UmbCheckout.Stripe.Interfaces
{
    /// <summary>
    /// Creates the Stripe client for the configured mode (test or live). Injected so tests can point Stripe services at stripe-mock.
    /// </summary>
    public interface IUmbCheckoutStripeClientFactory
    {
        Task<IStripeClient> CreateAsync();

        /// <summary>Creates the Stripe client, stopping when <paramref name="cancellationToken"/> is cancelled (it reads the saved settings).</summary>
        /// <remarks>Implementations written before this overload existed ignore the token.</remarks>
        Task<IStripeClient> CreateAsync(CancellationToken cancellationToken) => CreateAsync();
    }
}
