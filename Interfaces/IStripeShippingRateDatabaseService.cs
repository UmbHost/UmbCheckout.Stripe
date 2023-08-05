using UmbCheckout.Stripe.Models;

namespace UmbCheckout.Stripe.Interfaces
{
    public interface IStripeShippingRateDatabaseService
    {
        Task<IEnumerable<ShippingRate>> GetShippingRates();

        Task<ShippingRate?> GetShippingRate(Guid key);

        Task<ShippingRate?> GetShippingRate(string value);

        Task<ShippingRate?> CreateShippingRate(ShippingRate shippingRate);

        Task<ShippingRate?> UpdateShippingRate(ShippingRate shippingRate);

        Task<bool> DeleteShippingRate(Guid key);
    }
}
