using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using UmbCheckout.Core.Interfaces;
using UmbCheckout.Shared.Extensions;
using UmbCheckout.Stripe.Interfaces;

namespace UmbCheckout.Stripe.Middleware
{
    /// <summary>
    /// Clears the basket when the customer returns from a completed Stripe Checkout session
    /// </summary>
    /// <remarks>
    /// This runs as middleware after the session middleware, as the session is not yet available while Umbraco routes the request
    /// </remarks>
    internal class StripeCheckoutReturnMiddleware
    {
        private readonly RequestDelegate _next;

        public StripeCheckoutReturnMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IStripeSessionService stripeSessionService, ISessionService sessionService, ILogger<StripeCheckoutReturnMiddleware> logger)
        {
            if (HttpMethods.IsGet(context.Request.Method)
                && context.Request.Query.TryGetValue("session_id", out var sessionIdValues)
                && context.Request.Query.TryGetValue("success", out var successValues))
            {
                var sessionId = sessionIdValues.ToString();
                var success = successValues.ToString().ToBoolean();

                if (success && !string.IsNullOrEmpty(sessionId))
                {
                    try
                    {
                        var stripeSession = await stripeSessionService.GetSessionAsync(sessionId);

                        if (stripeSession.Status == "complete")
                        {
                            await sessionService.Clear();
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Unable to clear the basket after returning from Stripe Checkout session {SessionId}", sessionId);
                    }
                }
            }

            await _next(context);
        }
    }
}
