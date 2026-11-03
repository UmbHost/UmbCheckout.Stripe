using Umbraco.Cms.Core.Models.PublishedContent;

namespace UmbCheckout.Stripe.Interfaces
{
    /// <summary>
    /// Whether a request is for the configured success page, in any of its cultures and domains. The success handler only
    /// clears the basket there, so a session id in another page's query string does nothing.
    /// </summary>
    public interface IUmbCheckoutSuccessPageMatcher
    {
        /// <summary>Whether <paramref name="requestUri"/> is a URL of the success page (host and path, any culture).</summary>
        Task<bool> IsSuccessPageAsync(Uri requestUri, CancellationToken cancellationToken = default);

        /// <summary>
        /// Whether the request is for the success page, when the routed page is already known (the success handler runs after
        /// Umbraco's content finders): the page itself is compared, so every culture and domain matches.
        /// </summary>
        /// <remarks>The default implementation (for implementations written before this overload) compares the URL.</remarks>
        Task<bool> IsSuccessPageAsync(IPublishedContent? routedPage, Uri requestUri, CancellationToken cancellationToken = default)
            => IsSuccessPageAsync(requestUri, cancellationToken);
    }
}
