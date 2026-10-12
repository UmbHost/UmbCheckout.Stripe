using Microsoft.Extensions.Logging;
using Umbraco.Cms.Infrastructure.Migrations;

namespace UmbCheckout.Stripe.Migrations.Legacy
{
    /// <summary>Frozen pre-v17 step: only runs on installs that started the NPoco plan. Never change it.</summary>
    internal class AddStripeSettingsCollectPromotionalConsent : AsyncMigrationBase
    {
        public AddStripeSettingsCollectPromotionalConsent(IMigrationContext context) : base(context)
        {
        }

        protected override Task MigrateAsync()
        {
            Logger.LogDebug("Running migration {MigrationStep}", "UmbCheckoutAddStripeSettingsCollectPromotionalConsent");

            if (!ColumnExists("UmbCheckoutStripeSettings", "CollectPromotionalConsent"))
            {
                Create.Column("CollectPromotionalConsent").OnTable("UmbCheckoutStripeSettings").AsBoolean().NotNullable().WithDefaultValue(false).Do();
            }
            else
            {
                Logger.LogDebug("The database column {DbColumn} already exists, skipping", "CollectPromotionalConsent");
            }

            return Task.CompletedTask;
        }
    }
}
