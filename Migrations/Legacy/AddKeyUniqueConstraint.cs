using Microsoft.Extensions.Logging;
using Umbraco.Cms.Infrastructure.Migrations;

namespace UmbCheckout.Stripe.Migrations.Legacy
{
    /// <summary>Frozen pre-v17 step: only runs on installs that started the NPoco plan. Never change it.</summary>
    internal class AddKeyUniqueConstraint : AsyncMigrationBase
    {
        public AddKeyUniqueConstraint(IMigrationContext context) : base(context)
        {
        }

        protected override Task MigrateAsync()
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

            return Task.CompletedTask;
        }
    }
}
