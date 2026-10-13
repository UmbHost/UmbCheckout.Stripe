using UmbCheckout.Core.Interfaces;

namespace UmbCheckout.Stripe.Interfaces
{
    /// <summary>
    /// Stripe checkout sessions the webhook has reported paid, kept 30 days. Basket reads ask it (as
    /// <see cref="IPaidCheckoutSessions"/>), so a customer who paid but never returned to the success page doesn't keep the paid
    /// basket. Replaceable: register your own implementation to keep paid sessions elsewhere.
    /// </summary>
    public interface IStripePaidCheckoutSessions : IPaidCheckoutSessions
    {
        /// <summary>Records the checkout session as paid (recording it again is harmless).</summary>
        Task MarkPaidAsync(string checkoutSessionId, CancellationToken cancellationToken = default);
    }
}
