using Microsoft.Extensions.Logging;
using UmbCheckout.Stripe.Pocos;
using Umbraco.Cms.Infrastructure.Migrations;

namespace UmbCheckout.Stripe.Migrations
{
    /// <summary>Checkout sessions the webhook reported paid (created on every site).</summary>
    internal class AddUmbCheckoutStripePaidSessionsTable : MigrationBase
    {
        public AddUmbCheckoutStripePaidSessionsTable(IMigrationContext context) : base(context)
        {
        }

        protected override void Migrate()
        {
            Logger.LogDebug("Running migration {MigrationStep}", "UmbCheckoutStripePaidSessionsTable");

            if (!TableExists(UmbCheckoutStripePaidSession.TableName))
            {
                Create.Table<UmbCheckoutStripePaidSession>().Do();
            }
            else
            {
                Logger.LogDebug("The database table {DbTable} already exists, skipping", UmbCheckoutStripePaidSession.TableName);
            }
        }
    }
}
