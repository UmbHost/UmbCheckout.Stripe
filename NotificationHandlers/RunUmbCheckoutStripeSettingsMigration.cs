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
            migrationPlan.From("fff4ab6a-f9e0-4991-8d28-590547631013")
                .To<AddStripeShippingAllowedCountries>("8fa88aa1-e9d7-4e93-94dc-b2934b5f230e");
            migrationPlan.From("8fa88aa1-e9d7-4e93-94dc-b2934b5f230e")
                .To<AddStripeSettingsCollectPhoneNumber>("34d9f1ff-bd65-4afd-adf7-993a0c616be8");

            var upgrader = new Upgrader(migrationPlan);
            upgrader.Execute(
                _migrationPlanExecutor,
                _coreScopeProvider,
                _keyValueService);
        }
    }
}
