﻿using UmbCheckout.Stripe.NotificationHandlers;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;

namespace UmbCheckout.Stripe.Composers
{
    internal class RegisterNotificationHandlersComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.AddNotificationHandler<TreeNodesRenderingNotification, StripeSettingsTreeNotificationHandler>();
            builder.AddNotificationHandler<TreeNodesRenderingNotification, StripeTreeNotificationHandler>();
            builder.AddNotificationHandler<TreeNodesRenderingNotification, StripeShippingTreeNotificationHandler>();
            builder.AddNotificationAsyncHandler<RoutingRequestNotification, StripeResponseNotificationHandler>();
        }
    }
}
