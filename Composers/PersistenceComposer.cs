using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using UmbCheckout.Shared.Persistence;
using UmbCheckout.Stripe.Persistence;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Extensions;

namespace UmbCheckout.Stripe.Composers
{
    /// <summary>Registers the Stripe settings and shipping rate tables' EF Core context and their startup migration. Sites need nothing in Program.cs.</summary>
    internal class PersistenceComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.TryAddTransient<ILegacyMigrationPlanRunner, UpgraderLegacyMigrationPlanRunner>();
            builder.Services.AddUmbracoDbContext<UmbCheckoutStripeDbContext>(
                (_, options, connectionString, providerName) => options.UseUmbCheckoutProvider(providerName, connectionString, UmbCheckoutStripeDbContext.HistoryTable),
                shareUmbracoConnection: true);
            builder.AddNotificationAsyncHandler<UmbracoApplicationStartingNotification, UmbCheckoutStripeDatabaseMigrator>();
        }
    }
}
