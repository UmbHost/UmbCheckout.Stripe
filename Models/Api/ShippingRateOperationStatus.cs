namespace UmbCheckout.Stripe.Models.Api
{
    public enum ShippingRateOperationStatus
    {
        NotFound,
        DuplicateValue,
        SaveFailed,
        StripeError,
        StripeNotConfigured,
        StripeUnreachable
    }
}
