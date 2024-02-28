﻿using Microsoft.Extensions.DependencyInjection;
using UmbCheckout.Stripe.Interfaces;
using UmbCheckout.Stripe.Services;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

#if NET8_0_OR_GREATER
using UmbCheckout.Stripe.Webhooks;
#endif

namespace UmbCheckout.Stripe.Composers
{
    public class RegisterServicesComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddTransient<IStripeSessionService, StripeSessionService>();
            builder.Services.AddTransient<IStripeShippingRateDatabaseService, StripeShippingRateDatabaseService>();
            builder.Services.AddTransient<IStripeShippingRateApiService, StripeShippingRateApiService>();
            builder.Services.AddTransient<IStripeSettingsService, StripeSettingsService>();

#if NET8_0_OR_GREATER
            builder.WebhookEvents().Add<OnShippingRateDeletedWebhook>();
            builder.WebhookEvents().Add<OnShippingRateSavedWebhook>();
#endif
        }
    }
}
