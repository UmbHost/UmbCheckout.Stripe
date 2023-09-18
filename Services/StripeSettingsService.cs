using Microsoft.Extensions.Logging;
using UmbCheckout.Stripe.Interfaces;
using UmbCheckout.Stripe.Models;
using UmbCheckout.Stripe.Notifications;
using UmbHost.Licensing.Services;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Scoping;
using IScopeProvider = Umbraco.Cms.Infrastructure.Scoping.IScopeProvider;

namespace UmbCheckout.Stripe.Services
{
    /// <summary>
    /// A service which Gets or Updates the configuration
    /// </summary>
    internal class StripeSettingsService : IStripeSettingsService
    {
        private readonly IScopeProvider _scopeProvider;
        private readonly ICoreScopeProvider _coreScopeProvider;
        private readonly IUmbracoMapper _mapper;
        private readonly ILogger<StripeSettingsService> _logger;

        public StripeSettingsService(IScopeProvider scopeProvider, ILogger<StripeSettingsService> logger, IUmbracoMapper mapper, ICoreScopeProvider coreScopeProvider, LicenseService licenseService)
        {
            _scopeProvider = scopeProvider;
            _logger = logger;
            _mapper = mapper;
            _coreScopeProvider = coreScopeProvider;
            licenseService.RunLicenseCheck();
        }

        /// <inheritdoc />
        public async Task<UmbCheckoutStripeSettings?> GetStripeSettings()
        {
            try
            {
                using var scope = _scopeProvider.CreateScope(autoComplete: true);
                var result = await scope.Database.QueryAsync<Pocos.UmbCheckoutStripeSettings>().SingleOrDefault();

                return _mapper.Map<Pocos.UmbCheckoutStripeSettings, UmbCheckoutStripeSettings>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<bool> UpdateStripeSettings(UmbCheckoutStripeSettings stripeSettings)
        {
            try
            {
                using var coreScope = _coreScopeProvider.CreateCoreScope(autoComplete: true);
                var stripeSettingsPoco = _mapper.Map<UmbCheckoutStripeSettings, Pocos.UmbCheckoutStripeSettings>(stripeSettings);
                var existingConfiguration = await GetStripeSettings();
                if (existingConfiguration != null)
                {
                    if (stripeSettingsPoco != null)
                    {
                        stripeSettingsPoco.Id = existingConfiguration.Id;
                        stripeSettingsPoco.Key = existingConfiguration.Key;
                        using var scope = _scopeProvider.CreateScope(autoComplete: true);
                        var db = scope.Database;
                        var result = await db.UpdateAsync(stripeSettingsPoco);
                        var updatedConfiguration = await GetStripeSettings();
                        scope.Notifications.Publish(new OnStripeSettingsSavedNotification(updatedConfiguration));
                        return result != 0;
                    }
                }

                if (stripeSettingsPoco != null)
                {
                    var created = await CreateStripeSettings(stripeSettingsPoco);

                    return created;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Inserts the initial Stripe settings
        /// </summary>
        /// <param name="stripeSettings">The Stripe settings</param>
        /// <returns>true is inserted successfully false if not</returns>
        private async Task<bool> CreateStripeSettings(Pocos.UmbCheckoutStripeSettings stripeSettings)
        {
            try
            {
                using var scope = _scopeProvider.CreateScope(autoComplete: true);
                var db = scope.Database;
                var result = (long?)await db.InsertAsync(stripeSettings);

                return result is not null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
