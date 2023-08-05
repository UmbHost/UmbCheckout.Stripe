using System.Text.Json.Serialization;

namespace UmbCheckout.Stripe.Models
{
    internal class ShippingRateResponse
    {
        [JsonPropertyName("key")]
        public Guid Key { get; set; }
        [JsonPropertyName("properties")]
        public IEnumerable<Property> Properties { get; set; } = Enumerable.Empty<Property>();
    }
}
