using Microsoft.Extensions.Logging;
using NPoco;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace UmbCheckout.Stripe.Migrations.Legacy
{
    /// <summary>Frozen pre-v17 step: only runs on installs that started the NPoco plan. Never change it.</summary>
    internal class AddStripeShippingAllowedCountries : AsyncMigrationBase
    {
        public AddStripeShippingAllowedCountries(IMigrationContext context) : base(context)
        {
        }

        protected override Task MigrateAsync()
        {
            Logger.LogDebug("Running migration {MigrationStep}", "UmbCheckoutAddStripeShippingAllowedCountries");

            if (!ColumnExists("UmbCheckoutStripeSettings", "ShippingAllowedCountries"))
            {
                AddColumn<ShippingAllowedCountriesColumn>("UmbCheckoutStripeSettings", "ShippingAllowedCountries");
            }
            else
            {
                Logger.LogDebug("The database column {DbColumn} already exists, skipping", "ShippingAllowedCountries");
            }

            return Task.CompletedTask;
        }

        [TableName("UmbCheckoutStripeSettings")]
        [ExplicitColumns]
        private sealed class ShippingAllowedCountriesColumn
        {
            [Column("ShippingAllowedCountries")]
            [NullSetting(NullSetting = NullSettings.Null)]
            public string? ShippingAllowedCountries { get; set; }
        }
    }
}
