using Microsoft.Extensions.Logging;
using UmbCheckout.Stripe.Pocos;
using Umbraco.Cms.Infrastructure.Migrations;

namespace UmbCheckout.Stripe.Migrations
{
    internal class AddStripeShippingKey : MigrationBase
    {
        public AddStripeShippingKey(IMigrationContext context) : base(context)
        {
        }

        protected override void Migrate()
        {
            Logger.LogDebug("Running migration {MigrationStep}", "UmbCheckoutAddStripeShippingKey");

            if (!ColumnExists("UmbCheckoutStripeShipping", "Key"))
            {
                AddColumn<UmbCheckoutStripeShipping>("UmbCheckoutStripeShipping", "Key");
            }
            else
            {
                Logger.LogDebug("The database table {DbTable} already exists, skipping", "UmbCheckoutAddStripeShippingKey");
            }
        }
    }
}
