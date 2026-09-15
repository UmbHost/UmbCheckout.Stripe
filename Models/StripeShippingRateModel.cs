namespace UmbCheckout.Stripe.Models
{
    /// <summary>
    /// A Shipping Rate as retrieved from the Stripe API, mapped for the backoffice Management API
    /// </summary>
    public class StripeShippingRateModel
    {
        public string Id { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;

        public bool Active { get; set; }

        public long? Amount { get; set; }

        public string? Currency { get; set; }

        public long? DeliveryEstimateMinimumValue { get; set; }

        public string? DeliveryEstimateMinimumUnit { get; set; }

        public long? DeliveryEstimateMaximumValue { get; set; }

        public string? DeliveryEstimateMaximumUnit { get; set; }
    }
}
