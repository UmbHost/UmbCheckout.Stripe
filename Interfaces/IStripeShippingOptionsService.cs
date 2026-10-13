using Stripe.Checkout;
using UmbCheckout.Shared.Models;

namespace UmbCheckout.Stripe.Interfaces;


/// <summary>
/// Interface for calculating the shipping rates to be applied to a basket. <see cref="IStripeSessionService.CreateSession"/> 
/// </summary>
public interface IStripeShippingOptionsService
{
    [Obsolete("Use GetShippingOptions(Basket, CancellationToken) (the overload with a CancellationToken). Scheduled for removal in 17.1.")]
    Task<List<SessionShippingOptionOptions>> GetShippingOptions(Basket basket);

    /// <summary>As <see cref="GetShippingOptions(Basket)"/>, stopping when <paramref name="cancellationToken"/> is cancelled.</summary>
    /// <remarks>Implementations written before the token overloads existed ignore the token.</remarks>
#pragma warning disable CS0618 // forwards to the old overload so implementations written before the token overload keep working
    Task<List<SessionShippingOptionOptions>> GetShippingOptions(Basket basket, CancellationToken cancellationToken) => GetShippingOptions(basket);
#pragma warning restore CS0618
}