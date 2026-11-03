using UmbCheckout.Shared.Persistence;
using Umbraco.Cms.Core.Cache;
using UmbCheckoutStripeSettings = UmbCheckout.Stripe.Pocos.UmbCheckoutStripeSettings;

namespace UmbCheckout.Stripe.Cache
{
    /// <summary>
    /// Clears the cached Stripe settings on every server when they are saved on one (Umbraco's distributed cache calls it).
    /// </summary>
    internal sealed class UmbCheckoutStripeSettingsCacheRefresher(SettingsCache<UmbCheckoutStripeSettings> cache) : ICacheRefresher
    {
        public static readonly Guid UniqueId = new("0c1f6a52-8f43-4c2e-a5d7-3b9e8e2f1d74");

        public Guid RefresherUniqueId => UniqueId;

        public string Name => "UmbCheckout Stripe settings cache refresher";

        public void RefreshAll() => cache.Clear();

        public void Refresh(int id) => cache.Clear();

        public void Remove(int id) => cache.Clear();

        public void Refresh(Guid id) => cache.Clear();
    }
}
