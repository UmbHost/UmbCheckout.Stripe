using UmbCheckout.Shared.Models;

namespace UmbCheckout.Stripe.Interfaces
{
    /// <summary>A basket line whose price changed (<see cref="NewPrice"/> set) or that was removed (<see cref="NewPrice"/> null) at checkout.</summary>
    public sealed record RepricedLine(Guid Key, string Name, decimal OldPrice, decimal? NewPrice)
    {
        /// <summary>The line was removed: its product is no longer sold here (unpublished, deleted, unpriced or now in another currency), or Stripe would refuse it (quantity, number of lines).</summary>
        public bool Removed => NewPrice is null;
    }

    /// <summary>The basket at current prices and what changed.</summary>
    public sealed record BasketRepricing(Basket Basket, IReadOnlyList<RepricedLine> Changes);

    /// <summary>Re-prices a basket from published content before checkout.</summary>
    public interface IStripeBasketPricingService
    {
        /// <summary>
        /// Brings every line to its current published price (<see cref="IStripeLineItemPricingService"/>), and drops lines whose
        /// product is unpublished, deleted, unpriced or now sold in another currency, whose quantity Stripe refuses (outside 1 to
        /// <see cref="Shared.Consts.MaxLineQuantity"/>), or beyond <see cref="Shared.Consts.MaxLineItems"/> lines. The given basket is
        /// updated in place and returned.
        /// </summary>
        Task<BasketRepricing> RepriceAsync(Basket basket);
    }
}
