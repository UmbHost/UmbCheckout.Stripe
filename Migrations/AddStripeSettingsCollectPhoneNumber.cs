﻿using Microsoft.Extensions.Logging;
using UmbCheckout.Stripe.Pocos;
using Umbraco.Cms.Infrastructure.Migrations;

namespace UmbCheckout.Stripe.Migrations
{
    internal class AddStripeSettingsCollectPhoneNumber : MigrationBase
    {
        public AddStripeSettingsCollectPhoneNumber(IMigrationContext context) : base(context)
        {
        }

        protected override void Migrate()
        {
            Logger.LogDebug("Running migration {MigrationStep}", "UmbCheckoutAddStripeSettingsCollectPhoneNumber");

            if (!ColumnExists("UmbCheckoutStripeSettings", "CollectPhoneNumber"))
            {
                Create.Column("CollectPhoneNumber").OnTable("UmbCheckoutStripeSettings").AsBoolean().NotNullable().WithDefaultValue(false).Do();
            }
            else
            {
                Logger.LogDebug("The database column {DbColumn} already exists, skipping", "CollectPhoneNumber");
            }
        }
    }
}
