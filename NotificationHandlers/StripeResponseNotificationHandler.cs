using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Stripe.Checkout;
using UmbCheckout.Core.Interfaces;
using UmbCheckout.Shared.Extensions;
using UmbCheckout.Stripe.Interfaces;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Web.Common.DependencyInjection;
using Umbraco.Extensions;

namespace UmbCheckout.Stripe.NotificationHandlers
{
    /// <summary>
    /// Clears the paid basket when Stripe Checkout returns the customer to the configured success page (any culture or domain).
    /// Only the basket the returning checkout session paid for is cleared: not one started later, nor one reached from a stale or
    /// shared success link. Bad input and Stripe errors are logged; the page still renders.
    /// </summary>
    public class StripeResponseNotificationHandler : INotificationAsyncHandler<RoutingRequestNotification>
    {
        /// <summary>Stripe Checkout session ids: cs_test_… or cs_live_…, letters and digits only.</summary>
        private static readonly Regex CheckoutSessionId = new("^cs_(test|live)_[A-Za-z0-9]{1,250}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>A checkout session can't be paid more than a day after it was created (Stripe's longest expiry).</summary>
        private static readonly TimeSpan CheckoutSessionLifetime = TimeSpan.FromHours(24);

        private readonly ISessionService _sessionService;
        private readonly IStripeSessionService _stripeSessionService;
        private readonly IUmbCheckoutSuccessPageMatcher _successPageMatcher;
        private readonly ILogger<StripeResponseNotificationHandler> _logger;
        private readonly Func<IPublishedRequestBuilder, string?> _routedPageCulture;

        [Obsolete("Use the constructor that takes an IUmbCheckoutSuccessPageMatcher and a logger. This one resolves them from the application's services.")]
        public StripeResponseNotificationHandler(IStripeSessionService stripeSessionService, ISessionService sessionService)
            : this(stripeSessionService, sessionService,
                StaticServiceProvider.Instance.GetRequiredService<IUmbCheckoutSuccessPageMatcher>(),
                StaticServiceProvider.Instance.GetRequiredService<ILogger<StripeResponseNotificationHandler>>())
        {
        }

        [ActivatorUtilitiesConstructor]
        public StripeResponseNotificationHandler(IStripeSessionService stripeSessionService, ISessionService sessionService,
            IUmbCheckoutSuccessPageMatcher successPageMatcher, ILogger<StripeResponseNotificationHandler> logger)
            : this(stripeSessionService, sessionService, successPageMatcher, logger, RoutedPageCulture)
        {
        }

        internal StripeResponseNotificationHandler(IStripeSessionService stripeSessionService, ISessionService sessionService,
            IUmbCheckoutSuccessPageMatcher successPageMatcher, ILogger<StripeResponseNotificationHandler> logger, Func<IPublishedRequestBuilder, string?> routedPageCulture)
        {
            _stripeSessionService = stripeSessionService;
            _sessionService = sessionService;
            _successPageMatcher = successPageMatcher;
            _logger = logger;
            _routedPageCulture = routedPageCulture;
        }

        /// <summary>
        /// The basket culture of the routed page, as <c>IBasketContext.SiteRootCulture</c> gives it once routing has finished
        /// (this handler runs before then, after Umbraco's content finders), falling back to the matched domain's culture.
        /// </summary>
        private static string? RoutedPageCulture(IPublishedRequestBuilder request)
            => request.PublishedContent?.Root()?.GetCultureFromDomains() ?? request.Domain?.Culture;

        public async Task HandleAsync(RoutingRequestNotification notification, CancellationToken cancellationToken)
        {
            var request = notification.RequestBuilder;
            var uri = request.Uri;
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
                if (!await _successPageMatcher.IsSuccessPageAsync(request.PublishedContent, uri, cancellationToken))
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

            if (!CheckoutSessionId.IsMatch(sessionId))
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
                // Stripe errors, a missing or malformed key, a timeout, an unreachable Stripe. The page still renders; the basket
                // stays until a successful return.
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
                if (stripeSession.Metadata != null
                    && stripeSession.Metadata.TryGetValue(Consts.SessionMetadata.BasketCurrency, out var currency)
                    && !string.IsNullOrWhiteSpace(currency))
                {
                    // This runs while routing, before the request's basket context knows the page, so the basket's culture and
                    // currency come from the session's metadata.
                    stripeSession.Metadata.TryGetValue(Consts.SessionMetadata.BasketCulture, out var culture);
                    culture = string.IsNullOrEmpty(culture) ? null : culture;

                    // Only the basket this checkout session paid for: not one started after paying, nor a stale or shared link.
                    var current = await _sessionService.Get(culture, currency);
                    if (!string.Equals(current.Basket.CheckoutSessionId, sessionId, StringComparison.Ordinal))
                    {
                        _logger.LogInformation("The current {Culture} {Currency} basket was not paid for by checkout session {CheckoutSessionId}; it is kept", culture, currency, sessionId);
                        return;
                    }

                    await _sessionService.Clear(culture, currency);
                    _logger.LogInformation("Cleared the {Culture} {Currency} basket paid for by checkout session {CheckoutSessionId}", culture, currency, sessionId);
                    return;
                }

                await ClearPreUpgradeBasketAsync(request, stripeSession);
            }
            catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
            {
                // The customer has paid: the success page must still render.
                _logger.LogError(ex, "Could not clear the basket paid for by checkout session {CheckoutSessionId}", sessionId);
            }
        }

        /// <summary>
        /// A checkout session without basket metadata (created before baskets were per culture and currency) is matched to the
        /// routed page's culture's basket in the session's currency. A basket that records this checkout session is the paid one
        /// and is cleared at any age. A basket that records no checkout session (it may still be under the culture-only key) is
        /// cleared only while the session is recent enough to be this return, so a stale link to an old session clears nothing.
        /// A basket sent to another checkout session is kept.
        /// </summary>
        private async Task ClearPreUpgradeBasketAsync(IPublishedRequestBuilder request, Session stripeSession)
        {
            var currency = stripeSession.Currency?.ToUpperInvariant();
            if (string.IsNullOrEmpty(currency))
            {
                _logger.LogInformation("Checkout session {CheckoutSessionId} has no basket details or currency; the basket is kept", stripeSession.Id);
                return;
            }

            var culture = _routedPageCulture(request);
            var current = await _sessionService.Get(culture, currency);
            var recorded = current.Basket.CheckoutSessionId;
            if (!string.Equals(recorded, stripeSession.Id, StringComparison.Ordinal))
            {
                if (!string.IsNullOrEmpty(recorded))
                {
                    _logger.LogInformation("The current {Culture} {Currency} basket was sent to another checkout session; it is kept", culture, currency);
                    return;
                }

                if (stripeSession.Created < DateTime.UtcNow - CheckoutSessionLifetime)
                {
                    _logger.LogInformation("Checkout session {CheckoutSessionId} has no basket details and is not recent; the basket is kept", stripeSession.Id);
                    return;
                }
            }

            await _sessionService.Clear(culture, currency);
            _logger.LogInformation("Cleared the {Culture} {Currency} basket paid for by checkout session {CheckoutSessionId} (no basket details on the session)", culture, currency, stripeSession.Id);
        }
    }
}
