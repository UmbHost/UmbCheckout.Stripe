using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using UmbCheckout.Stripe.Interfaces;
using UmbCheckout.Stripe.Models;
using UmbCheckout.Stripe.Notifications.Webhooks;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.Common.Authorization;
using Umbraco.Cms.Web.Common.Routing;

namespace UmbCheckout.Stripe.Controllers.Api
{
    [PluginController(Shared.Consts.PackageName)]
    [ApiController]
    [BackOfficeRoute($"{Shared.Consts.ApiName}/{Shared.Consts.ApiVersion}/stripe/webhook-api")]
    [Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
    [Authorize(Policy = AuthorizationPolicies.SectionAccessSettings)]
    [MapToApi(Shared.Consts.ApiName)]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "Stripe Webhooks")]
    public class StripeWebhookApiController : Controller
    {
        private readonly ILogger<StripeWebhookApiController> _logger;
        private readonly ICoreScopeProvider _coreScopeProvider;
        private readonly StripeSettings _settings;
        private readonly IStripeSettingsService _stripeSettingsService;

        public StripeWebhookApiController(ILogger<StripeWebhookApiController> logger, ICoreScopeProvider coreScopeProvider, IOptionsMonitor<StripeSettings> stripeSettings, IStripeSettingsService stripeSettingsService)
        {
            _logger = logger;
            _coreScopeProvider = coreScopeProvider;
            _stripeSettingsService = stripeSettingsService;
            _settings = stripeSettings.CurrentValue;
        }

        [HttpPost("checkout-events")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CheckoutEvents()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            try
            {

                var webHookSecret = string.Empty;
                var stripeSettings = _stripeSettingsService.GetStripeSettings().Result;
                if (stripeSettings != null)
                {
                    webHookSecret = stripeSettings.UseLiveApiDetails ? _settings.Live.WebHookSecret : _settings.Test.WebHookSecret;
                }

                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], webHookSecret);

                switch (stripeEvent.Type)
                {
                    case "checkout.session.async_payment_failed":
                        {
                            using var scope = _coreScopeProvider.CreateCoreScope(autoComplete: true);
                            scope.Notifications.Publish(new OnPaymentFailedNotification(stripeEvent));
                            break;
                        }
                    case "checkout.session.async_payment_succeeded":
                        {
                            using var scope = _coreScopeProvider.CreateCoreScope(autoComplete: true);
                            scope.Notifications.Publish(new OnPaymentSuccessNotification(stripeEvent));
                            break;
                        }
                    case "checkout.session.completed":
                        {
                            using var scope = _coreScopeProvider.CreateCoreScope(autoComplete: true);
                            scope.Notifications.Publish(new OnCheckoutSessionCompletedNotification(stripeEvent));
                            break;
                        }
                    case "checkout.session.expired":
                        {
                            using var scope = _coreScopeProvider.CreateCoreScope(autoComplete: true);
                            scope.Notifications.Publish(new OnCheckoutSessionExpiredNotification(stripeEvent));
                            break;
                        }
                    case "charge.succeeded":
                        {
                            using var scope = _coreScopeProvider.CreateCoreScope(autoComplete: true);
                            scope.Notifications.Publish(new OnChargeSucceededNotification(stripeEvent));
                            break;
                        }
                    case "charge.failed":
                        {
                            using var scope = _coreScopeProvider.CreateCoreScope(autoComplete: true);
                            scope.Notifications.Publish(new OnChargeFailedNotification(stripeEvent));
                            break;
                        }
                    case "payment_intent.created":
                        {
                            using var scope = _coreScopeProvider.CreateCoreScope(autoComplete: true);
                            scope.Notifications.Publish(new OnPaymentIntentCreatedNotification(stripeEvent));
                            break;
                        }
                    case "payment_intent.canceled":
                        {
                            using var scope = _coreScopeProvider.CreateCoreScope(autoComplete: true);
                            scope.Notifications.Publish(new OnPaymentIntentCancelledNotification(stripeEvent));
                            break;
                        }
                    case "payment_intent.succeeded":
                        {
                            using var scope = _coreScopeProvider.CreateCoreScope(autoComplete: true);
                            scope.Notifications.Publish(new OnPaymentIntentSucceededNotification(stripeEvent));
                            break;
                        }
                    case "payment_intent.payment_failed":
                        {
                            using var scope = _coreScopeProvider.CreateCoreScope(autoComplete: true);
                            scope.Notifications.Publish(new OnPaymentIntentPaymentFailedNotification(stripeEvent));
                            break;
                        }
                    case "consent.promotions":
                    {
                        using var scope = _coreScopeProvider.CreateCoreScope(autoComplete: true);
                        scope.Notifications.Publish(new OnPromotionalEmailConsentNotification(stripeEvent));
                        break;
                    }
                    default:
                        _logger.LogWarning("Unhandled event type: {0}", stripeEvent.Type);
                        break;
                }


                return Accepted();
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest();
            }
        }
    }
}
