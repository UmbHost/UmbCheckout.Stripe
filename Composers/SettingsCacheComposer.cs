using Microsoft.Extensions.DependencyInjection;
using UmbCheckout.Shared.Persistence;
using UmbCheckout.Stripe.Cache;
using UmbCheckout.Stripe.Notifications;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Extensions;
using UmbCheckoutStripeSettings = UmbCheckout.Stripe.Pocos.UmbCheckoutStripeSettings;

namespace UmbCheckout.Stripe.Composers
{
    /// <summary>
    /// One cached Stripe settings row per process: cleared on save here, after the outermost commit by the Saved handler (which also
    /// tells every other server), and on every other server by its cache refresher.
    /// </summary>
    public class SettingsCacheComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddSingleton<SettingsCache<UmbCheckoutStripeSettings>>();
            builder.AddNotificationHandler<OnStripeSettingsSavedNotification, UmbCheckoutStripeSettingsSavedCacheHandler>();
            builder.CacheRefreshers().Add<UmbCheckoutStripeSettingsCacheRefresher>();
        }
    }
}
