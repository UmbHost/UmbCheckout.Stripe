using System.Globalization;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UmbCheckout.Shared.Extensions;
using UmbCheckout.Shared.Models;
using UmbCheckout.Stripe.Interfaces;
using UmbCheckout.Stripe.Models;
using UmbHost.Licensing.Services;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;

namespace UmbCheckout.Stripe.Controllers.BackOffice.Api
{
    /// <summary>
    /// UmbracoAuthorizedApiController to retrieve the Stripe settings for the backoffice
    /// </summary>
    [PluginController(Shared.Consts.PackageName)]
    public class StripeSettingsApiController : UmbracoAuthorizedApiController
    {
        private readonly IStripeSettingsService _stripeSettingsService;
        private readonly ILocalizedTextService _localizedTextService;
        private readonly ILogger<StripeSettingsApiController> _logger;

        public StripeSettingsApiController(ILogger<StripeSettingsApiController> logger, LicenseService licenseService, ILocalizedTextService localizedTextService, IStripeSettingsService stripeSettingsService)
        {
            _logger = logger;
            _localizedTextService = localizedTextService;
            _stripeSettingsService = stripeSettingsService;
            licenseService.RunLicenseCheck();
        }

        /// <summary>
        /// Gets the Stripe settings properties
        /// </summary>
        /// <returns>The Stripe settings properties in JSON</returns>
        [HttpGet]
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
        [HttpPatch]
        public async Task<IActionResult> UpdateStripeSettings([FromBody] StripeSettingsValue configValues)
        {
            try
            {
                var useLiveApiDetails =
                    configValues.UseLiveApiDetails.ToBoolean();

                var collectPhoneNumber =
                    configValues.CollectPhoneNumber.ToBoolean();

                var configuration = new UmbCheckoutStripeSettings()
                {
                    UseLiveApiDetails = useLiveApiDetails,
                    CollectPhoneNumber = collectPhoneNumber,
                    ShippingAllowedCountries = configValues.
                        ShippingAllowedCountries
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
                    Alias = "shippingAllowedCountries",
                    Description = _localizedTextService.Localize(Shared.Consts.LocalizationKeys.Area, Shared.Consts.LocalizationKeys.ShippingAllowedCountries, CultureInfo.CurrentUICulture),
                    Label = _localizedTextService.Localize(Shared.Consts.LocalizationKeys.Area, Shared.Consts.LocalizationKeys.ShippingAllowedCountriesLabel, CultureInfo.CurrentUICulture),
                    Value = stripeSettingsDb != null && !string.IsNullOrEmpty(stripeSettingsDb.ShippingAllowedCountries) ? stripeSettingsDb.ShippingAllowedCountries : "",
                    View = "textbox"
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
