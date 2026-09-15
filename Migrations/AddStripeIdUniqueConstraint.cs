using Microsoft.Extensions.Logging;
using Umbraco.Cms.Infrastructure.Migrations;

namespace UmbCheckout.Stripe.Migrations
{
    internal class AddStripeIdUniqueConstraint : AsyncMigrationBase
    {
        public AddStripeIdUniqueConstraint(IMigrationContext context) : base(context)
        {
        }

        protected override Task MigrateAsync()
        {
            Logger.LogDebug("Running migration {MigrationStep}", "UmbCheckoutAddStripeIdShippingRateUniqueConstraint");

            if (!IndexExists("IX_stripeIdShippingRate"))
            {
                Create.Index("IX_stripeIdShippingRate").OnTable("UmbCheckoutStripeShipping").WithOptions().NonClustered()
                    .OnColumn("Value").Unique().Do();
            }
            else
            {
                Logger.LogDebug("The index {DbTable} already exists, skipping", "IX_stripeIdShippingRate");
            }
            return Task.CompletedTask;
        }
    }
}
