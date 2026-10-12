using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stripe;
using UmbCheckout.Stripe.Interfaces;
using UmbCheckout.Stripe.Notifications.Webhooks;
using Umbraco.Cms.Core.Events;

namespace UmbCheckout.Stripe.Controllers.Api
{
    /// <summary>
    /// Receives Stripe webhook events (/umbraco/api/stripewebhookapi/checkoutevents) and publishes a notification per event type.
    /// Each event is handled once: Stripe redelivers until it gets a 2xx, so a redelivery is acknowledged without running
    /// handlers, and a failed handler answers 500 (and forgets the event) so Stripe's retry runs it again.
    /// A plain controller (UmbracoApiController is obsolete); the route is spelt out because Stripe endpoints already use it,
    /// and it is anonymous because Stripe signs its requests rather than logging in.
    /// </summary>
    [AllowAnonymous]
    [Route("umbraco/api/stripewebhookapi")]
    public class StripeWebhookApiController : ControllerBase
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

        [HttpPost("checkoutevents")]
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

            var publish = Notification(stripeEvent);
            if (publish == null)
            {
                _logger.LogInformation("Stripe webhook event type {EventType} has no UmbCheckout notification", stripeEvent.Type);
                return Accepted();
            }

            switch (await _eventStore.TryClaimAsync(stripeEvent.Id, cancellationToken))
            {
                case StripeWebhookEventClaim.AlreadyHandled:
                    return Ok(); // a redelivery of a handled event
                case StripeWebhookEventClaim.InProgress:
                    // Another delivery is running the handlers; a non-2xx makes Stripe retry later, by when it has finished
                    // (acknowledged then) or died (taken over once its lease expires).
                    return StatusCode(StatusCodes.Status409Conflict);
            }

            try
            {
                // Not the request's token: if Stripe drops the connection, handlers still finish rather than stop midway.
                await publish(CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Handling Stripe webhook event {EventId} ({EventType}) failed", stripeEvent.Id, stripeEvent.Type);
                try
                {
                    await _eventStore.ForgetAsync(stripeEvent.Id, CancellationToken.None);
                }
                catch (Exception forgetEx)
                {
                    // The claim stays until its lease expires; Stripe's retry takes the event over then.
                    _logger.LogError(forgetEx, "Could not release Stripe webhook event {EventId}", stripeEvent.Id);
                }

                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            try
            {
                await _eventStore.CompleteAsync(stripeEvent.Id, CancellationToken.None);
            }
            catch (Exception ex)
            {
                // The handlers succeeded: acknowledge, so Stripe doesn't re-run them; a redelivery inside the lease gets 409.
                _logger.LogError(ex, "Stripe webhook event {EventId} was handled but could not be marked as done", stripeEvent.Id);
            }

            return Accepted();
        }

        /// <summary>
        /// The event's notification, published immediately and awaited so a failing handler is seen here and Stripe retries;
        /// null when the event type has none.
        /// </summary>
        private Func<CancellationToken, Task>? Notification(Event e) => e.Type switch
        {
            "checkout.session.async_payment_failed" => ct => _eventAggregator.PublishAsync(new OnPaymentFailedNotification(e), ct),
            "checkout.session.async_payment_succeeded" => ct => _eventAggregator.PublishAsync(new OnPaymentSuccessNotification(e), ct),
            "checkout.session.completed" => ct => _eventAggregator.PublishAsync(new OnCheckoutSessionCompletedNotification(e), ct),
            "checkout.session.expired" => ct => _eventAggregator.PublishAsync(new OnCheckoutSessionExpiredNotification(e), ct),
            "charge.succeeded" => ct => _eventAggregator.PublishAsync(new OnChargeSucceededNotification(e), ct),
            "charge.failed" => ct => _eventAggregator.PublishAsync(new OnChargeFailedNotification(e), ct),
            "payment_intent.created" => ct => _eventAggregator.PublishAsync(new OnPaymentIntentCreatedNotification(e), ct),
            "payment_intent.canceled" => ct => _eventAggregator.PublishAsync(new OnPaymentIntentCancelledNotification(e), ct),
            "payment_intent.succeeded" => ct => _eventAggregator.PublishAsync(new OnPaymentIntentSucceededNotification(e), ct),
            "payment_intent.payment_failed" => ct => _eventAggregator.PublishAsync(new OnPaymentIntentPaymentFailedNotification(e), ct),
            "consent.promotions" => ct => _eventAggregator.PublishAsync(new OnPromotionalEmailConsentNotification(e), ct),
            _ => null
        };
    }
}
