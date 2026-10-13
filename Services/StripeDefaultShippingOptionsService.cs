using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Stripe;
using Stripe.Checkout;
using UmbCheckout.Shared.Extensions;
using UmbCheckout.Shared.Models;
using UmbCheckout.Stripe.Interfaces;
using Umbraco.Cms.Web.Common.DependencyInjection;

namespace UmbCheckout.Stripe.Services;

/// <summary>
/// Default implementation of the <see cref="IStripeShippingOptionsService" />: the configured shipping rates that are active and
/// charge in the basket's currency (at most five, Stripe Checkout's limit)
/// </summary>
public class StripeDefaultShippingOptionsService : IStripeShippingOptionsService
{
    /// <summary>Stripe Checkout accepts at most five shipping options.</summary>
    private const int MaxShippingOptions = 5;

    private readonly IStripeShippingRateDatabaseService _stripeDatabaseService;
    private readonly IStripeShippingRateApiService _shippingRateApiService;
    private readonly ILogger<StripeDefaultShippingOptionsService> _logger;

    [Obsolete("Use the constructor that takes an IStripeShippingRateApiService and a logger. This one resolves them from the application's services.")]
    public StripeDefaultShippingOptionsService(IStripeShippingRateDatabaseService stripeDatabaseService)
        : this(stripeDatabaseService, StaticServiceProvider.Instance.GetRequiredService<IStripeShippingRateApiService>(), StaticServiceProvider.Instance.GetRequiredService<ILogger<StripeDefaultShippingOptionsService>>())
    {
    }

    [ActivatorUtilitiesConstructor]
    public StripeDefaultShippingOptionsService(IStripeShippingRateDatabaseService stripeDatabaseService, IStripeShippingRateApiService shippingRateApiService, ILogger<StripeDefaultShippingOptionsService> logger)
    {
        _stripeDatabaseService = stripeDatabaseService;
        _shippingRateApiService = shippingRateApiService;
        _logger = logger;
    }

    /// <summary>The configured shipping rates that are active and charge in the basket's currency (at most five).</summary>
    public async Task<List<SessionShippingOptionOptions>> GetShippingOptions(Basket basket)
    {
        var currency = basket.CurrencyCode ?? basket.LineItems.SingleCurrency();
        var options = new List<SessionShippingOptionOptions>();

        foreach (var configured in await _stripeDatabaseService.GetShippingRates())
        {
            ShippingRate rate;
            try
            {
                rate = await _shippingRateApiService.GetShippingRate(configured.Value);
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
                _logger.LogWarning("More than {MaxShippingOptions} shipping rates charge in {CurrencyCode}; only the first {OfferedCount} are offered", MaxShippingOptions, currency, MaxShippingOptions);
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
