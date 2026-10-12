using Stripe;

namespace UmbCheckout.Stripe.Interfaces
{
    /// <summary>
    /// Creates the Stripe client for the configured mode (test or live). Injected so tests can point Stripe services at stripe-mock.
    /// </summary>
    public interface IUmbCheckoutStripeClientFactory
    {
        Task<IStripeClient> CreateAsync();
    }
}
