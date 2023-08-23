using Umbraco.Cms.Core.Models.PublishedContent;

namespace UmbCheckout.Stripe.ViewModels
{
    /// <summary>
    /// View model for the Add to Basket Button View Component
    /// </summary>
    public class StripeAddToBasketButtonViewModel
    {

        public string? LinkCssClass { get; set; } = null;

        public string LinkName { get; set; } = string.Empty;

        public Guid? ReturnGuid { get; set; } = null;
        public IPublishedContent? Product { get; set; } = null;
    }
}
