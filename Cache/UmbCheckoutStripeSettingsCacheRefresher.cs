using UmbCheckout.Shared.Persistence;
using Umbraco.Cms.Core.Cache;
using UmbCheckoutStripeSettings = UmbCheckout.Stripe.Pocos.UmbCheckoutStripeSettings;

namespace UmbCheckout.Stripe.Cache
{
    /// <summary>
    /// Clears the cached Stripe settings on every server when they are saved on one (Umbraco's distributed cache calls it).
    /// </summary>
    internal sealed class UmbCheckoutStripeSettingsCacheRefresher : ICacheRefresher
    {
        /// <summary>The same id as UmbCheckout for Umbraco 17.</summary>
        public static readonly Guid UniqueId = new("0c1f6a52-8f43-4c2e-a5d7-3b9e8e2f1d74");

        private readonly SettingsCache<UmbCheckoutStripeSettings> _cache;

        public UmbCheckoutStripeSettingsCacheRefresher(SettingsCache<UmbCheckoutStripeSettings> cache)
        {
            _cache = cache;
        }

        public Guid RefresherUniqueId => UniqueId;

        public string Name => "UmbCheckout Stripe settings cache refresher";

        public void RefreshAll() => _cache.Clear();

        public void Refresh(int id) => _cache.Clear();

        public void Remove(int id) => _cache.Clear();

        public void Refresh(Guid id) => _cache.Clear();
    }
}
