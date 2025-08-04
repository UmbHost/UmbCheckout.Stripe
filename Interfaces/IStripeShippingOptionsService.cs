using Stripe.Checkout;
using UmbCheckout.Shared.Models;

namespace UmbCheckout.Stripe.Interfaces;


/// <summary>
/// Interface for calculating the shipping rates to be applied to a basket. <see cref="IStripeSessionService.CreateSession"/> 
/// </summary>
public interface IStripeShippingOptionsService
{
    Task<List<SessionShippingOptionOptions>> GetShippingOptions(Basket basket);
}