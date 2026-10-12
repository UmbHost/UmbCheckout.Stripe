using Microsoft.Extensions.Logging;
using Umbraco.Cms.Infrastructure.Migrations;

namespace UmbCheckout.Stripe.Migrations.Legacy
{
    /// <summary>Frozen pre-v17 step: only runs on installs that started the NPoco plan. Never change it.</summary>
    internal class AddStripeSettingsCollectPhoneNumber : AsyncMigrationBase
    {
        public AddStripeSettingsCollectPhoneNumber(IMigrationContext context) : base(context)
        {
        }

        protected override Task MigrateAsync()
        {
            Logger.LogDebug("Running migration {MigrationStep}", "UmbCheckoutAddStripeSettingsCollectPhoneNumber");

            if (!ColumnExists("UmbCheckoutStripeSettings", "CollectPhoneNumber"))
            {
                Create.Column("CollectPhoneNumber").OnTable("UmbCheckoutStripeSettings").AsBoolean().NotNullable().WithDefaultValue(false).Do();
            }
            else
            {
                Logger.LogDebug("The database column {DbColumn} already exists, skipping", "CollectPhoneNumber");
            }

            return Task.CompletedTask;
        }
    }
}
