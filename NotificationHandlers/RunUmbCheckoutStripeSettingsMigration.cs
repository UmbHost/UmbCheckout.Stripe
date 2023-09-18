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
    internal class RunUmbCheckoutStripeSettingsMigration : INotificationHandler<UmbracoApplicationStartingNotification>
    {
        private readonly IMigrationPlanExecutor _migrationPlanExecutor;
        private readonly ICoreScopeProvider _coreScopeProvider;
        private readonly IKeyValueService _keyValueService;
        private readonly IRuntimeState _runtimeState;

        public RunUmbCheckoutStripeSettingsMigration(IMigrationPlanExecutor migrationPlanExecutor, ICoreScopeProvider coreScopeProvider, IKeyValueService keyValueService, IRuntimeState runtimeState)
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

            var migrationPlan = new MigrationPlan("UmbCheckoutStripeSettings");
            migrationPlan.From(string.Empty)
                .To<AddUmbCheckoutStripeSettingsTable>("fff4ab6a-f9e0-4991-8d28-590547631013");

            var upgrader = new Upgrader(migrationPlan);
            upgrader.Execute(
                _migrationPlanExecutor,
                _coreScopeProvider,
                _keyValueService);
        }
    }
}
