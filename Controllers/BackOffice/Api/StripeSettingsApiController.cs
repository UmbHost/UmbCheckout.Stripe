using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UmbCheckout.Stripe.Composers;
using UmbCheckout.Stripe.Interfaces;
using UmbCheckout.Stripe.Models;
using UmbCheckout.Stripe.Models.Api;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Management.Controllers;
using Umbraco.Cms.Api.Management.Routing;

namespace UmbCheckout.Stripe.Controllers.BackOffice.Api
{
    [ApiVersion("1.0")]
    [VersionedApiBackOfficeRoute("umbcheckout/stripe-settings")]
    [ApiExplorerSettings(GroupName = "Stripe Settings")]
    [MapToApi(StripeApiComposer.DocumentName)]
    public class StripeSettingsApiController : ManagementApiControllerBase
    {
        private readonly IStripeSettingsService _stripeSettingsService;

        public StripeSettingsApiController(IStripeSettingsService stripeSettingsService) => _stripeSettingsService = stripeSettingsService;

        [HttpGet]
        [ProducesResponseType(typeof(StripeSettingsResponseModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStripeSettings()
        {
            var settings = await _stripeSettingsService.GetStripeSettings() ?? new UmbCheckoutStripeSettings();
            return Ok(new StripeSettingsResponseModel
            {
                UseLiveApiDetails = settings.UseLiveApiDetails,
                CollectPhoneNumber = settings.CollectPhoneNumber,
                AllowPromotionalCodes = settings.AllowPromotionalCodes,
                CollectPromotionalEmailsConsent = settings.CollectPromotionalEmailsConsent,
                ShippingAllowedCountries = settings.ShippingAllowedCountries ?? string.Empty,
                EnableAbandonedCartRecovery = settings.EnableAbandonedCartRecovery,
                AllowPromotionalCodesOnRecoveredCarts = settings.AllowPromotionalCodesOnRecoveredCarts
            });
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateStripeSettings([FromBody] UpdateStripeSettingsRequestModel model)
        {
            var updated = await _stripeSettingsService.UpdateStripeSettings(new UmbCheckoutStripeSettings
            {
                UseLiveApiDetails = model.UseLiveApiDetails,
                CollectPhoneNumber = model.CollectPhoneNumber,
                AllowPromotionalCodes = model.AllowPromotionalCodes,
                CollectPromotionalEmailsConsent = model.CollectPromotionalEmailsConsent,
                ShippingAllowedCountries = model.ShippingAllowedCountries,
                EnableAbandonedCartRecovery = model.EnableAbandonedCartRecovery,
                AllowPromotionalCodesOnRecoveredCarts = model.AllowPromotionalCodesOnRecoveredCarts
            });

            return updated
                ? Ok()
                : OperationStatusResult(StripeSettingsOperationStatus.SaveFailed, problemDetails => BadRequest(problemDetails
                    .WithTitle("Stripe settings not saved")
                    .WithDetail("The Stripe settings could not be saved.")
                    .Build()));
        }
    }
}
