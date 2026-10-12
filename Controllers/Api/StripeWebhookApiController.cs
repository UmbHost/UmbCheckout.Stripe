using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stripe;
using UmbCheckout.Stripe.Interfaces;
using UmbCheckout.Stripe.Notifications.Webhooks;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Web.Common.Controllers;

namespace UmbCheckout.Stripe.Controllers.Api
{
    /// <summary>
    /// Receives Stripe webhook events (/umbraco/api/stripewebhookapi/checkoutevents) and publishes a notification per event type.
    /// Each event is handled once: Stripe redelivers until it gets a 2xx, so a redelivery is acknowledged without running
    /// handlers, and a failed handler answers 500 (and forgets the event) so Stripe's retry runs it again.
    /// </summary>
    public class StripeWebhookApiController : UmbracoApiController
    {
        /// <summary>API versions already warned about, so a mismatched endpoint logs once per version rather than per event.</summary>
        private static readonly ConcurrentDictionary<string, bool> WarnedApiVersions = new();

        private readonly ILogger<StripeWebhookApiController> _logger;
        private readonly IEventAggregator _eventAggregator;
        private readonly IStripeWebhookSecretProvider _secretProvider;
        private readonly IStripeWebhookEventStore _eventStore;

        public StripeWebhookApiController(ILogger<StripeWebhookApiController> logger, IEventAggregator eventAggregator,
            IStripeWebhookSecretProvider secretProvider, IStripeWebhookEventStore eventStore)
        {
            _logger = logger;
            _eventAggregator = eventAggregator;
            _secretProvider = secretProvider;
            _eventStore = eventStore;
        }

        [HttpPost]
        public async Task<IActionResult> CheckoutEvents()
        {
            var cancellationToken = HttpContext.RequestAborted;
            string json;
            using (var reader = new StreamReader(HttpContext.Request.Body))
            {
                json = await reader.ReadToEndAsync(cancellationToken);
            }

            var (secret, live) = await _secretProvider.GetAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(secret))
            {
                // 500 so Stripe keeps retrying until the secret is configured.
                _logger.LogError("Stripe webhook received but the {Mode} webhook secret is not configured", live ? "live" : "test");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            Event stripeEvent;
            try
            {
                // Stripe.net pins one API version; an endpoint created on another (a new account may not be able to choose
                // ours) is still accepted, with a warning once per version.
                stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], secret, throwOnApiVersionMismatch: false);
            }
            catch (StripeException ex)
            {
                _logger.LogWarning(ex, "Rejected a Stripe webhook with an invalid signature or payload");
                return BadRequest();
            }

            if (stripeEvent.ApiVersion != StripeConfiguration.ApiVersion && WarnedApiVersions.TryAdd(stripeEvent.ApiVersion ?? string.Empty, true))
            {
                _logger.LogWarning("Stripe webhook events use API version {EventApiVersion}; UmbCheckout expects {ExpectedApiVersion}. Create the webhook endpoint on {ExpectedApiVersion} where possible",
                    stripeEvent.ApiVersion, StripeConfiguration.ApiVersion, StripeConfiguration.ApiVersion);
            }

            if (!await _eventStore.TryBeginAsync(stripeEvent.Id, cancellationToken))
            {
                return Ok(); // already handled: a redelivery
            }

            try
            {
                if (!await PublishAsync(stripeEvent, cancellationToken))
                {
                    _logger.LogInformation("Stripe webhook event type {EventType} has no UmbCheckout notification", stripeEvent.Type);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Handling Stripe webhook event {EventId} ({EventType}) failed", stripeEvent.Id, stripeEvent.Type);
                await _eventStore.ForgetAsync(stripeEvent.Id, CancellationToken.None);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Accepted();
        }

        /// <summary>Published immediately and awaited, so a failing handler is seen here and Stripe retries.</summary>
        /// <returns>false when the event type has no notification.</returns>
        private async Task<bool> PublishAsync(Event e, CancellationToken cancellationToken)
        {
            switch (e.Type)
            {
                case "checkout.session.async_payment_failed": await _eventAggregator.PublishAsync(new OnPaymentFailedNotification(e), cancellationToken); return true;
                case "checkout.session.async_payment_succeeded": await _eventAggregator.PublishAsync(new OnPaymentSuccessNotification(e), cancellationToken); return true;
                case "checkout.session.completed": await _eventAggregator.PublishAsync(new OnCheckoutSessionCompletedNotification(e), cancellationToken); return true;
                case "checkout.session.expired": await _eventAggregator.PublishAsync(new OnCheckoutSessionExpiredNotification(e), cancellationToken); return true;
                case "charge.succeeded": await _eventAggregator.PublishAsync(new OnChargeSucceededNotification(e), cancellationToken); return true;
                case "charge.failed": await _eventAggregator.PublishAsync(new OnChargeFailedNotification(e), cancellationToken); return true;
                case "payment_intent.created": await _eventAggregator.PublishAsync(new OnPaymentIntentCreatedNotification(e), cancellationToken); return true;
                case "payment_intent.canceled": await _eventAggregator.PublishAsync(new OnPaymentIntentCancelledNotification(e), cancellationToken); return true;
                case "payment_intent.succeeded": await _eventAggregator.PublishAsync(new OnPaymentIntentSucceededNotification(e), cancellationToken); return true;
                case "payment_intent.payment_failed": await _eventAggregator.PublishAsync(new OnPaymentIntentPaymentFailedNotification(e), cancellationToken); return true;
                case "consent.promotions": await _eventAggregator.PublishAsync(new OnPromotionalEmailConsentNotification(e), cancellationToken); return true;
                default: return false;
            }
        }
    }
}
