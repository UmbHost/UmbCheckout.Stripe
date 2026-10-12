using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UmbCheckout.Core.Interfaces;
using UmbCheckout.Core.ManagementApi;
using UmbCheckout.Core.Models;
using UmbCheckout.Shared.Models;
using UmbCheckout.Shared.Notifications;
using UmbCheckout.Stripe.Interfaces;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;

namespace UmbCheckout.Stripe.Controllers.Surface
{
    public class StripeBasketController : SurfaceController
    {
        private readonly IBasketService _basketService;
        private readonly IStripeSessionService _sessionService;
        private readonly IStripeLineItemPricingService _pricingService;
        private readonly IStripeBasketPricingService _basketPricingService;
        private readonly ISessionService _umbCheckoutSessionService;
        private readonly ILogger<StripeBasketController> _logger;

        public StripeBasketController(IUmbracoContextAccessor umbracoContextAccessor, IUmbracoDatabaseFactory databaseFactory, ServiceContext services, AppCaches appCaches, IProfilingLogger profilingLogger, IPublishedUrlProvider publishedUrlProvider, IBasketService basketService, IStripeSessionService sessionService, IStripeLineItemPricingService pricingService, IStripeBasketPricingService basketPricingService, ISessionService umbCheckoutSessionService, ILogger<StripeBasketController> logger)
            : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            _basketService = basketService;
            _sessionService = sessionService;
            _pricingService = pricingService;
            _basketPricingService = basketPricingService;
            _umbCheckoutSessionService = umbCheckoutSessionService;
            _logger = logger;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(BasketAdd basketAdd, Guid? redirectGuid = null)
        {
            try
            {
                var product = UmbracoContext.Content?.GetById(basketAdd.Key);
                if (product == null)
                {
                    return RedirectToCurrentUmbracoPage();
                }

                var price = await _pricingService.GetPriceAsync(product);
                if (price == null)
                {
                    TempData[Shared.Consts.TempData.UmbCheckoutAddToBasketFailed] = basketAdd.Key;
                    return RedirectToCurrentUmbracoPage();
                }

                try
                {
                    await _basketService.Add(new LineItem
                    {
                        Key = product.Key,
                        Name = !string.IsNullOrEmpty(product.Name) ? product.Name : string.Empty,
                        Price = price.Price,
                        CurrencyCode = price.CurrencyCode,
                        Quantity = basketAdd.Quantity
                    });
                }
                catch (ArgumentException ex)
                {
                    _logger.LogWarning(ex, "Product {ProductKey} (quantity {Quantity}) was not added to the basket", product.Key, basketAdd.Quantity);
                    TempData[Shared.Consts.TempData.UmbCheckoutAddToBasketFailed] = basketAdd.Key;
                    return RedirectToCurrentUmbracoPage();
                }

                if (Cancelled())
                {
                    return redirectGuid.HasValue ? RedirectToUmbracoPage(redirectGuid.Value) : RedirectToCurrentUmbracoPage();
                }

                TempData[Shared.Consts.TempData.UmbCheckoutAddedToBasket] = basketAdd.Key;

                if (redirectGuid.HasValue)
                {
                    return RedirectToUmbracoPage(redirectGuid.Value);
                }

                return RedirectToCurrentUmbracoPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Adding {ProductKey} to the basket failed", basketAdd.Key);
                return StatusCode(500);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reduce(Guid key, Guid? redirectGuid = null)
        {
            try
            {
                await _basketService.Reduce(key);
                if (Cancelled())
                {
                    return redirectGuid.HasValue ? RedirectToUmbracoPage(redirectGuid.Value) : RedirectToCurrentUmbracoPage();
                }

                TempData[Shared.Consts.TempData.UmbCheckoutBasketReduced] = key;

                if (redirectGuid.HasValue)
                {
                    return RedirectToUmbracoPage(redirectGuid.Value);
                }

                return RedirectToCurrentUmbracoPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Reducing {ProductKey} in the basket failed", key);
                return StatusCode(500);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(Guid key, Guid? redirectGuid = null)
        {
            try
            {
                await _basketService.Remove(key);
                if (Cancelled())
                {
                    return redirectGuid.HasValue ? RedirectToUmbracoPage(redirectGuid.Value) : RedirectToCurrentUmbracoPage();
                }

                TempData[Shared.Consts.TempData.UmbCheckoutBasketRemoved] = key;

                if (redirectGuid.HasValue)
                {
                    return RedirectToUmbracoPage(redirectGuid.Value);
                }

                return RedirectToCurrentUmbracoPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Removing {ProductKey} from the basket failed", key);
                return StatusCode(500);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout()
        {
            try
            {
                // Checkout charges the stored basket: a site's Loaded hooks shape what shoppers see, never what is charged.
                Basket basket;
                using (UmbCheckoutHooks.SuppressLoaded())
                {
                    basket = await _basketService.Get();
                }

                if (!basket.LineItems.Any())
                {
                    TempData[Shared.Consts.TempData.UmbCheckoutEmptyBasket] = true;
                    return RedirectToCurrentUmbracoPage();
                }

                // Checkout charges current published prices; if anything changed the customer sees it before paying.
                var repricing = await _basketPricingService.RepriceAsync(basket);
                if (repricing.Changes.Count > 0)
                {
                    await _umbCheckoutSessionService.Update(repricing.Basket);
                    if (Cancelled())
                    {
                        return RedirectToCurrentUmbracoPage();
                    }

                    TempData[Shared.Consts.TempData.UmbCheckoutBasketRepriced] = string.Join(", ", repricing.Changes.Select(change => change.Name).Distinct());
                    return RedirectToCurrentUmbracoPage();
                }

                var stripeSession = await _sessionService.CreateSessionAsync(repricing.Basket, HttpContext?.RequestAborted ?? default);

                // The success page clears this basket only when it returns from this checkout session. Bookkeeping, not a
                // shopper's change, so a site's Saving handlers can't refuse it (or the paid basket would never clear).
                repricing.Basket.CheckoutSessionId = stripeSession.Id;
                using (UmbCheckoutHooks.SuppressSaving())
                {
                    await _umbCheckoutSessionService.Update(repricing.Basket);
                }

                return Redirect(stripeSession.Url);
            }
            catch (OperationCanceledException) when (HttpContext?.RequestAborted.IsCancellationRequested == true)
            {
                // The shopper left: nobody is waiting for a response, and it isn't an error.
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Checkout failed");
                return StatusCode(500);
            }
        }

        /// <summary>Whether a notification handler cancelled this request's basket change; if so its reason is kept for the shopper.</summary>
        private bool Cancelled()
        {
            if (UmbCheckoutCancellation.Message(HttpContext) is not { } message)
            {
                return false;
            }

            TempData[Shared.Consts.TempData.UmbCheckoutBasketChangeCancelled] = message;
            return true;
        }

        [HttpGet]
        public async Task<IActionResult> EmptyBasket()
        {
            try
            {
                await _basketService.Clear();
                Cancelled();
                return RedirectToCurrentUmbracoPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500);
            }
        }
    }
}
