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
    /// <summary>The Stripe webhook event table: part of the open package, so it is created on every site (not licence-gated).</summary>
    internal class RunUmbCheckoutStripeWebhookEventsMigration : INotificationHandler<UmbracoApplicationStartingNotification>
    {
        private readonly IMigrationPlanExecutor _migrationPlanExecutor;
        private readonly ICoreScopeProvider _coreScopeProvider;
        private readonly IKeyValueService _keyValueService;
        private readonly IRuntimeState _runtimeState;

        public RunUmbCheckoutStripeWebhookEventsMigration(IMigrationPlanExecutor migrationPlanExecutor, ICoreScopeProvider coreScopeProvider, IKeyValueService keyValueService, IRuntimeState runtimeState)
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
            var migrationPlan = new MigrationPlan("UmbCheckoutStripeWebhookEvents");
            migrationPlan.From(string.Empty)
                .To<AddUmbCheckoutStripeWebhookEventsTable>("3b8f6a52-1d4e-4c79-9e0a-7f2c5d81b6e3");
            return migrationPlan;
        }
    }
}
