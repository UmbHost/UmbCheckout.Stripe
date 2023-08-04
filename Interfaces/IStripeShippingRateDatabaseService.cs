using UmbCheckout.Stripe.Models;

namespace UmbCheckout.Stripe.Interfaces
{
    public interface IStripeShippingRateDatabaseService
    {
        Task<IEnumerable<ShippingRate>> GetShippingRates();

        Task<ShippingRate?> GetShippingRate(Guid key);

        Task<ShippingRate?> UpdateShippingRate(ShippingRate shippingRate);

        Task<bool> DeleteShippingRate(Guid key);
    }
}
