using System.ComponentModel.DataAnnotations;

namespace UmbCheckout.Stripe.Models.Api
{
    public class CreateShippingRateRequestModel
    {
        public Guid? Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Value { get; set; } = string.Empty;
    }
}
