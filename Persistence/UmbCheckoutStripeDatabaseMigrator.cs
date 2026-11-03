using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UmbCheckout.Shared.Persistence;
using UmbCheckout.Stripe.Migrations.Legacy;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;

namespace UmbCheckout.Stripe.Persistence;

/// <summary>Two NPoco plans (settings, shipping) fed this context's tables before v17; both are finished before Baseline is adopted.</summary>
internal sealed class UmbCheckoutStripeDatabaseMigrator(
    IDbContextFactory<UmbCheckoutStripeDbContext> contextFactory,
    IRuntimeState runtimeState,
    IKeyValueService keyValueService,
    ILegacyMigrationPlanRunner legacyPlanRunner,
    ILogger<UmbCheckoutStripeDatabaseMigrator> logger)
    : EfCoreDatabaseMigrator<UmbCheckoutStripeDbContext>(contextFactory, runtimeState, keyValueService, legacyPlanRunner, logger)
{
    protected override IEnumerable<MigrationPlan> LegacyPlans => [CreateSettingsPlan(), CreateShippingPlan()];

    /// <summary>The frozen pre-v17 settings plan (state ids unchanged).</summary>
    internal static MigrationPlan CreateSettingsPlan()
    {
        var plan = new MigrationPlan("UmbCheckoutStripeSettings");
        plan.From(string.Empty).To<AddUmbCheckoutStripeSettingsTable>("fff4ab6a-f9e0-4991-8d28-590547631013");
        plan.From("fff4ab6a-f9e0-4991-8d28-590547631013").To<AddStripeShippingAllowedCountries>("8fa88aa1-e9d7-4e93-94dc-b2934b5f230e");
        plan.From("8fa88aa1-e9d7-4e93-94dc-b2934b5f230e").To<AddStripeSettingsCollectPhoneNumber>("34d9f1ff-bd65-4afd-adf7-993a0c616be8");
        plan.From("34d9f1ff-bd65-4afd-adf7-993a0c616be8").To<AddStripeSettingsCollectPromotionalConsent>("5d796bc1-feff-41e0-ad27-f0d258ec699e");
        plan.From("5d796bc1-feff-41e0-ad27-f0d258ec699e").To<AddStripeSettingsAllowPromotionalCodes>("a9081699-3234-49a2-abbf-0fc48caa9711");
        plan.From("a9081699-3234-49a2-abbf-0fc48caa9711").To<AddStripeSettingsAbandonedCarts>("05b38788-64d1-41cc-9f8c-3e3900127140");
        return plan;
    }

    /// <summary>The frozen pre-v17 shipping plan (state ids unchanged, including the upper-case one).</summary>
    internal static MigrationPlan CreateShippingPlan()
    {
        var plan = new MigrationPlan("UmbCheckoutStripeShipping");
        plan.From(string.Empty).To<AddUmbCheckoutStripeShippingTable>("56d077b2-8954-42e6-a886-baadaf14df56");
        plan.From("56d077b2-8954-42e6-a886-baadaf14df56").To<AddStripeShippingKey>("DD66EDFF-44F4-4B03-B9E5-8FA276B709A3");
        plan.From("DD66EDFF-44F4-4B03-B9E5-8FA276B709A3").To<AddKeyUniqueConstraint>("cc5862ca-619a-49b4-804b-23e6e7145c0e");
        plan.From("cc5862ca-619a-49b4-804b-23e6e7145c0e").To<AddStripeIdUniqueConstraint>("9219beca-8c25-4397-8394-ae9dd51ffbee");
        return plan;
    }
}
