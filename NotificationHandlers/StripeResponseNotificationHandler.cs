using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Extensions.Logging;
using Stripe;
using Stripe.Checkout;
using UmbCheckout.Core.Interfaces;
using UmbCheckout.Shared.Extensions;
using UmbCheckout.Stripe.Interfaces;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace UmbCheckout.Stripe.NotificationHandlers
{
    /// <summary>
    /// Clears the paid basket when Stripe Checkout returns the customer to the configured success page.
    /// </summary>
    public partial class StripeResponseNotificationHandler : INotificationAsyncHandler<RoutingRequestNotification>
    {
        private readonly ISessionService _sessionService;
        private readonly IStripeSessionService _stripeSessionService;
        private readonly IUmbCheckoutSuccessPageMatcher _successPageMatcher;
        private readonly ILogger<StripeResponseNotificationHandler> _logger;

        public StripeResponseNotificationHandler(IStripeSessionService stripeSessionService, ISessionService sessionService,
            IUmbCheckoutSuccessPageMatcher successPageMatcher, ILogger<StripeResponseNotificationHandler> logger)
        {
            _stripeSessionService = stripeSessionService;
            _sessionService = sessionService;
            _successPageMatcher = successPageMatcher;
            _logger = logger;
        }

        // Stripe Checkout session ids: cs_test_… or cs_live_…, letters and digits only.
        [GeneratedRegex("^cs_(test|live)_[A-Za-z0-9]{1,250}$")]
        private static partial Regex CheckoutSessionId();

        public async Task HandleAsync(RoutingRequestNotification notification, CancellationToken cancellationToken)
        {
            var uri = notification.RequestBuilder.Uri;
            if (!uri.Query.Contains("session_id") || !uri.Query.Contains("success"))
            {
                return;
            }

            var queryString = HttpUtility.ParseQueryString(uri.Query);
            var sessionId = queryString.Get("session_id");
            if (queryString.Get("success")?.ToBoolean() != true || string.IsNullOrEmpty(sessionId))
            {
                return;
            }

            try
            {
                if (!await _successPageMatcher.IsSuccessPageAsync(uri, cancellationToken))
                {
                    return;
                }
            }
            catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
            {
                // e.g. the configuration can't be read: the page still renders.
                _logger.LogWarning(ex, "Could not check whether {Path} is the success page", uri.AbsolutePath);
                return;
            }

            if (!CheckoutSessionId().IsMatch(sessionId))
            {
                _logger.LogWarning("Ignored a success redirect with an invalid checkout session id");
                return;
            }

            Session stripeSession;
            try
            {
                stripeSession = await _stripeSessionService.GetSessionAsync(sessionId);
            }
            catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
            {
                // Stripe errors, a missing or malformed key, an HttpClient timeout (TaskCanceledException), an unreachable Stripe.
                // The page still renders; the basket stays until a successful return.
                _logger.LogWarning(ex, "Could not read checkout session {CheckoutSessionId} on the success page", sessionId);
                return;
            }

            // Complete means the customer committed. Delayed methods (Bacs, SEPA) report "unpaid" until the bank confirms;
            // that result arrives by webhook, and keeping the basket would invite paying twice.
            if (stripeSession.Status != "complete")
            {
                _logger.LogInformation("Checkout session {CheckoutSessionId} is {Status}; the basket is kept", sessionId, stripeSession.Status);
                return;
            }

            try
            {
                // Clear the basket that was paid for. This runs while routing, before the page (and its culture) is
                // known, so the basket's culture and currency come from the session's metadata.
                if (stripeSession.Metadata != null && stripeSession.Metadata.TryGetValue(Consts.SessionMetadata.BasketCurrency, out var currency))
                {
                    stripeSession.Metadata.TryGetValue(Consts.SessionMetadata.BasketCulture, out var culture);
                    await _sessionService.Clear(string.IsNullOrEmpty(culture) ? null : culture, currency);
                    _logger.LogInformation("Cleared the {Culture} {Currency} basket paid for by checkout session {CheckoutSessionId}", culture, currency, sessionId);
                }
                else
                {
                    // Sessions created before baskets were per currency carry no basket metadata.
                    await _sessionService.Clear(stripeSession.Currency?.ToUpperInvariant());
                    _logger.LogInformation("Cleared the {Currency} basket paid for by checkout session {CheckoutSessionId} (no basket metadata)", stripeSession.Currency, sessionId);
                }
            }
            catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
            {
                // The customer has paid: the success page must still render.
                _logger.LogError(ex, "Could not clear the basket paid for by checkout session {CheckoutSessionId}", sessionId);
            }
        }
    }
}
