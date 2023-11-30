using Microsoft.Extensions.Logging;
using UmbCheckout.Stripe.Pocos;
using Umbraco.Cms.Infrastructure.Migrations;

namespace UmbCheckout.Stripe.Migrations
{
    internal class AddStripeShippingAllowedCountries : MigrationBase
    {
        public AddStripeShippingAllowedCountries(IMigrationContext context) : base(context)
        {
        }

        protected override void Migrate()
        {
            Logger.LogDebug("Running migration {MigrationStep}", "UmbCheckoutAddStripeShippingAllowedCountries");

            if (!ColumnExists("UmbCheckoutStripeSettings", "ShippingAllowedCountries"))
            {
                AddColumn<UmbCheckoutStripeSettings>("UmbCheckoutStripeSettings", "ShippingAllowedCountries");
            }
            else
            {
                Logger.LogDebug("The database column {DbColumn} already exists, skipping", "ShippingAllowedCountries");
            }
        }
    }
}
