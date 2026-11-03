using Microsoft.Extensions.Logging;
using UmbCheckout.Stripe.Pocos;
using Umbraco.Cms.Infrastructure.Migrations;

namespace UmbCheckout.Stripe.Migrations
{
    /// <summary>Handled Stripe webhook event ids (created on every site).</summary>
    internal class AddUmbCheckoutStripeWebhookEventsTable : MigrationBase
    {
        public AddUmbCheckoutStripeWebhookEventsTable(IMigrationContext context) : base(context)
        {
        }

        protected override void Migrate()
        {
            Logger.LogDebug("Running migration {MigrationStep}", "UmbCheckoutStripeWebhookEventsTable");

            if (!TableExists(UmbCheckoutStripeWebhookEvent.TableName))
            {
                Create.Table<UmbCheckoutStripeWebhookEvent>().Do();
            }
            else
            {
                Logger.LogDebug("The database table {DbTable} already exists, skipping", UmbCheckoutStripeWebhookEvent.TableName);
            }
        }
    }
}
