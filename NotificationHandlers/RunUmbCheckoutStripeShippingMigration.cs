﻿using UmbCheckout.Stripe.Migrations;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;

namespace UmbCheckout.Stripe.NotificationHandlers
{
    internal class RunUmbCheckoutStripeShippingMigration : INotificationHandler<UmbracoApplicationStartingNotification>
    {
        private readonly IMigrationPlanExecutor _migrationPlanExecutor;
        private readonly ICoreScopeProvider _coreScopeProvider;
        private readonly IKeyValueService _keyValueService;
        private readonly IRuntimeState _runtimeState;

        public RunUmbCheckoutStripeShippingMigration(IMigrationPlanExecutor migrationPlanExecutor, ICoreScopeProvider coreScopeProvider, IKeyValueService keyValueService, IRuntimeState runtimeState)
        {
            _migrationPlanExecutor = migrationPlanExecutor;
            _coreScopeProvider = coreScopeProvider;
            _keyValueService = keyValueService;
            _runtimeState = runtimeState;
        }

        public void Handle(UmbracoApplicationStartingNotification notification)
        {
            if (_runtimeState.Level < RuntimeLevel.Run)
            {
                return;
            }

            var migrationPlan = new MigrationPlan("UmbCheckoutStripeShipping");
            migrationPlan.From(string.Empty)
                .To<AddUmbCheckoutStripeShippingTable>("56d077b2-8954-42e6-a886-baadaf14df56");
            migrationPlan.From("56d077b2-8954-42e6-a886-baadaf14df56")
                .To<AddStripeShippingKey>("DD66EDFF-44F4-4B03-B9E5-8FA276B709A3");
            migrationPlan.From("DD66EDFF-44F4-4B03-B9E5-8FA276B709A3")
                .To<AddKeyUniqueConstraint>("cc5862ca-619a-49b4-804b-23e6e7145c0e");
            migrationPlan.From("cc5862ca-619a-49b4-804b-23e6e7145c0e")
                .To<AddStripeIdUniqueConstraint>("9219beca-8c25-4397-8394-ae9dd51ffbee");

            var upgrader = new Upgrader(migrationPlan);
            upgrader.Execute(
                _migrationPlanExecutor,
                _coreScopeProvider,
                _keyValueService);
        }
    }
}
