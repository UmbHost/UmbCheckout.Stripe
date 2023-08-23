using Microsoft.AspNetCore.Mvc;
using UmbCheckout.Stripe.ViewModels;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace UmbCheckout.Stripe.ViewComponents
{
    /// <summary>
    /// A view component for the Basket link with total item count
    /// </summary>
    [ViewComponent(Name = "StripeAddToBasketButton")]
    public class StripeAddToBasketButtonViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(IPublishedContent product, string linkName = "Add to Basket", string? linkCssClass = null, Guid? returnGuid = null)
        {
            var model = new StripeAddToBasketButtonViewModel
            {
                Product = product,
                LinkCssClass = linkCssClass,
                LinkName = linkName,
                ReturnGuid = returnGuid
            };

            return View("~/Views/Partials/UmbCheckout/_StripeAddToBasketButton.cshtml", model);
        }
    }
}