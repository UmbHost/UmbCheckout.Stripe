using Microsoft.Extensions.Logging;
using Stripe;
using Stripe.Checkout;
using UmbCheckout.Shared.Extensions;
using UmbCheckout.Shared.Models;
using UmbCheckout.Stripe.Interfaces;

namespace UmbCheckout.Stripe.Services;

/// <summary>
/// Default implementation of the <see cref="IStripeShippingOptionsService" />
/// </summary>
public class StripeDefaultShippingOptionsService : IStripeShippingOptionsService
{
    /// <summary>Stripe Checkout accepts at most five shipping options.</summary>
    private const int MaxShippingOptions = 5;

    private readonly IStripeShippingRateDatabaseService _stripeDatabaseService;
    private readonly IStripeShippingRateApiService _shippingRateApiService;
    private readonly ILogger<StripeDefaultShippingOptionsService> _logger;

    public StripeDefaultShippingOptionsService(IStripeShippingRateDatabaseService stripeDatabaseService, IStripeShippingRateApiService shippingRateApiService, ILogger<StripeDefaultShippingOptionsService> logger)
    {
        _stripeDatabaseService = stripeDatabaseService;
        _shippingRateApiService = shippingRateApiService;
        _logger = logger;
    }

    /// <summary>The configured shipping rates that are active and charge in the basket's currency (at most five).</summary>
    [Obsolete("Use GetShippingOptions(Basket, CancellationToken) (the overload with a CancellationToken). Scheduled for removal in the next major version.")]
    public Task<List<SessionShippingOptionOptions>> GetShippingOptions(Basket basket) => GetShippingOptions(basket, CancellationToken.None);

    /// <summary>The configured shipping rates that are active and charge in the basket's currency (at most five), stopping when <paramref name="cancellationToken"/> is cancelled.</summary>
    public async Task<List<SessionShippingOptionOptions>> GetShippingOptions(Basket basket, CancellationToken cancellationToken)
    {
        var currency = basket.CurrencyCode ?? basket.LineItems.SingleCurrency();
        var options = new List<SessionShippingOptionOptions>();

        foreach (var configured in await _stripeDatabaseService.GetShippingRatesAsync(cancellationToken))
        {
            ShippingRate rate;
            try
            {
                rate = await _shippingRateApiService.GetShippingRate(configured.Value, cancellationToken);
            }
            catch (StripeException ex)
            {
                _logger.LogWarning(ex, "Shipping rate {ShippingRateId} could not be read from Stripe; it is not offered", configured.Value);
                continue;
            }

            if (!rate.Active || !ChargesIn(rate, currency))
            {
                continue;
            }

            if (options.Count == MaxShippingOptions)
            {
                _logger.LogWarning("More than {MaxShippingOptions} shipping rates charge in {CurrencyCode}; only the first {MaxShippingOptions} are offered", MaxShippingOptions, currency, MaxShippingOptions);
                break;
            }

            options.Add(new SessionShippingOptionOptions { ShippingRate = rate.Id });
        }

        return options;
    }

    private static bool ChargesIn(ShippingRate rate, string? currency)
        => currency != null
           && (string.Equals(rate.FixedAmount?.Currency, currency, StringComparison.OrdinalIgnoreCase)
               || rate.FixedAmount?.CurrencyOptions?.Keys.Any(key => string.Equals(key, currency, StringComparison.OrdinalIgnoreCase)) == true);
}
