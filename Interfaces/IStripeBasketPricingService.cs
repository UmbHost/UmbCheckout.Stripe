using UmbCheckout.Shared.Models;

namespace UmbCheckout.Stripe.Interfaces
{
    /// <summary>A basket line whose price changed (NewPrice set) or that was removed (NewPrice null) at checkout.</summary>
    public sealed record RepricedLine(Guid Key, string Name, decimal OldPrice, decimal? NewPrice)
    {
        public bool Removed => NewPrice is null;
    }

    /// <summary>The basket at current prices and what changed.</summary>
    public sealed record BasketRepricing(Basket Basket, IReadOnlyList<RepricedLine> Changes);

    /// <summary>Re-prices a basket from published content before checkout.</summary>
    public interface IStripeBasketPricingService
    {
        /// <summary>
        /// Brings every line to its current published price; drops lines whose product is unpublished, deleted, unpriced or
        /// now sold in another currency.
        /// </summary>
        [Obsolete("Use RepriceAsync(Basket, CancellationToken) (the overload with a CancellationToken). Scheduled for removal in the next major version.")]
        Task<BasketRepricing> RepriceAsync(Basket basket);

        /// <summary>Re-prices <paramref name="basket"/>, stopping when <paramref name="cancellationToken"/> is cancelled.</summary>
        /// <remarks>Implementations written before this overload existed ignore the token.</remarks>
#pragma warning disable CS0618 // forwards to the old overload so implementations written before the token overload keep working
        Task<BasketRepricing> RepriceAsync(Basket basket, CancellationToken cancellationToken) => RepriceAsync(basket);
#pragma warning restore CS0618
    }
}
