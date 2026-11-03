using Microsoft.Extensions.Logging;
using NPoco;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace UmbCheckout.Stripe.Migrations.Legacy
{
    /// <summary>Frozen pre-v17 step: only runs on installs that started the NPoco plan. Never change it.</summary>
    internal class AddStripeShippingKey : AsyncMigrationBase
    {
        public AddStripeShippingKey(IMigrationContext context) : base(context)
        {
        }

        protected override Task MigrateAsync()
        {
            Logger.LogDebug("Running migration {MigrationStep}", "UmbCheckoutAddStripeShippingKey");

            if (!ColumnExists("UmbCheckoutStripeShipping", "Key"))
            {
                AddColumn<KeyColumn>("UmbCheckoutStripeShipping", "Key");
            }
            else
            {
                Logger.LogDebug("The database table {DbTable} already exists, skipping", "UmbCheckoutAddStripeShippingKey");
            }

            return Task.CompletedTask;
        }

        [TableName("UmbCheckoutStripeShipping")]
        [ExplicitColumns]
        private sealed class KeyColumn
        {
            [Column("Key")]
            [Index(IndexTypes.UniqueNonClustered, Name = "IX_shippingKey")]
            [NullSetting(NullSetting = NullSettings.Null)]
            public Guid Key { get; set; }
        }
    }
}
