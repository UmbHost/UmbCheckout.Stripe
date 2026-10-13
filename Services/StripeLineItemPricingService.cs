using System.Globalization;
using Microsoft.Extensions.Logging;
using UmbCheckout.Core.Interfaces;
using UmbCheckout.Stripe.Interfaces;
using UmbCheckout.Stripe.Money;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace UmbCheckout.Stripe.Services
{
    /// <inheritdoc />
    internal class StripeLineItemPricingService : IStripeLineItemPricingService
    {
        private readonly ICurrencyService _currencyService;
        private readonly IBasketContext _basketContext;
        private readonly ILogger<StripeLineItemPricingService> _logger;

        public StripeLineItemPricingService(ICurrencyService currencyService, IBasketContext basketContext, ILogger<StripeLineItemPricingService> logger)
        {
            _currencyService = currencyService;
            _basketContext = basketContext;
            _logger = logger;
        }

        /// <inheritdoc />
        public Task<LineItemPrice?> GetPriceAsync(IPublishedContent product) => GetPriceAsync(product, CancellationToken.None);

        /// <inheritdoc />
        public async Task<LineItemPrice?> GetPriceAsync(IPublishedContent product, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            // An empty Decimal field converts to 0m, so emptiness is checked before the value is read. A typed 0 is a free product.
            var property = product.GetProperty(Shared.Consts.PropertyAlias.PriceAlias);
            var price = property?.HasValue() == true ? ReadPrice(property.GetValue()) : null;
            if (price is not { } amount || amount < 0)
            {
                _logger.LogError("Product {ProductKey} has no valid price in {PropertyAlias}; it can't be sold", product.Key, Shared.Consts.PropertyAlias.PriceAlias);
                return null;
            }

            var currency = await _currencyService.GetCurrencyForContentAsync(product, cancellationToken);
            if (!StripeCurrency.IsSupported(currency))
            {
                _logger.LogError("Product {ProductKey} resolves to currency {CurrencyCode}, which Stripe does not support; check the site root's {PropertyAlias} or the Configuration currency. It can't be sold", product.Key, currency, Shared.Consts.PropertyAlias.CurrencyCode);
                return null;
            }

            // The basket shown on this page is in the page's currency: a product sold in another currency (e.g. from another
            // site or a shared product library) would go to a basket the customer never sees here.
            var pageCurrency = await _basketContext.GetCurrencyAsync(cancellationToken);
            if (pageCurrency != null && !string.Equals(pageCurrency.Trim(), currency, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Product {ProductKey} is sold in {CurrencyCode} but this page sells in {PageCurrencyCode}; it can't be sold on this page", product.Key, currency, pageCurrency);
                return null;
            }

            var rounded = StripeCurrency.Round(amount, currency);
            if (rounded != amount)
            {
                _logger.LogWarning("Product {ProductKey} price {Price} has more decimal places than {CurrencyCode} allows; {RoundedPrice} is used", product.Key, amount, currency, rounded);
            }

            return new LineItemPrice(rounded, currency.ToUpperInvariant());
        }

        /// <summary>Umbraco's Decimal editor yields decimal; other numeric editors are accepted, text only in invariant form.</summary>
        private static decimal? ReadPrice(object? value) => value switch
        {
            decimal d => d,
            int i => i,
            long l => l,
            double d when double.IsFinite(d) && Math.Abs(d) < 7.9e28 => (decimal)d,
            string s when decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var parsed) => parsed,
            _ => null
        };
    }
}
