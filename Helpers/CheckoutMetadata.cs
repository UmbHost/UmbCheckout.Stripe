namespace UmbCheckout.Stripe.Helpers
{
    /// <summary>The Stripe product metadata for one checkout line.</summary>
    public static class CheckoutMetadata
    {
        /// <summary>
        /// The line's own values, then the product's values for keys the line doesn't set, then the product's node key.
        /// Returns a new dictionary: the basket line is never changed, and duplicate keys never throw.
        /// </summary>
        public static Dictionary<string, string> ForLine(IReadOnlyDictionary<string, string> lineMetaData, IReadOnlyDictionary<string, string>? productMetaData, Guid productKey)
        {
            var metaData = new Dictionary<string, string>(lineMetaData);
            if (productMetaData != null)
            {
                foreach (var (key, value) in productMetaData)
                {
                    metaData.TryAdd(key, value);
                }
            }

            metaData.TryAdd("nodeKey", productKey.ToString());
            return metaData;
        }
    }
}
