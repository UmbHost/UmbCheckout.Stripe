using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Umbraco.Cms.Web.Common.Authorization;
using Umbraco.Cms.Web.Common.Routing;
using Umbraco.Cms.Api.Common.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text.Json;
using UmbCheckout.Shared.Extensions;
using UmbCheckout.Shared.Models;
using UmbCheckout.Stripe.Interfaces;
using UmbCheckout.Stripe.Models;
using UmbHost.Licencing.Services;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Api.Common.Filters;
using Umbraco.Cms.Core;
using Microsoft.AspNetCore.Http;

namespace UmbCheckout.Stripe.Controllers.BackOffice.Api
{
    /// <summary>
    /// UmbracoAuthorizedApiController to retrieve the Stripe settings for the backoffice
    /// </summary>
    [PluginController(Shared.Consts.PackageName)]
    [ApiController]
    [BackOfficeRoute($"{Shared.Consts.ApiName}/{Shared.Consts.ApiVersion}/stripe/settings")]
    [Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
    [Authorize(Policy = AuthorizationPolicies.SectionAccessSettings)]
    [JsonOptionsName(Constants.JsonOptionsNames.BackOffice)]
    [MapToApi(Shared.Consts.ApiName)]
    [ApiVersion("2.0")]
    [ApiExplorerSettings(GroupName = "Stripe Settings")]
    public class StripeSettingsApiController : ControllerBase
    {
        private readonly IStripeSettingsService _stripeSettingsService;
        private readonly ILocalizedTextService _localizedTextService;
        private readonly ILogger<StripeSettingsApiController> _logger;

        public StripeSettingsApiController(ILogger<StripeSettingsApiController> logger, LicenceService licenseService, ILocalizedTextService localizedTextService, IStripeSettingsService stripeSettingsService)
        {
            _logger = logger;
            _localizedTextService = localizedTextService;
            _stripeSettingsService = stripeSettingsService;
            licenseService.RunLicenceCheck();
        }

        /// <summary>
        /// Gets the Stripe settings properties
        /// </summary>
        /// <returns>The Stripe settings properties in JSON</returns>
        [HttpGet("get-settings")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<IActionResult> GetStripeSettings()
        {
            try
            {
                var backOfficeProperties = await GetStripeSettingsProperties();

                return new JsonResult(backOfficeProperties, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Updates the configuration
        /// </summary>
        /// <param name="configValues">The Stripe settings values</param>
        /// <returns>The updated Stripe settings properties in JSON</returns>
        [HttpPatch("update-settings")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateStripeSettings([FromBody] StripeSettingsValue configValues)
        {
            try
            {
                var useLiveApiDetails =
                    configValues.UseLiveApiDetails.ToBoolean();

                var collectPhoneNumber =
                    configValues.CollectPhoneNumber.ToBoolean();

                var collectPromotionalEmailsConsent =
                    configValues.CollectPromotionalEmailsConsent.ToBoolean();

                var allowPromotionalCodesOnRecoveredCarts =
                    configValues.AllowPromotionalCodesOnRecoveredCarts.ToBoolean();

                var enableAbandonedCartRecovery = configValues.EnableAbandonedCartRecovery.ToBoolean();

                var allowPromotionalCodes = configValues.AllowPromotionalCodes.ToBoolean();

                var configuration = new UmbCheckoutStripeSettings()
                {
                    UseLiveApiDetails = useLiveApiDetails,
                    CollectPhoneNumber = collectPhoneNumber,
                    CollectPromotionalEmailsConsent = collectPromotionalEmailsConsent,
                    AllowPromotionalCodes = allowPromotionalCodes,
                    ShippingAllowedCountries = configValues.
                        ShippingAllowedCountries,
                    AllowPromotionalCodesOnRecoveredCarts = allowPromotionalCodesOnRecoveredCarts,
                    EnableAbandonedCartRecovery = enableAbandonedCartRecovery
                };

                var updated = await _stripeSettingsService.UpdateStripeSettings(configuration);

                if (updated)
                {
                    var backOfficeProperties = await GetStripeSettingsProperties();

                    return new JsonResult(backOfficeProperties, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Converts the Stripe settings properties to a JSON string
        /// </summary>
        /// <returns>The Stripe settings properties in JSON</returns>
        private async Task<List<Property>> GetStripeSettingsProperties()
        {
            try
            {
                var stripeSettingsDb = await _stripeSettingsService.GetStripeSettings();
                var backOfficeProperties = new List<Property>
                {
                    new()
                    {
                        Alias = "useLiveApiDetails",
                        Description = _localizedTextService.Localize(Shared.Consts.LocalizationKeys.Area, Shared.Consts.LocalizationKeys.UseLiveApiDetails, CultureInfo.CurrentUICulture),
                        Label = _localizedTextService.Localize(Shared.Consts.LocalizationKeys.Area, Shared.Consts.LocalizationKeys.UseLiveApiDetailsLabel, CultureInfo.CurrentUICulture),
                        Value = stripeSettingsDb != null ? stripeSettingsDb.UseLiveApiDetails.ToString() : "false",
                        View = "boolean"
                    },
                    new()
                    {
                        Alias = "collectPhoneNumber",
                        Description = _localizedTextService.Localize(Shared.Consts.LocalizationKeys.Area, Shared.Consts.LocalizationKeys.CollectPhoneNumber, CultureInfo.CurrentUICulture),
                        Label = _localizedTextService.Localize(Shared.Consts.LocalizationKeys.Area, Shared.Consts.LocalizationKeys.CollectPhoneNumberLabel, CultureInfo.CurrentUICulture),
                        Value = stripeSettingsDb != null ? stripeSettingsDb.CollectPhoneNumber.ToString() : "false",
                        View = "boolean"
                    },
                    new()
                    {
                        Alias = "allowPromotionalCodes",
                        Description = _localizedTextService.Localize(Shared.Consts.LocalizationKeys.Area, Shared.Consts.LocalizationKeys.AllowPromotionalCodes, CultureInfo.CurrentUICulture),
                        Label = _localizedTextService.Localize(Shared.Consts.LocalizationKeys.Area, Shared.Consts.LocalizationKeys.AllowPromotionalCodesLabel, CultureInfo.CurrentUICulture),
                        Value = stripeSettingsDb != null ? stripeSettingsDb.AllowPromotionalCodes.ToString() : "false",
                        View = "boolean"
                    },
                    new()
                    {
                        Alias = "collectPromotionalEmailsConsent",
                        Description = _localizedTextService.Localize(Shared.Consts.LocalizationKeys.Area, Shared.Consts.LocalizationKeys.CollectPromotionalEmailsConsent, CultureInfo.CurrentUICulture),
                        Label = _localizedTextService.Localize(Shared.Consts.LocalizationKeys.Area, Shared.Consts.LocalizationKeys.CollectPromotionalEmailsConsentLabel, CultureInfo.CurrentUICulture),
                        Value = stripeSettingsDb != null ? stripeSettingsDb.CollectPromotionalEmailsConsent.ToString() : "false",
                        View = "boolean"
                    },
                    new()
                    {
                        Alias = "shippingAllowedCountries",
                        Description = _localizedTextService.Localize(Shared.Consts.LocalizationKeys.Area, Shared.Consts.LocalizationKeys.ShippingAllowedCountries, CultureInfo.CurrentUICulture),
                        Label = _localizedTextService.Localize(Shared.Consts.LocalizationKeys.Area, Shared.Consts.LocalizationKeys.ShippingAllowedCountriesLabel, CultureInfo.CurrentUICulture),
                        Value = stripeSettingsDb != null && !string.IsNullOrEmpty(stripeSettingsDb.ShippingAllowedCountries) ? stripeSettingsDb.ShippingAllowedCountries : "",
                        View = "textbox"
                    },
                    new()
                    {
                        Alias = "enableAbandonedCartRecovery",
                        Description = _localizedTextService.Localize(Shared.Consts.LocalizationKeys.Area, Shared.Consts.LocalizationKeys.EnableAbandonedCartRecovery, CultureInfo.CurrentUICulture),
                        Label = _localizedTextService.Localize(Shared.Consts.LocalizationKeys.Area, Shared.Consts.LocalizationKeys.EnableAbandonedCartRecoveryLabel, CultureInfo.CurrentUICulture),
                        Value = stripeSettingsDb != null ? stripeSettingsDb.EnableAbandonedCartRecovery.ToString() : "false",
                        View = "boolean"
                    },
                    new()
                    {
                        Alias = "allowPromotionalCodesOnRecoveredCarts",
                        Description = _localizedTextService.Localize(Shared.Consts.LocalizationKeys.Area, Shared.Consts.LocalizationKeys.AllowPromotionalCodesOnRecoveredCarts, CultureInfo.CurrentUICulture),
                        Label = _localizedTextService.Localize(Shared.Consts.LocalizationKeys.Area, Shared.Consts.LocalizationKeys.AllowPromotionalCodesOnRecoveredCartsLabel, CultureInfo.CurrentUICulture),
                        Value = stripeSettingsDb != null ? stripeSettingsDb.AllowPromotionalCodesOnRecoveredCarts.ToString() : "false",
                        View = "boolean"
                    }
                };

                return backOfficeProperties;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
