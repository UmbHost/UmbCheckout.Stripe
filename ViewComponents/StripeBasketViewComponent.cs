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

        public async Task<IViewComponentResult> InvokeAsync(string? tableCssClass = null)
        {
            var basket = await _basketService.Get();

            var model = new StripeBasketViewModel
            {
                TableCssClass = tableCssClass,
                Basket = basket,
                SubTotal = await _basketService.SubTotal(),
                TotalItems = await _basketService.TotalItems()
            };

            return View("~/Views/Partials/UmbCheckout/_StripeBasket.cshtml", model);
        }
    }
}
