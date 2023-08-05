using Microsoft.Extensions.Logging;
using Umbraco.Cms.Infrastructure.Migrations;

namespace UmbCheckout.Stripe.Migrations
{
    internal class AddStripeIdUniqueConstraint : MigrationBase
    {
        public AddStripeIdUniqueConstraint(IMigrationContext context) : base(context)
        {
        }

        protected override void Migrate()
        {
            Logger.LogDebug("Running migration {MigrationStep}", "UmbCheckoutAddKeyUniqueConstraint");

            if (!IndexExists("IX_stripeId"))
            {
                Create.Index("IX_stripeId").OnTable("UmbCheckoutStripeShipping").WithOptions().NonClustered()
                    .OnColumn("Value").Unique().Do();
            }
            else
            {
                Logger.LogDebug("The index {DbTable} already exists, skipping", "IX_shippingKey");
            }
        }
    }
}
