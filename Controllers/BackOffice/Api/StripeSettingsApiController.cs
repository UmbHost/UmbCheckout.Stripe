using Umbraco.Cms.Web.Common.Authorization;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UmbCheckout.Core.ManagementApi;
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
    // Settings > UmbCheckout: changing these requires Settings section access, not just a back-office login.
    [Authorize(Policy = AuthorizationPolicies.SectionAccessSettings)]
    // Editors manage what is stored: reads here skip the Loaded hooks, so display changes aren't saved back.
    [SuppressLoadedHooks]
    public class StripeSettingsApiController : ManagementApiControllerBase
    {
        private readonly IStripeSettingsService _stripeSettingsService;

        public StripeSettingsApiController(IStripeSettingsService stripeSettingsService) => _stripeSettingsService = stripeSettingsService;

        [HttpGet]
        [ProducesResponseType(typeof(StripeSettingsResponseModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStripeSettings()
        {
            var settings = await _stripeSettingsService.GetStripeSettingsAsync(HttpContext?.RequestAborted ?? default) ?? new UmbCheckoutStripeSettings();
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
            var updated = await _stripeSettingsService.UpdateStripeSettingsAsync(new UmbCheckoutStripeSettings
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
                : UmbCheckoutCancellation.Result(HttpContext) ?? OperationStatusResult(StripeSettingsOperationStatus.SaveFailed, problemDetails => BadRequest(problemDetails
                    .WithTitle("Stripe settings not saved")
                    .WithDetail("The Stripe settings could not be saved.")
                    .Build()));
        }
    }
}
