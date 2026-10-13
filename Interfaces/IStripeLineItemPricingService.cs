using Umbraco.Cms.Core.Models.PublishedContent;

namespace UmbCheckout.Stripe.Interfaces
{
    /// <summary>A product's price, rounded for its currency.</summary>
    public sealed record LineItemPrice(decimal Price, string CurrencyCode);

    /// <summary>Prices a product for the basket from the published content and the server's currency.</summary>
    public interface IStripeLineItemPricingService
    {
        /// <summary>The product's rounded price and currency, or null (logged) when it cannot be sold.</summary>
        Task<LineItemPrice?> GetPriceAsync(IPublishedContent product);

        /// <summary>The product's rounded price and currency, stopping when <paramref name="cancellationToken"/> is cancelled.</summary>
        /// <remarks>Implementations written before this overload existed ignore the token.</remarks>
        Task<LineItemPrice?> GetPriceAsync(IPublishedContent product, CancellationToken cancellationToken) => GetPriceAsync(product);
    }
}
