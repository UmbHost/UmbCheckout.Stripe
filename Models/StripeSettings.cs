namespace UmbCheckout.Stripe.Models
{
    public class StripeSettings
    {
        public StripeApiDetails Live { get; set; } = new();

        public StripeApiDetails Test { get; set; } = new();
    }

    public class StripeApiDetails
    {
        public string WebHookSecret { get; init; } = string.Empty;

        public string ApiKey { get; init; } = string.Empty;
    }
}
