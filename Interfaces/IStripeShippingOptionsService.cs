using Stripe.Checkout;
using UmbCheckout.Shared.Models;

namespace UmbCheckout.Stripe.Interfaces;


/// <summary>
/// Interface for calculating the shipping rates to be applied to a basket. <see cref="IStripeSessionService.CreateSession"/> 
/// </summary>
public interface IStripeShippingOptionsService
{
    Task<List<SessionShippingOptionOptions>> GetShippingOptions(Basket basket);

    /// <summary>The shipping options for <paramref name="basket"/>, stopping when <paramref name="cancellationToken"/> is cancelled.</summary>
    /// <remarks>Implementations written before this overload existed ignore the token.</remarks>
    Task<List<SessionShippingOptionOptions>> GetShippingOptions(Basket basket, CancellationToken cancellationToken) => GetShippingOptions(basket);
}