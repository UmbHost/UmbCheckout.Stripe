using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using UmbCheckout.Stripe.Models;

namespace UmbCheckout.Stripe.Notifications
{
    /// <summary>
    /// Published after shipping rates are read (list or single), before they are returned; change, remove or reorder <see cref="Items"/>.
    /// A single read returns nothing when its rate is removed. Keep handlers cheap: it runs on every read.
    /// </summary>
    public sealed class OnShippingRatesLoadedNotification(List<ShippingRate> items, EventMessages messages)
        : ObjectNotification<List<ShippingRate>>(items, messages)
    {
        /// <summary>The shipping rates about to be returned.</summary>
        public List<ShippingRate> Items => Target;
    }
}
