using Microsoft.Extensions.DependencyInjection;
using UmbCheckout.Stripe.Interfaces;
using UmbCheckout.Stripe.Services;
using UmbCheckout.Stripe.Webhooks;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace UmbCheckout.Stripe.Composers
{
    public class RegisterServicesComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddTransient<IStripeSessionService, StripeSessionService>();
            builder.Services.AddTransient<IUmbCheckoutSuccessPageMatcher, UmbCheckoutSuccessPageMatcher>();
            builder.Services.AddTransient<IStripeWebhookEventStore, StripeWebhookEventStore>();
            builder.Services.AddTransient<IStripeWebhookSecretProvider, StripeWebhookSecretProvider>();
            builder.Services.AddTransient<IStripeLineItemPricingService, StripeLineItemPricingService>();
            builder.Services.AddTransient<IStripeBasketPricingService, StripeBasketPricingService>();
            builder.Services.AddTransient<IStripeShippingRateDatabaseService, StripeShippingRateDatabaseService>();
            builder.Services.AddTransient<IStripeShippingRateApiService, StripeShippingRateApiService>();
            builder.Services.AddTransient<IStripeShippingOptionsService, StripeDefaultShippingOptionsService>();
            builder.Services.AddTransient<IStripeSettingsService, StripeSettingsService>();
            // One cached settings row per process, cleared on save here and on every other server by its cache refresher.
            builder.Services.AddSingleton<UmbCheckout.Shared.Persistence.SettingsCache<UmbCheckout.Stripe.Pocos.UmbCheckoutStripeSettings>>();
            builder.CacheRefreshers().Add<UmbCheckout.Stripe.Cache.UmbCheckoutStripeSettingsCacheRefresher>();
            builder.Services.AddTransient<IUmbCheckoutStripeClientFactory, UmbCheckoutStripeClientFactory>();

            builder.WebhookEvents().Add<OnShippingRateDeletedWebhook>();
            builder.WebhookEvents().Add<OnShippingRateSavedWebhook>();
            builder.WebhookEvents().Add<OnChargeFailedWebhook>();
            builder.WebhookEvents().Add<OnChargeSucceededWebhook>();
            builder.WebhookEvents().Add<OnCheckoutSessionCompletedWebhook>();
            builder.WebhookEvents().Add<OnCheckoutSessionExpiredWebhook>();
            builder.WebhookEvents().Add<OnPaymentFailedWebhook>();
            builder.WebhookEvents().Add<OnPaymentIntentCancelledWebhook>();
            builder.WebhookEvents().Add<OnPaymentIntentCreatedWebhook>();
            builder.WebhookEvents().Add<OnPaymentIntentPaymentFailedWebhook>();
            builder.WebhookEvents().Add<OnPaymentIntentSucceededWebhook>();
            builder.WebhookEvents().Add<OnPaymentSuccessWebhook>();
        }
    }
}
