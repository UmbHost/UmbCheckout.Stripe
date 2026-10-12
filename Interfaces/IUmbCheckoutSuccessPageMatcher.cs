namespace UmbCheckout.Stripe.Interfaces
{
    /// <summary>
    /// Whether a request is for the configured success page, in any of its cultures and domains. The success handler only
    /// clears the basket there, so a session id in another page's query string does nothing.
    /// </summary>
    public interface IUmbCheckoutSuccessPageMatcher
    {
        Task<bool> IsSuccessPageAsync(Uri requestUri, CancellationToken cancellationToken = default);
    }
}
