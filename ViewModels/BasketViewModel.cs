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
    }
}
