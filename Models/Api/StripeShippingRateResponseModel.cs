namespace UmbCheckout.Stripe.Models.Api
{
    public class StripeShippingRateResponseModel
    {
        public string Id { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public long? Amount { get; set; }
        public string? Currency { get; set; }
        public bool Active { get; set; }

        public static StripeShippingRateResponseModel From(global::Stripe.ShippingRate rate) => new()
        {
            Id = rate.Id,
            DisplayName = rate.DisplayName,
            Amount = rate.FixedAmount?.Amount,
            Currency = rate.FixedAmount?.Currency,
            Active = rate.Active
        };
    }
}
