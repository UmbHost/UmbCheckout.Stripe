using UmbCheckout.Stripe.NotificationHandlers;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;

namespace UmbCheckout.Stripe.Composers
{
    internal class CreateTablesComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.AddNotificationHandler<UmbracoApplicationStartingNotification, RunUmbCheckoutStripeSettingsMigration>();
            builder.AddNotificationHandler<UmbracoApplicationStartingNotification, RunUmbCheckoutStripeShippingMigration>();
        }
    }
}
