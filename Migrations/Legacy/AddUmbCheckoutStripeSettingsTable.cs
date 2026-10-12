using Microsoft.Extensions.Logging;
using NPoco;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace UmbCheckout.Stripe.Migrations.Legacy
{
    /// <summary>Frozen pre-v17 step: only runs on installs that started the NPoco plan. Never change it.</summary>
    internal class AddUmbCheckoutStripeSettingsTable : AsyncMigrationBase
    {
        public AddUmbCheckoutStripeSettingsTable(IMigrationContext context) : base(context)
        {
        }

        protected override Task MigrateAsync()
        {
            Logger.LogDebug("Running migration {MigrationStep}", "UmbCheckoutStripeSettingsTable");

            if (TableExists("UmbCheckoutStripeSettings") == false)
            {
                Create.Table<UmbCheckoutStripeSettingsSchema>().Do();
            }
            else
            {
                Logger.LogDebug("The database table {DbTable} already exists, skipping", "UmbCheckoutStripeSettingsTable");
            }

            return Task.CompletedTask;
        }

        [TableName("UmbCheckoutStripeSettings")]
        [PrimaryKey("Id", AutoIncrement = true)]
        [ExplicitColumns]
        public class UmbCheckoutStripeSettingsSchema
        {
            [PrimaryKeyColumn(AutoIncrement = true, IdentitySeed = 1)]
            [Column("Id")]
            public long Id { get; set; }

            [Column("Key")]
            [NullSetting(NullSetting = NullSettings.Null)]
            public Guid Key { get; set; } = Guid.NewGuid();

            [Column("UseLiveApiDetails")]
            [NullSetting(NullSetting = NullSettings.NotNull)]
            public bool UseLiveApiDetails { get; set; } = false;
        }
    }
}
