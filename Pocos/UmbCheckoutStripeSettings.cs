namespace UmbCheckout.Stripe.Pocos
{
    /// <summary>The Stripe settings row (table UmbCheckoutStripeSettings, mapped in UmbCheckoutStripeSettingsConfiguration)</summary>
    internal class UmbCheckoutStripeSettings
    {
        public long Id { get; set; }

        /// <summary>Null on rows created before the Key column existed; assigned on the next save.</summary>
        public Guid? Key { get; set; } = Guid.NewGuid();

        public bool UseLiveApiDetails { get; set; }

        public bool CollectPhoneNumber { get; set; }

        public bool CollectPromotionalConsent { get; set; }

        public bool AllowPromotionalCodes { get; set; }

        public string? ShippingAllowedCountries { get; set; }

        public bool EnableAbandonedCartRecovery { get; set; }

        public bool AllowPromotionalCodesOnRecoveredCarts { get; set; }
    }
}
