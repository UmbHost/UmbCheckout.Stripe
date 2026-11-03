using System.Net;

namespace UmbCheckout.Stripe.Helpers
{
    /// <summary>
    /// Stripe's own message for a rejected API key (HTTP 401 or 403) echoes the key's last 4 characters, so it is neither shown in a
    /// toast nor logged: <see cref="Detail"/> is shown and logged instead.
    /// </summary>
    public static class StripeKeyRejection
    {
        /// <summary>The fixed detail for a key Stripe rejects.</summary>
        public const string Detail = "Stripe rejected the API key for the active mode (test or live). Check the Stripe settings.";

        /// <summary>Whether Stripe rejected the API key (HTTP 401 or 403).</summary>
        public static bool IsRejected(global::Stripe.StripeException exception) =>
            exception.HttpStatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden;

        /// <summary>The detail to show for a Stripe failure: the fixed <see cref="Detail"/> for a rejected key, else Stripe's message.</summary>
        public static string DetailFor(global::Stripe.StripeException exception) =>
            IsRejected(exception) ? Detail : exception.StripeError?.Message ?? exception.Message;
    }
}
