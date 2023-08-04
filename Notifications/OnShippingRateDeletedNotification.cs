using Umbraco.Cms.Core.Notifications;
using ShippingRate = UmbCheckout.Stripe.Models.ShippingRate;

namespace UmbCheckout.Stripe.Notifications
{
    public class OnShippingRateDeletedNotification : INotification
    {
        public ShippingRate ShippingRate { get; set; }

        public OnShippingRateDeletedNotification(ShippingRate shippingRate)
        {
            ShippingRate = shippingRate;
        }
    }
}
