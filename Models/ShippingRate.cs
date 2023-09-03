using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace UmbCheckout.Stripe.Models
{
    public class ShippingRate
    {
        [JsonPropertyName("id")]
        [Required]
        public long Id { get; set; }

        [JsonPropertyName("key")]
        [Required]
        public Guid Key { get; set; } = Guid.NewGuid();

        [JsonPropertyName("name")]
        [Required]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("value")]
        [Required]
        public string Value { get; set; } = string.Empty;
    }
}
