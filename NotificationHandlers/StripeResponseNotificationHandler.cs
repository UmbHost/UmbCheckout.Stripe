using System.Web;
using UmbCheckout.Core.Interfaces;
using UmbCheckout.Shared.Extensions;
using UmbCheckout.Stripe.Interfaces;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace UmbCheckout.Stripe.NotificationHandlers
{
    public class StripeResponseNotificationHandler : INotificationAsyncHandler<RoutingRequestNotification>
    {
        private readonly ISessionService _sessionService;
        private readonly IStripeSessionService _stripeSessionService;
        public StripeResponseNotificationHandler(IStripeSessionService stripeSessionService, ISessionService sessionService)
        {
            _stripeSessionService = stripeSessionService;
            _sessionService = sessionService;
        }

        public async Task HandleAsync(RoutingRequestNotification notification, CancellationToken cancellationToken)
        {
            if (notification.RequestBuilder.Uri.Query.Contains("session_id") && notification.RequestBuilder.Uri.Query.Contains("success"))
            {
                var queryString = HttpUtility.ParseQueryString(notification.RequestBuilder.Uri.Query);
                var sessionId = queryString.Get("session_id");
                var success = queryString.Get("success")?.ToBoolean();
                if (success.HasValue && success.Value && !string.IsNullOrEmpty(sessionId))
                {
                    var stripeSession = await _stripeSessionService.GetSessionAsync(sessionId);

                    if (stripeSession.Status == "complete")
                    {
                        // Clear the basket that was paid for. This runs while routing, before the page (and its culture) is
                        // known, so the basket's culture and currency come from the session's metadata.
                        if (stripeSession.Metadata != null && stripeSession.Metadata.TryGetValue(Consts.SessionMetadata.BasketCurrency, out var currency))
                        {
                            stripeSession.Metadata.TryGetValue(Consts.SessionMetadata.BasketCulture, out var culture);
                            await _sessionService.Clear(string.IsNullOrEmpty(culture) ? null : culture, currency);
                        }
                        else
                        {
                            // Sessions created before baskets were per currency carry no basket metadata.
                            await _sessionService.Clear(stripeSession.Currency?.ToUpperInvariant());
                        }
                    }
                }
            }
        }
    }
}
