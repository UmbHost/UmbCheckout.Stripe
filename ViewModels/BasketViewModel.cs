using UmbCheckout.Shared.Models;

namespace UmbCheckout.Stripe.ViewModels
{
    /// <summary>
    /// View model for the Basket View Component
    /// </summary>
    public class StripeBasketViewModel
    {
        public string? TableCssClass { get; set; } = null;

        public Basket Basket { get; set; } = new Basket();

        public decimal SubTotal { get; set; }
        public long TotalItems { get; set; }

        public string ProductNameColumnText { get; set; } = "Name";

        public string ProductPriceColumnText { get; set; } = "Price";

        public string QuantityColumnText { get; set; } = "Quantity";

        public string ReduceColumnText { get; set; } = "Reduce";

        public string IncreaseColumnText { get; set; } = "Increase";

        public string RemoveColumnText { get; set; } = "Remove";

        public string? CheckoutButtonCssClass { get; set; } = null;

        public string CheckoutButtonText { get; set; } = "Checkout";

        public string? ReduceButtonCssClass { get; set; } = null;

        public string ReduceButtonText { get; set; } = "-";

        public string? IncreaseButtonCssClass { get; set; } = null;

        public string IncreaseButtonText { get; set; } = "+";

        public string? RemoveButtonCssClass { get; set; } = null;

        public string RemoveButtonText { get; set; } = "Remove";

        public string EmptyBasketText { get; set; } = "Your basket is empty!";

        public string SubTotalText { get; set; } = "Sub Total:";

        public string FormatCurrency { get; set; } = "GBP";

        public string SubtotalInformationText { get; set; } = "Coupons, Shipping and Tax are calculated on the next checkout step";
    }
}
