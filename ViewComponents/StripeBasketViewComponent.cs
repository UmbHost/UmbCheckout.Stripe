using Microsoft.AspNetCore.Mvc;
using UmbCheckout.Core.Interfaces;
using UmbCheckout.Core.ViewModels;
using UmbCheckout.Stripe.ViewModels;

namespace UmbCheckout.Stripe.ViewComponents
{
    /// <summary>
    /// View component for the Stripe basket
    /// </summary>
    [ViewComponent(Name = "StripeBasket")]
    public class StripeBasketViewComponent : ViewComponent
    {
        private readonly IBasketService _basketService;

        public StripeBasketViewComponent(IBasketService basketService)
        {
            _basketService = basketService;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            string? productNameColumnText = "Name", 
            string? productPriceColumnText = "Price", 
            string? quantityColumnText = "Quantity", 
            string? reduceColumnText = "Reduce", 
            string? increaseColumnText = "Increase",
            string? removeColumnText = "Remove", 
            string? tableCssClass = null,
            string? reduceButtonCssClass = null,
            string? reduceButtonText = "-",
            string? increaseButtonCssClass = null,
            string? increaseButtonText = "+",
            string? removeButtonCssClass = null,
            string? removeButtonText = "Remove",
            string? emptyBasketText  = "Your basket is empty!",
            string? subTotalText = "Sub Total:",
            string? formatCurrency = "GBP",
            string? subTotalInformationText = "Coupons, Shipping and Tax are calculated on the next checkout step",
            string? checkoutButtonCssClass = null,
            string? productNameAlias = null,
            string checkoutButtonText = "Checkout")
        {
            var basket = await _basketService.Get();

            var model = new StripeBasketViewModel
            {
                TableCssClass = tableCssClass,
                CheckoutButtonCssClass = checkoutButtonCssClass,
                CheckoutButtonText = checkoutButtonText,
                ProductNameColumnText = productNameColumnText,
                ProductPriceColumnText = productPriceColumnText,
                QuantityColumnText = quantityColumnText,
                RemoveColumnText = removeColumnText,
                ReduceColumnText = reduceColumnText,
                IncreaseColumnText = increaseColumnText,
                ReduceButtonCssClass = reduceButtonCssClass,
                ReduceButtonText = reduceButtonText,
                IncreaseButtonCssClass = increaseButtonCssClass,
                IncreaseButtonText = increaseButtonText,
                RemoveButtonCssClass = removeButtonCssClass,
                RemoveButtonText = removeButtonText,
                EmptyBasketText = emptyBasketText,
                SubTotalText = subTotalText,
                FormatCurrency = formatCurrency,
                SubtotalInformationText = subTotalInformationText,
                Basket = basket,
                SubTotal = await _basketService.SubTotal(),
                TotalItems = await _basketService.TotalItems(),
                ProductNameAlias = productNameAlias
            };

            return View("~/Views/Partials/UmbCheckout/_StripeBasket.cshtml", model);
        }
    }
}
