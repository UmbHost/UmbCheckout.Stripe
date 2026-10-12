namespace UmbCheckout.Stripe.Models.Api
{
    /// <summary>A shipping rate as it exists in Stripe (v13's "Stripe Shipping Rate Details").</summary>
    public class StripeShippingRateResponseModel
    {
        public string Id { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public MoneyResponseModel? Amount { get; set; }
        public bool Active { get; set; }
        public DeliveryEstimateResponseModel? DeliveryEstimateMinimum { get; set; }
        public DeliveryEstimateResponseModel? DeliveryEstimateMaximum { get; set; }

        /// <summary>The rate in the Stripe dashboard, in the rate's own mode (test or live).</summary>
        public string DashboardUrl { get; set; } = string.Empty;

        public static StripeShippingRateResponseModel From(global::Stripe.ShippingRate rate) => new()
        {
            Id = rate.Id,
            DisplayName = rate.DisplayName,
            Amount = rate.FixedAmount is null ? null : MoneyResponseModel.From(rate.FixedAmount.Amount, rate.FixedAmount.Currency),
            Active = rate.Active,
            DeliveryEstimateMinimum = rate.DeliveryEstimate?.Minimum is { } min ? new DeliveryEstimateResponseModel { Value = min.Value, Unit = min.Unit } : null,
            DeliveryEstimateMaximum = rate.DeliveryEstimate?.Maximum is { } max ? new DeliveryEstimateResponseModel { Value = max.Value, Unit = max.Unit } : null,
            DashboardUrl = $"https://dashboard.stripe.com/{(rate.Livemode ? "" : "test/")}shipping-rates/{rate.Id}"
        };
    }
}
