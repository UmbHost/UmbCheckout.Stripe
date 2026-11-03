using Stripe.Checkout;
using UmbCheckout.Shared.Models;

namespace UmbCheckout.Stripe.Money
{
    /// <summary>
    /// Turns basket line items into Stripe Checkout price data: one currency per session, amounts in the currency's minor unit.
    /// </summary>
    public static class StripeLineItemPrice
    {
        /// <summary>The basket's single currency as Stripe expects it (lowercase).</summary>
        /// <exception cref="InvalidOperationException">The basket is empty or mixes currencies (a Checkout session takes one).</exception>
        /// <exception cref="ArgumentException">The currency is not one Stripe supports (including the empty code of a basket saved before the upgrade).</exception>
        public static string BasketCurrency(IEnumerable<LineItem> lineItems)
        {
            var codes = lineItems.Select(lineItem => lineItem.CurrencyCode?.Trim() ?? string.Empty).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (codes.Count != 1)
            {
                throw new InvalidOperationException(codes.Count == 0
                    ? "The basket has no line items."
                    : $"The basket mixes currencies ({string.Join(", ", codes)}); a Stripe Checkout session takes one currency.");
            }

            return StripeCurrency.ApiCode(codes[0]);
        }

        /// <summary>The Stripe unit amount for <paramref name="price"/>, rounded by UmbCheckout's single policy.</summary>
        public static long UnitAmount(decimal price, string currency)
            => StripeCurrency.ToMinorUnits(StripeCurrency.Round(price, currency), currency);

        /// <summary>Price data for one Checkout line item.</summary>
        public static SessionLineItemPriceDataOptions PriceData(LineItem lineItem, string currency, string productName) => new()
        {
            Currency = currency,
            ProductData = new SessionLineItemPriceDataProductDataOptions
            {
                Name = !string.IsNullOrEmpty(lineItem.Name) ? lineItem.Name : productName
            },
            UnitAmount = UnitAmount(lineItem.Price, currency)
        };
    }
}
