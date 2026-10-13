using Microsoft.Extensions.Logging;
using UmbCheckout.Shared.Models;
using UmbCheckout.Stripe.Interfaces;
using Umbraco.Cms.Core.PublishedCache;

namespace UmbCheckout.Stripe.Services
{
    /// <inheritdoc />
    internal class StripeBasketPricingService : IStripeBasketPricingService
    {
        private readonly IPublishedContentCache _contentCache;
        private readonly IStripeLineItemPricingService _pricingService;
        private readonly ILogger<StripeBasketPricingService> _logger;

        public StripeBasketPricingService(IPublishedContentCache contentCache, IStripeLineItemPricingService pricingService, ILogger<StripeBasketPricingService> logger)
        {
            _contentCache = contentCache;
            _pricingService = pricingService;
            _logger = logger;
        }

        /// <inheritdoc />
        public Task<BasketRepricing> RepriceAsync(Basket basket) => RepriceAsync(basket, CancellationToken.None);

        /// <inheritdoc />
        public async Task<BasketRepricing> RepriceAsync(Basket basket, CancellationToken cancellationToken)
        {
            var lines = new List<LineItem>();
            var changes = new List<RepricedLine>();

            foreach (var line in basket.LineItems)
            {
                // Stripe refuses a quantity outside 1 to 999,999, and more than 100 line items; legacy baskets may hold either.
                if (line.Quantity < 1 || line.Quantity > Shared.Consts.MaxLineQuantity || lines.Count >= Shared.Consts.MaxLineItems)
                {
                    changes.Add(new RepricedLine(line.Key, line.Name, line.Price, null));
                    continue;
                }

                var product = await _contentCache.GetByIdAsync(line.Key);
                var current = product == null ? null : await _pricingService.GetPriceAsync(product, cancellationToken);
                var currency = basket.CurrencyCode ?? line.CurrencyCode;

                if (current == null || !string.Equals(current.CurrencyCode, currency, StringComparison.OrdinalIgnoreCase))
                {
                    changes.Add(new RepricedLine(line.Key, line.Name, line.Price, null));
                    continue;
                }

                if (current.Price != line.Price)
                {
                    changes.Add(new RepricedLine(line.Key, line.Name, line.Price, current.Price));
                    line.Price = current.Price;
                }

                lines.Add(line);
            }

            if (changes.Count > 0)
            {
                _logger.LogInformation("Basket {BasketId} repriced at checkout: {ChangedCount} price changes, {RemovedCount} lines removed", basket.Id, changes.Count(c => !c.Removed), changes.Count(c => c.Removed));
            }

            basket.LineItems = lines;
            return new BasketRepricing(basket, changes);
        }
    }
}
