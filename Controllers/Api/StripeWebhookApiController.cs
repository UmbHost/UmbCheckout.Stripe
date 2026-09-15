using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stripe;
using UmbCheckout.Stripe.Services;
using UmbCheckout.Stripe.Notifications.Webhooks;
using Umbraco.Cms.Core.Scoping;

namespace UmbCheckout.Stripe.Controllers.Api
{
    /// <summary>
    /// Receives Stripe webhook events.
    /// </summary>
    /// <remarks>
    /// The route is kept identical to the legacy UmbracoApiController-based route (/umbraco/api/StripeWebhookApi/CheckoutEvents)
    /// as merchants have this URL configured in their Stripe webhook settings.
    /// </remarks>
    [AllowAnonymous]
    [Route("umbraco/api/StripeWebhookApi")]
    public class StripeWebhookApiController : ControllerBase
    {
        private readonly ILogger<StripeWebhookApiController> _logger;
        private readonly ICoreScopeProvider _coreScopeProvider;
        private readonly StripeClientProvider _stripeClientProvider;

        public StripeWebhookApiController(ILogger<StripeWebhookApiController> logger, ICoreScopeProvider coreScopeProvider, StripeClientProvider stripeClientProvider)
        {
            _logger = logger;
            _coreScopeProvider = coreScopeProvider;
            _stripeClientProvider = stripeClientProvider;
        }

        [HttpPost("CheckoutEvents")]
        public async Task<IActionResult> CheckoutEvents()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            try
            {

                var (apiDetails, _) = await _stripeClientProvider.GetApiDetailsAsync();
                var webHookSecret = apiDetails.WebHookSecret;

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
