using Umbraco.Cms.Core.Notifications;
using ShippingRate = UmbCheckout.Stripe.Models.ShippingRate;

namespace UmbCheckout.Stripe.Notifications
{
    public class OnShippingRateSavedNotification : INotification
    {
        public ShippingRate ShippingRate { get; set; }

        public OnShippingRateSavedNotification(ShippingRate shippingRate)
        {
            ShippingRate = shippingRate;
        }
    }
}
