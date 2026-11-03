namespace UmbCheckout.Stripe.Pocos
{
    /// <summary>A Stripe shipping rate row (table UmbCheckoutStripeShipping, mapped in UmbCheckoutStripeShippingConfiguration)</summary>
    public class UmbCheckoutStripeShipping
    {
        public long Id { get; set; }

        /// <summary>Null on rows created before the Key column existed; assigned on the next save.</summary>
        public Guid? Key { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = string.Empty;

        /// <summary>The Stripe shipping rate id</summary>
        public string Value { get; set; } = string.Empty;
    }
}
