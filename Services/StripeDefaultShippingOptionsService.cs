using Stripe.Checkout;
using UmbCheckout.Shared.Models;
using UmbCheckout.Stripe.Interfaces;

namespace UmbCheckout.Stripe.Services;

/// <summary>
/// Default implementation of the <see cref="IStripeShippingOptionsService" />
/// </summary>
public class StripeDefaultShippingOptionsService : IStripeShippingOptionsService
{
    private readonly IStripeShippingRateDatabaseService _stripeDatabaseService;

    public StripeDefaultShippingOptionsService(IStripeShippingRateDatabaseService stripeDatabaseService)
    {
        _stripeDatabaseService = stripeDatabaseService;
    }

    public async Task<List<SessionShippingOptionOptions>> GetShippingOptions(Basket basket)
    {
        var shippingRates = await _stripeDatabaseService.GetShippingRates();
        var shippingRatesList = shippingRates.ToList();

        if (shippingRatesList.Any())
        {
            var shippingRateOptions = shippingRatesList.Select(shippingRate =>
                new SessionShippingOptionOptions { ShippingRate = shippingRate.Value }).ToList();

            return shippingRateOptions;
        }

        return new List<SessionShippingOptionOptions>();
    }
}