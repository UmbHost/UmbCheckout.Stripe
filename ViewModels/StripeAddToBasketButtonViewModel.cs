using Umbraco.Cms.Core.Models.PublishedContent;

namespace UmbCheckout.Stripe.ViewModels
{
    /// <summary>
    /// View model for the Add to Basket Button View Component
    /// </summary>
    public class StripeAddToBasketButtonViewModel
    {
        public bool ShowQuantity { get; set; }

        public string? QuantityLabel { get; set; }

        public string? ButtonCssClass { get; set; } = null;

        public string ButtonText { get; set; } = string.Empty;

        public Guid? ReturnGuid { get; set; } = null;
        public IPublishedContent? Product { get; set; } = null;

        public string? ProductNameAlias { get; set; } = null;

        public string? InputCssClass { get; set; } = null;
        
        public string? SelectCssClass { get; set; } = null;

        public string? LabelCssClass { get; set; } = null;

        public string? FormGroupSpacerClass { get; set; } = null;

        public string? VariantSelectLabel { get; set; } = null;

        public string? CurrencyCode { get; set; } = null;
    }
}
