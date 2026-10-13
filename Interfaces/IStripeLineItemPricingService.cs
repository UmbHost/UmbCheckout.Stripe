using Umbraco.Cms.Core.Models.PublishedContent;

namespace UmbCheckout.Stripe.Interfaces
{
    /// <summary>A product's price, rounded for its currency.</summary>
    public sealed record LineItemPrice(decimal Price, string CurrencyCode);

    /// <summary>Prices a product for the basket from the published content and the server's currency.</summary>
    public interface IStripeLineItemPricingService
    {
        /// <summary>The product's rounded price and currency, or null (logged) when it cannot be sold.</summary>
        [Obsolete("Use GetPriceAsync(IPublishedContent, CancellationToken) (the overload with a CancellationToken). Scheduled for removal in the next major version.")]
        Task<LineItemPrice?> GetPriceAsync(IPublishedContent product);

        /// <summary>The product's rounded price and currency, stopping when <paramref name="cancellationToken"/> is cancelled.</summary>
        /// <remarks>Implementations written before this overload existed ignore the token.</remarks>
#pragma warning disable CS0618 // forwards to the old overload so implementations written before the token overload keep working
        Task<LineItemPrice?> GetPriceAsync(IPublishedContent product, CancellationToken cancellationToken) => GetPriceAsync(product);
#pragma warning restore CS0618
    }
}
