using Microsoft.Extensions.Logging;
using Umbraco.Cms.Infrastructure.Migrations;

namespace UmbCheckout.Stripe.Migrations
{
    internal class AddStripeSettingsAllowPromotionalCodes : MigrationBase
    {
        public AddStripeSettingsAllowPromotionalCodes(IMigrationContext context) : base(context)
        {
        }

        protected override void Migrate()
        {
            Logger.LogDebug("Running migration {MigrationStep}", "UmbCheckoutAddStripeSettingsAllowPromotionalCodes");

            if (!ColumnExists("UmbCheckoutStripeSettings", "AllowPromotionalCodes"))
            {
                Create.Column("AllowPromotionalCodes").OnTable("UmbCheckoutStripeSettings").AsBoolean().NotNullable().WithDefaultValue(false).Do();
            }
            else
            {
                Logger.LogDebug("The database column {DbColumn} already exists, skipping", "AllowPromotionalCodes");
            }
        }
    }
}
