using Microsoft.Extensions.Logging;
using Umbraco.Cms.Infrastructure.Migrations;

namespace UmbCheckout.Stripe.Migrations
{
    internal class AddStripeSettingsCollectPromotionalConsent : MigrationBase
    {
        public AddStripeSettingsCollectPromotionalConsent(IMigrationContext context) : base(context)
        {
        }

        protected override void Migrate()
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
        }
    }
}
