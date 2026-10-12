namespace UmbCheckout.Stripe.Models.Api
{
    public class UpdateStripeSettingsRequestModel
    {
        public bool UseLiveApiDetails { get; set; }
        public bool CollectPhoneNumber { get; set; }
        public bool AllowPromotionalCodes { get; set; }
        public bool CollectPromotionalEmailsConsent { get; set; }
        public string ShippingAllowedCountries { get; set; } = string.Empty;
        public bool EnableAbandonedCartRecovery { get; set; }
        public bool AllowPromotionalCodesOnRecoveredCarts { get; set; }
    }
}
