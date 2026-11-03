using Microsoft.Extensions.Logging;
using Umbraco.Cms.Infrastructure.Migrations;

namespace UmbCheckout.Stripe.Migrations
{
    /// <summary>
    /// Rows created before the Key column existed have a NULL Key, which the backoffice can't look up. Gives each its own key.
    /// </summary>
    internal abstract class FillMissingKeys : MigrationBase
    {
        protected FillMissingKeys(IMigrationContext context) : base(context)
        {
        }

        /// <summary>The table whose rows get keys.</summary>
        protected abstract string TableName { get; }

        protected override void Migrate()
        {
            Logger.LogDebug("Running migration {MigrationStep}", "UmbCheckoutFillMissingKeys " + TableName);

            if (!TableExists(TableName) || !ColumnExists(TableName, "Key"))
            {
                return;
            }

            var syntax = Database.SqlContext.SqlSyntax;
            var table = syntax.GetQuotedTableName(TableName);
            var key = syntax.GetQuotedColumnName("Key");
            var id = syntax.GetQuotedColumnName("Id");

            foreach (var rowId in Database.Fetch<long>($"SELECT {id} FROM {table} WHERE {key} IS NULL"))
            {
                Database.Execute($"UPDATE {table} SET {key} = @0 WHERE {id} = @1", Guid.NewGuid(), rowId);
            }
        }
    }

    internal class FillMissingShippingKeys : FillMissingKeys
    {
        public FillMissingShippingKeys(IMigrationContext context) : base(context)
        {
        }

        protected override string TableName => "UmbCheckoutStripeShipping";
    }

    internal class FillMissingSettingsKeys : FillMissingKeys
    {
        public FillMissingSettingsKeys(IMigrationContext context) : base(context)
        {
        }

        protected override string TableName => "UmbCheckoutStripeSettings";
    }
}
