using UmbCheckout.Stripe.Migrations;
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
    /// <summary>
    /// The paid checkout sessions table: part of the open package, so it is created on every site (not licence-gated). Basket
    /// reads clear paid baskets on licensed and unlicensed sites alike.
    /// </summary>
    internal class RunUmbCheckoutStripePaidSessionsMigration : INotificationHandler<UmbracoApplicationStartingNotification>
    {
        private readonly IMigrationPlanExecutor _migrationPlanExecutor;
        private readonly ICoreScopeProvider _coreScopeProvider;
        private readonly IKeyValueService _keyValueService;
        private readonly IRuntimeState _runtimeState;

        public RunUmbCheckoutStripePaidSessionsMigration(IMigrationPlanExecutor migrationPlanExecutor, ICoreScopeProvider coreScopeProvider, IKeyValueService keyValueService, IRuntimeState runtimeState)
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

            new Upgrader(CreatePlan()).Execute(_migrationPlanExecutor, _coreScopeProvider, _keyValueService);
        }

        internal static MigrationPlan CreatePlan()
        {
            var migrationPlan = new MigrationPlan("UmbCheckoutStripePaidSessions");
            migrationPlan.From(string.Empty)
                .To<AddUmbCheckoutStripePaidSessionsTable>("5c2a9e17-4b3d-4f86-a1e0-6d8b2c7f9a35");
            return migrationPlan;
        }
    }
}
