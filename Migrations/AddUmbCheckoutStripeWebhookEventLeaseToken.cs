using Microsoft.Extensions.Logging;
using UmbCheckout.Stripe.Pocos;
using Umbraco.Cms.Infrastructure.Migrations;

namespace UmbCheckout.Stripe.Migrations
{
    /// <summary>
    /// The lease token on a claimed webhook event, so an attempt that outlived its lease can't complete or release the claim of
    /// the delivery that took the event over (open: every site).
    /// </summary>
    internal class AddUmbCheckoutStripeWebhookEventLeaseToken : MigrationBase
    {
        public AddUmbCheckoutStripeWebhookEventLeaseToken(IMigrationContext context) : base(context)
        {
        }

        protected override void Migrate()
        {
            Logger.LogDebug("Running migration {MigrationStep}", "UmbCheckoutStripeWebhookEventLeaseToken");

            if (!ColumnExists(UmbCheckoutStripeWebhookEvent.TableName, "LeaseToken"))
            {
                AddColumn<UmbCheckoutStripeWebhookEvent>(UmbCheckoutStripeWebhookEvent.TableName, "LeaseToken");
            }
            else
            {
                Logger.LogDebug("The column {DbColumn} already exists, skipping", "UmbCheckoutStripeWebhookEvents.LeaseToken");
            }
        }
    }
}
