using Microsoft.Extensions.Logging;
using Umbraco.Cms.Infrastructure.Migrations;

namespace UmbCheckout.Stripe.Migrations
{
    internal class AddStripeSettingsAbandonedCarts : MigrationBase
    {
        public AddStripeSettingsAbandonedCarts(IMigrationContext context) : base(context)
        {
        }

        protected override void Migrate()
        {
            Logger.LogDebug("Running migration {MigrationStep}", "UmbCheckoutAddStripeSettingsAbandonedCarts");

            if (!ColumnExists("UmbCheckoutStripeSettings", "EnableAbandonedCartRecovery"))
            {
                Create.Column("EnableAbandonedCartRecovery").OnTable("UmbCheckoutStripeSettings").AsBoolean().NotNullable().WithDefaultValue(false).Do();
            }
            else
            {
                Logger.LogDebug("The database column {DbColumn} already exists, skipping", "EnableAbandonedCartRecovery");
            }

            if (!ColumnExists("UmbCheckoutStripeSettings", "AllowPromotionalCodesOnRecoveredCarts"))
            {
                Create.Column("AllowPromotionalCodesOnRecoveredCarts").OnTable("UmbCheckoutStripeSettings").AsBoolean().NotNullable().WithDefaultValue(false).Do();
            }
            else
            {
                Logger.LogDebug("The database column {DbColumn} already exists, skipping", "AllowPromotionalCodesOnRecoveredCarts");
            }
        }
    }
}
