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
        public async Task<IViewComponentResult> InvokeAsync(IPublishedContent product, string? quantityLabel = "Quantity", bool showQuantity = true, string? inputCssClass = null, string? selectCssClass = null, string? labelCssClass = null, string? formGroupSpacerClass = null, string? variantSelectLabel = null, string buttonText = "Add to Basket", string? buttonCssClass = null, Guid? returnGuid = null, string? productNameAlias = null)
        {
            var model = new StripeAddToBasketButtonViewModel
            {
                ShowQuantity = showQuantity,
                QuantityLabel = quantityLabel,
                Product = product,
                ButtonCssClass = buttonCssClass,
                ButtonText = buttonText,
                ReturnGuid = returnGuid,
                ProductNameAlias = productNameAlias,
                InputCssClass = inputCssClass,
                SelectCssClass = selectCssClass,
                LabelCssClass = labelCssClass,
                FormGroupSpacerClass = formGroupSpacerClass,
                VariantSelectLabel = variantSelectLabel
            };

            return View("~/Views/Partials/UmbCheckout/_StripeAddToBasketButton.cshtml", model);
        }
    }
}