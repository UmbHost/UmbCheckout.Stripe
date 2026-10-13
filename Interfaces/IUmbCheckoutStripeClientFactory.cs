using Stripe;

namespace UmbCheckout.Stripe.Interfaces
{
    /// <summary>
    /// Creates the Stripe client for the configured mode (test or live), as v17. Every backoffice Stripe service gets its client here,
    /// so a missing key is reported the same way everywhere, and tests can swap the client.
    /// </summary>
    public interface IUmbCheckoutStripeClientFactory
    {
        /// <summary>Creates the Stripe client for the saved mode (live only when live mode has been saved).</summary>
        /// <exception cref="StripeNotConfiguredException">The mode's API key is missing or empty.</exception>
        Task<IStripeClient> CreateAsync();

        /// <summary>As <see cref="CreateAsync()"/>, stopping when <paramref name="cancellationToken"/> is cancelled.</summary>
        /// <remarks>Implementations written before the token overloads existed ignore the token.</remarks>
        Task<IStripeClient> CreateAsync(CancellationToken cancellationToken) => CreateAsync();
    }
}
