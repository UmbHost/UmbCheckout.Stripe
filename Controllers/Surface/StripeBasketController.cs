using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UmbCheckout.Core.Interfaces;
using UmbCheckout.Core.Models;
using UmbCheckout.Shared.Extensions;
using UmbCheckout.Shared.Models;
using UmbCheckout.Stripe.Interfaces;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Common.DependencyInjection;
using Umbraco.Cms.Web.Website.Controllers;

namespace UmbCheckout.Stripe.Controllers.Surface
{
    public class StripeBasketController : SurfaceController
    {
        private readonly IBasketService _basketService;
        private readonly IStripeSessionService _sessionService;
        private readonly IStripeLineItemPricingService _pricingService;
        private readonly ILogger<StripeBasketController> _logger;

        [Obsolete("Use the constructor that takes an IStripeLineItemPricingService. This one resolves it from the application's services.")]
        public StripeBasketController(IUmbracoContextAccessor umbracoContextAccessor, IUmbracoDatabaseFactory databaseFactory, ServiceContext services, AppCaches appCaches, IProfilingLogger profilingLogger, IPublishedUrlProvider publishedUrlProvider, IBasketService basketService, IStripeSessionService sessionService, ILogger<StripeBasketController> logger)
            : this(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider, basketService, sessionService, StaticServiceProvider.Instance.GetRequiredService<IStripeLineItemPricingService>(), logger)
        {
        }

        [ActivatorUtilitiesConstructor]
        public StripeBasketController(IUmbracoContextAccessor umbracoContextAccessor, IUmbracoDatabaseFactory databaseFactory, ServiceContext services, AppCaches appCaches, IProfilingLogger profilingLogger, IPublishedUrlProvider publishedUrlProvider, IBasketService basketService, IStripeSessionService sessionService, IStripeLineItemPricingService pricingService, ILogger<StripeBasketController> logger)
            : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            _basketService = basketService;
            _sessionService = sessionService;
            _pricingService = pricingService;
            _logger = logger;
        }

        /// <summary>
        /// Adds a product to the basket. Its price and currency are set by the server (<see cref="IStripeLineItemPricingService"/>);
        /// the posted <see cref="BasketAdd.Key"/> and <see cref="BasketAdd.Quantity"/> are the only values read from the form.
        /// A product that can't be priced, or is priced in another currency than the basket's lines, is refused with
        /// TempData <see cref="Shared.Consts.TempData.UmbCheckoutAddToBasketFailed"/>.
        /// </summary>
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
                    _logger.LogWarning("Product {ProductKey} could not be priced; it was not added to the basket", product.Key);
                    TempData[Shared.Consts.TempData.UmbCheckoutAddToBasketFailed] = basketAdd.Key;
                    return RedirectToCurrentUmbracoPage();
                }

                // A Checkout session takes one currency: refuse a line that would mix currencies (e.g. a basket saved before the upgrade).
                var basket = await _basketService.Get();
                if (!basket.LineItems.AcceptsCurrency(price.CurrencyCode))
                {
                    _logger.LogWarning("Product {ProductKey} is priced in {CurrencyCode} but the basket holds other currencies ({BasketCurrencies}); it was not added", product.Key, price.CurrencyCode, string.Join(", ", basket.LineItems.Select(lineItem => lineItem.CurrencyCode).Distinct()));
                    TempData[Shared.Consts.TempData.UmbCheckoutAddToBasketFailed] = basketAdd.Key;
                    return RedirectToCurrentUmbracoPage();
                }

                await _basketService.Add(new LineItem
                {
                    Key = product.Key,
                    Name = !string.IsNullOrEmpty(product.Name) ? product.Name : string.Empty,
                    Price = price.Price,
                    CurrencyCode = price.CurrencyCode,
                    Quantity = basketAdd.Quantity
                });

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
        public IActionResult Reduce(Guid key, Guid? redirectGuid = null)
        {
            try
            {
                _basketService.Reduce(key);

                TempData[Shared.Consts.TempData.UmbCheckoutBasketReduced] = key;

                if (redirectGuid.HasValue)
                {
                    return RedirectToUmbracoPage(redirectGuid.Value);
                }

                return RedirectToCurrentUmbracoPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(Guid key, Guid? redirectGuid = null)
        {
            try
            {
                _basketService.Remove(key);

                TempData[Shared.Consts.TempData.UmbCheckoutBasketRemoved] = key;

                if (redirectGuid.HasValue)
                {
                    return RedirectToUmbracoPage(redirectGuid.Value);
                }

                return RedirectToCurrentUmbracoPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout()
        {
            try
            {
                var basket = await _basketService.Get();
                if (!basket.LineItems.Any())
                {
                    TempData[Shared.Consts.TempData.UmbCheckoutEmptyBasket] = true;
                    return RedirectToCurrentUmbracoPage();
                }

                var stripeSession = await _sessionService.CreateSessionAsync(basket);
                return Redirect(stripeSession.Url);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EmptyBasket()
        {
            try
            {
                await _basketService.Clear();
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
