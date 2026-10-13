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
        [Obsolete("Use CreateAsync(CancellationToken) (the overload with a CancellationToken). Scheduled for removal in 17.1.")]
        Task<IStripeClient> CreateAsync();

        /// <summary>As <see cref="CreateAsync()"/>, stopping when <paramref name="cancellationToken"/> is cancelled.</summary>
        /// <remarks>Implementations written before the token overloads existed ignore the token.</remarks>
#pragma warning disable CS0618 // forwards to the old overload so implementations written before the token overload keep working
        Task<IStripeClient> CreateAsync(CancellationToken cancellationToken) => CreateAsync();
#pragma warning restore CS0618
    }
}
