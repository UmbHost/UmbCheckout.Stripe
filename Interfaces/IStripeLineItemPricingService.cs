using Umbraco.Cms.Core.Models.PublishedContent;

namespace UmbCheckout.Stripe.Interfaces
{
    /// <summary>A product's price, rounded for its currency (an upper-case ISO 4217 code Stripe supports).</summary>
    public sealed record LineItemPrice(decimal Price, string CurrencyCode);

    /// <summary>Prices a product for the basket from the published content and the server's currency.</summary>
    public interface IStripeLineItemPricingService
    {
        /// <summary>
        /// The product's rounded price and currency, or null (logged) when it cannot be sold here: no valid price, no supported
        /// currency, or a currency other than the current page's (its basket would not be the one shown on this page).
        /// </summary>
        Task<LineItemPrice?> GetPriceAsync(IPublishedContent product);
    }
}
