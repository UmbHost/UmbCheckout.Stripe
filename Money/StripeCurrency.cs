namespace UmbCheckout.Stripe.Money
{
    /// <summary>
    /// Converts between Stripe's integer minor-unit amounts and decimal major-unit amounts.
    /// Source: https://docs.stripe.com/currencies — currencies are two-decimal unless listed otherwise.
    /// </summary>
    public static class StripeCurrency
    {
        /// <summary>
        /// Stripe's zero-decimal currencies. ISK and UGX are deliberately absent: although zero-decimal, Stripe's API
        /// still represents them as two-decimal values (5 ISK is sent as 500), so they use the two-decimal default.
        /// </summary>
        private static readonly HashSet<string> ZeroDecimal = new(StringComparer.OrdinalIgnoreCase)
        {
            "bif", "clp", "djf", "gnf", "jpy", "kmf", "krw", "mga", "pyg", "rwf", "vnd", "vuv", "xaf", "xof", "xpf"
        };

        /// <summary>Currencies whose minor unit is a thousandth (ISO 4217 exponent 3).</summary>
        private static readonly HashSet<string> ThreeDecimal = new(StringComparer.OrdinalIgnoreCase)
        {
            "bhd", "jod", "kwd", "omr", "tnd"
        };

        /// <summary>The number of decimal places Stripe's API uses for <paramref name="currency"/> amounts.</summary>
        public static int MinorUnitDigits(string currency)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(currency);
            if (ZeroDecimal.Contains(currency)) return 0;
            if (ThreeDecimal.Contains(currency)) return 3;
            return 2;
        }

        /// <summary>Converts a Stripe amount (e.g. 1099 USD cents) to major units (10.99).</summary>
        public static decimal ToMajorUnits(long minorUnits, string currency)
            => minorUnits / Factor(currency);

        /// <summary>
        /// Converts major units (10.99) to a Stripe amount (1099).
        /// Throws when <paramref name="amount"/> has more decimal places than the currency allows: rounding money must be a deliberate choice by the caller.
        /// </summary>
        public static long ToMinorUnits(decimal amount, string currency)
        {
            var scaled = amount * Factor(currency);
            if (scaled != decimal.Truncate(scaled))
            {
                throw new ArgumentException($"{amount} has more decimal places than {currency.ToUpperInvariant()} allows ({MinorUnitDigits(currency)}).", nameof(amount));
            }

            return (long)scaled;
        }

        private static decimal Factor(string currency) => MinorUnitDigits(currency) switch
        {
            0 => 1m,
            3 => 1000m,
            _ => 100m
        };
    }
}
