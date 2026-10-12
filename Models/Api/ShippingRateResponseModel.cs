namespace UmbCheckout.Stripe.Models.Api
{
    public class ShippingRateResponseModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}
