using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UmbCheckout.Stripe.Interfaces;
using UmbCheckout.Stripe.Models;

namespace UmbCheckout.Stripe.Controllers.BackOffice
{
    /// <summary>
    /// Retrieves and updates the Stripe settings for the backoffice
    /// </summary>
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "Stripe")]
    public class StripeSettingsController : UmbCheckoutStripeManagementApiControllerBase
    {
        private readonly IStripeSettingsService _stripeSettingsService;
        private readonly ILogger<StripeSettingsController> _logger;

        public StripeSettingsController(IStripeSettingsService stripeSettingsService, ILogger<StripeSettingsController> logger)
        {
            _stripeSettingsService = stripeSettingsService;
            _logger = logger;
        }

        /// <summary>
        /// Gets the Stripe settings
        /// </summary>
        [HttpGet("settings")]
        [ProducesResponseType<StripeSettingsModel>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSettings()
        {
            try
            {
                var stripeSettings = await _stripeSettingsService.GetStripeSettings();

                return Ok(ToModel(stripeSettings));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Updates the Stripe settings
        /// </summary>
        /// <param name="model">The Stripe settings values</param>
        /// <returns>The updated Stripe settings</returns>
        [HttpPut("settings")]
        [ProducesResponseType<StripeSettingsModel>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateSettings([FromBody] StripeSettingsModel model)
        {
            try
            {
                var stripeSettings = new UmbCheckoutStripeSettings
                {
                    UseLiveApiDetails = model.UseLiveApiDetails,
                    CollectPhoneNumber = model.CollectPhoneNumber,
                    CollectPromotionalEmailsConsent = model.CollectPromotionalEmailsConsent,
                    AllowPromotionalCodes = model.AllowPromotionalCodes,
                    EnableAbandonedCartRecovery = model.EnableAbandonedCartRecovery,
                    AllowPromotionalCodesOnRecoveredCarts = model.AllowPromotionalCodesOnRecoveredCarts,
                    ShippingAllowedCountries = model.ShippingAllowedCountries ?? string.Empty
                };

                var updated = await _stripeSettingsService.UpdateStripeSettings(stripeSettings);

                if (updated)
                {
                    var updatedSettings = await _stripeSettingsService.GetStripeSettings();
                    return Ok(ToModel(updatedSettings));
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        private static StripeSettingsModel ToModel(UmbCheckoutStripeSettings? stripeSettings)
        {
            return new StripeSettingsModel
            {
                UseLiveApiDetails = stripeSettings?.UseLiveApiDetails ?? false,
                CollectPhoneNumber = stripeSettings?.CollectPhoneNumber ?? false,
                CollectPromotionalEmailsConsent = stripeSettings?.CollectPromotionalEmailsConsent ?? false,
                AllowPromotionalCodes = stripeSettings?.AllowPromotionalCodes ?? false,
                EnableAbandonedCartRecovery = stripeSettings?.EnableAbandonedCartRecovery ?? false,
                AllowPromotionalCodesOnRecoveredCarts = stripeSettings?.AllowPromotionalCodesOnRecoveredCarts ?? false,
                ShippingAllowedCountries = stripeSettings?.ShippingAllowedCountries ?? string.Empty
            };
        }
    }
}
