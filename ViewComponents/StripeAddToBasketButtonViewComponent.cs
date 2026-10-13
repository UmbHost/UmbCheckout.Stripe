using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using UmbCheckout.Core.Interfaces;
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
        private readonly ICurrencyService _currencyService;
        private readonly ILogger<StripeAddToBasketButtonViewComponent> _logger;

        /// <summary>1 once a currencyNodeKey in another currency than the charged one has been logged in this process.</summary>
        private static int _currencyNodeKeyWarned;

        /// <summary>Kept for code that constructs the view component itself; it logs nothing.</summary>
        [Obsolete("Use the constructor that takes an ILogger. Scheduled for removal in 17.1.")]
        public StripeAddToBasketButtonViewComponent(ICurrencyService currencyService)
            : this(currencyService, NullLogger<StripeAddToBasketButtonViewComponent>.Instance)
        {
        }

        [ActivatorUtilitiesConstructor]
        public StripeAddToBasketButtonViewComponent(ICurrencyService currencyService, ILogger<StripeAddToBasketButtonViewComponent> logger)
        {
            _currencyService = currencyService;
            _logger = logger;
        }

        /// <summary>Lets tests see the once-per-process warning again.</summary>
        internal static void ResetCurrencyNodeKeyWarning() => Interlocked.Exchange(ref _currencyNodeKeyWarned, 0);

        /// <param name="currencyNodeKey">No longer decides the currency: the button shows the currency checkout charges (the product's site root, then Configuration).
        /// When it names another currency, a warning is logged once per process.</param>
        public async Task<IViewComponentResult> InvokeAsync(IPublishedContent product, string? quantityLabel = "Quantity", bool showQuantity = true, string? inputCssClass = null, string? selectCssClass = null, string? labelCssClass = null, string? formGroupSpacerClass = null, string? variantSelectLabel = null, string buttonText = "Add to Basket", string? buttonCssClass = null, Guid? returnGuid = null, string? productNameAlias = null, Guid? currencyNodeKey = null)
        {
            // The currency checkout charges (the product's site root, then Configuration); a null product renders nothing, as before.
            var cancellationToken = HttpContext?.RequestAborted ?? CancellationToken.None;
            var currencyCode = product is null ? null : await _currencyService.GetCurrencyForContentAsync(product, cancellationToken);

            if (currencyNodeKey is not null && currencyCode is not null && Volatile.Read(ref _currencyNodeKeyWarned) == 0)
            {
                await WarnIfCurrencyNodeKeyDiffersAsync(currencyNodeKey.Value, currencyCode, cancellationToken);
            }

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
                VariantSelectLabel = variantSelectLabel,
                CurrencyCode = currencyCode
            };

            return View("~/Views/Partials/UmbCheckout/_StripeAddToBasketButton.cshtml", model);
        }

        /// <summary>
        /// Sites upgraded from an earlier 13.x may pass a currencyNodeKey to show another currency; checkout now charges the product's own,
        /// so say so once. Never breaks the render.
        /// </summary>
        private async Task WarnIfCurrencyNodeKeyDiffersAsync(Guid currencyNodeKey, string chargedCurrency, CancellationToken cancellationToken)
        {
            string? nodeCurrency;
            try
            {
                nodeCurrency = (await _currencyService.GetCurrencyAsync(currencyNodeKey, cancellationToken))?.Trim();
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                // GetCurrencyAsync logs its own failure; the button still renders in the charged currency.
                return;
            }

            if (!string.IsNullOrEmpty(nodeCurrency)
                && !string.Equals(nodeCurrency, chargedCurrency, StringComparison.OrdinalIgnoreCase)
                && Interlocked.Exchange(ref _currencyNodeKeyWarned, 1) == 0)
            {
                _logger.LogWarning("The add-to-basket button's currencyNodeKey names {NodeCurrency}, but products are charged in {ChargedCurrency} (the site root's currency, then UmbCheckout Configuration). currencyNodeKey no longer sets the currency; this is logged once per process", nodeCurrency.ToUpperInvariant(), chargedCurrency.ToUpperInvariant());
            }
        }
    }
}
