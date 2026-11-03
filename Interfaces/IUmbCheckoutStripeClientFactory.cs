using Stripe;

namespace UmbCheckout.Stripe.Interfaces
{
    /// <summary>
    /// Creates the Stripe client for the configured mode (test or live). Injected so tests can point Stripe services at stripe-mock.
    /// </summary>
    public interface IUmbCheckoutStripeClientFactory
    {
        [Obsolete("Use CreateAsync(CancellationToken) (the overload with a CancellationToken). Scheduled for removal in 17.1.")]
        Task<IStripeClient> CreateAsync();

        /// <summary>Creates the Stripe client, stopping when <paramref name="cancellationToken"/> is cancelled (it reads the saved settings).</summary>
        /// <remarks>Implementations written before this overload existed ignore the token.</remarks>
#pragma warning disable CS0618 // forwards to the old overload so implementations written before the token overload keep working
        Task<IStripeClient> CreateAsync(CancellationToken cancellationToken) => CreateAsync();
#pragma warning restore CS0618
    }
}
