namespace UmbCheckout.Stripe.Models
{
    public class UmbCheckoutStripeSettings
    {
        public long Id { get; set; }

        public Guid Key { get; set; } = Guid.NewGuid();

        public bool UseLiveApiDetails { get; set; } = false;
    }
}
