using UmbCheckout.Shared.Persistence;
using UmbCheckout.Stripe.Notifications;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Events;

namespace UmbCheckout.Stripe.Cache
{
    /// <summary>
    /// Clears the cached Stripe settings once a save has committed. Umbraco publishes Saved when the outermost scope completes, so this
    /// also covers a save inside someone else's scope (a uSync import), where the service's own clear runs before the commit.
    /// </summary>
    internal sealed class UmbCheckoutStripeSettingsSavedCacheHandler(SettingsCache<Pocos.UmbCheckoutStripeSettings> cache, DistributedCache? distributedCache = null)
        : INotificationHandler<OnStripeSettingsSavedNotification>
    {
        public void Handle(OnStripeSettingsSavedNotification notification)
        {
            // This server's copy now, every other server's through Umbraco's distributed cache (load balancing).
            cache.Clear();
            distributedCache?.RefreshAll(UmbCheckoutStripeSettingsCacheRefresher.UniqueId);
        }
    }
}
