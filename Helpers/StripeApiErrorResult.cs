using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stripe;

namespace UmbCheckout.Stripe.Helpers
{
    /// <summary>
    /// Turns a failed Stripe call in a backoffice API into a problem response (title, detail, status) that the screens show in a toast,
    /// instead of a 500: no API key for the active mode (400), a Stripe error (Stripe's own message; 404 for a missing object, else 400),
    /// or Stripe unreachable (502). Used by the open and the add-on Stripe APIs.
    /// </summary>
    public static class StripeApiErrorResult
    {
        public const string NotConfiguredTitle = "Stripe is not configured";

        public const string StripeErrorTitle = "Stripe returned an error";

        public const string UnreachableTitle = "Stripe could not be reached";

        public const string UnreachableDetail = "Stripe could not be reached. Check the server's connection and try again.";

        private static readonly JsonSerializerOptions Options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        /// <summary>
        /// The problem response for a Stripe failure, logged as a warning; null for any other exception (the caller rethrows it) and
        /// for a timeout when the request itself was aborted.
        /// </summary>
        public static IActionResult? For(Exception exception, ILogger logger, bool requestAborted = false)
        {
            switch (exception)
            {
                case StripeNotConfiguredException notConfigured:
                    logger.LogWarning("Stripe request refused: no Stripe API key is configured for the active mode (live: {Live})", notConfigured.Live);
                    return Problem(StatusCodes.Status400BadRequest, NotConfiguredTitle, notConfigured.Message);

                case StripeException stripe:
                    logger.LogWarning(stripe, "Stripe request failed with HTTP {StatusCode}", (int)stripe.HttpStatusCode);
                    return Problem(stripe.HttpStatusCode == HttpStatusCode.NotFound ? StatusCodes.Status404NotFound : StatusCodes.Status400BadRequest,
                        StripeErrorTitle, stripe.StripeError?.Message ?? stripe.Message);

                case HttpRequestException or TaskCanceledException when !requestAborted:
                    logger.LogWarning(exception, "Stripe could not be reached");
                    return Problem(StatusCodes.Status502BadGateway, UnreachableTitle, UnreachableDetail);

                default:
                    return null;
            }
        }

        private static IActionResult Problem(int status, string title, string detail) =>
            new JsonResult(new ProblemDetails { Status = status, Title = title, Detail = detail }, Options)
            {
                StatusCode = status,
                ContentType = "application/problem+json"
            };
    }
}
