using UmbCheckout.Shared.Persistence;
using UmbCheckout.Stripe.Notifications;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Events;
using UmbCheckoutStripeSettings = UmbCheckout.Stripe.Pocos.UmbCheckoutStripeSettings;

namespace UmbCheckout.Stripe.Cache
{
    /// <summary>
    /// Clears the cached Stripe settings once a save has committed. Umbraco publishes the scoped Saved notification when the outermost
    /// scope completes (and drops it on a rollback), so this also covers a save inside someone else's scope (a uSync import), where the
    /// service's own clear runs before the commit.
    /// </summary>
    internal sealed class UmbCheckoutStripeSettingsSavedCacheHandler : INotificationHandler<OnStripeSettingsSavedNotification>
    {
        private readonly SettingsCache<UmbCheckoutStripeSettings> _cache;
        private readonly DistributedCache? _distributedCache;

        public UmbCheckoutStripeSettingsSavedCacheHandler(SettingsCache<UmbCheckoutStripeSettings> cache, DistributedCache? distributedCache = null)
        {
            _cache = cache;
            _distributedCache = distributedCache;
        }

        public void Handle(OnStripeSettingsSavedNotification notification)
        {
            // This server's copy now, every other server's through Umbraco's distributed cache (load balancing).
            _cache.Clear();
            _distributedCache?.RefreshAll(UmbCheckoutStripeSettingsCacheRefresher.UniqueId);
        }
    }
}
