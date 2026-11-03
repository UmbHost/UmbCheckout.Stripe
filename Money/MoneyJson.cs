using System.Text.Json;
using System.Text.Json.Nodes;
using UmbCheckout.Stripe.Models.Api;

namespace UmbCheckout.Stripe.Money
{
    /// <summary>
    /// Adds money objects to a backoffice API response without changing what it already returns: the response is serialised exactly
    /// as before (same serializer and options), and a <see cref="MoneyResponseModel"/> is added next to each amount the screens show.
    /// </summary>
    internal static class MoneyJson
    {
        /// <summary>The value as the System.Text.Json result executor writes it: its runtime type, with <paramref name="options"/>.</summary>
        public static JsonNode? Serialize(object? value, JsonSerializerOptions options)
            => value is null ? null : JsonSerializer.SerializeToNode(value, value.GetType(), options);

        /// <summary>Sets <paramref name="name"/> on <paramref name="item"/> to the money object for the amount.</summary>
        public static void Add(JsonNode? item, string name, long minorUnits, string? currency, JsonSerializerOptions options)
        {
            if (item is JsonObject obj)
            {
                obj[name] = JsonSerializer.SerializeToNode(MoneyResponseModel.From(minorUnits, currency), options);
            }
        }
    }
}
