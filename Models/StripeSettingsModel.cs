namespace UmbCheckout.Stripe.Models
{
    /// <summary>
    /// The Stripe settings as exposed by the backoffice Management API
    /// </summary>
    public class StripeSettingsModel
    {
        public bool UseLiveApiDetails { get; set; }

        public bool CollectPhoneNumber { get; set; }

        public bool CollectPromotionalEmailsConsent { get; set; }

        public bool AllowPromotionalCodes { get; set; }

        public bool EnableAbandonedCartRecovery { get; set; }

        public bool AllowPromotionalCodesOnRecoveredCarts { get; set; }

        /// <remarks>
        /// Nullable: with Nullable enabled, ASP.NET treats a non-nullable reference-type property on a
        /// [FromBody] model as implicitly required (rejecting null/empty), which would make it impossible
        /// to save settings with no allowed countries configured.
        /// </remarks>
        public string? ShippingAllowedCountries { get; set; } = string.Empty;
    }
}
