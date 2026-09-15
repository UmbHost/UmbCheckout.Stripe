using System.ComponentModel.DataAnnotations;

namespace UmbCheckout.Stripe.Models
{
    /// <summary>
    /// The request body used to create or update a Shipping Rate via the backoffice Management API
    /// </summary>
    public class ShippingRateRequestModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Value { get; set; } = string.Empty;
    }
}
