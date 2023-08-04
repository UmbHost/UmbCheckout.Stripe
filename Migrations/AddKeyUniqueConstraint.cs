using Microsoft.Extensions.Logging;
using Umbraco.Cms.Infrastructure.Migrations;

namespace UmbCheckout.Stripe.Migrations
{
    internal class AddKeyUniqueConstraint : MigrationBase
    {
        public AddKeyUniqueConstraint(IMigrationContext context) : base(context)
        {
        }

        protected override void Migrate()
        {
            Logger.LogDebug("Running migration {MigrationStep}", "UmbCheckoutAddKeyUniqueConstraint");

            if (!IndexExists("IX_shippingKey"))
            {
                Create.Index("IX_shippingKey").OnTable("UmbCheckoutStripeShipping").WithOptions().NonClustered()
                    .OnColumn("Key").Unique().Do();
            }
            else
            {
                Logger.LogDebug("The index {DbTable} already exists, skipping", "IX_shippingKey");
            }
        }
    }
}
