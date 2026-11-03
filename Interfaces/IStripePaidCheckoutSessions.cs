using UmbCheckout.Core.Interfaces;

namespace UmbCheckout.Stripe.Interfaces
{
    /// <summary>Stripe checkout sessions the webhook has reported paid, kept 30 days.</summary>
    public interface IStripePaidCheckoutSessions : IPaidCheckoutSessions
    {
        /// <summary>Records the checkout session as paid (recording it again is harmless).</summary>
        Task MarkPaidAsync(string checkoutSessionId, CancellationToken cancellationToken = default);
    }
}
