using Microsoft.AspNetCore.Mvc;
using UmbCheckout.Core.Interfaces;
using UmbCheckout.Core.ViewModels;
using UmbCheckout.Shared.Extensions;
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
        private readonly ICurrencyService _currencyService;

        public StripeBasketViewComponent(IBasketService basketService, ICurrencyService currencyService)
        {
            _basketService = basketService;
            _currencyService = currencyService;
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
            string? emptyBasketButtonText = "Empty basket",
            string? subTotalText = "Sub Total:",
            Guid? currencyNodeKey = null,
            string? subTotalInformationText = "Coupons, Shipping and Tax are calculated on the next checkout step",
            string? checkoutButtonCssClass = null,
            string? productNameAlias = null,
            string? productAnchorLinkCssClass = null,
            string checkoutButtonText = "Checkout",
            string addToBasketFailedText = "That item could not be added to your basket.",
            string pricesChangedText = "Your basket was updated to current prices before checkout; please check it and continue:")
        {
            var cancellationToken = HttpContext?.RequestAborted ?? CancellationToken.None;
            var basket = await _basketService.Get(cancellationToken);

            // The subtotal is shown in the currency the basket is charged in; the configured chain only applies to an empty basket.
            var currencyCode = basket.CurrencyCode ?? basket.LineItems.SingleCurrency() ?? await _currencyService.GetCurrencyAsync(currencyNodeKey, cancellationToken);

            var notices = new List<string>();
            if (TempData[Shared.Consts.TempData.UmbCheckoutAddToBasketFailed] != null)
            {
                notices.Add(addToBasketFailedText);
            }

            if (TempData[Shared.Consts.TempData.UmbCheckoutBasketRepriced] is string changedProducts)
            {
                notices.Add($"{pricesChangedText} {changedProducts}");
            }

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
                EmptyBasketButtonText = emptyBasketButtonText,
                SubTotalText = subTotalText,
                FormatCurrency = currencyCode,
                SubtotalInformationText = subTotalInformationText,
                Basket = basket,
                Notices = notices,
                SubTotal = await _basketService.SubTotal(cancellationToken),
                TotalItems = await _basketService.TotalItems(cancellationToken),
                ProductNameAlias = productNameAlias,
                ProductAnchorLinkCssClass = productAnchorLinkCssClass
            };

            return View("~/Views/Partials/UmbCheckout/_StripeBasket.cshtml", model);
        }
    }
}
