using System.Diagnostics.CodeAnalysis;

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

        /// <summary>
        /// Zero-decimal currencies Stripe still represents as two-decimal values (5 ISK is sent as 500).
        /// Docs: "You can't charge fractions of ISK" / UGX likewise.
        /// </summary>
        private static readonly HashSet<string> WholeUnitsOnly = new(StringComparer.OrdinalIgnoreCase)
        {
            "isk", "ugx"
        };

        /// <summary>The number of decimal places a charge in <paramref name="currency"/> may have.</summary>
        public static int ChargeableDigits(string currency)
            => WholeUnitsOnly.Contains(currency) ? 0 : MinorUnitDigits(currency);

        /// <summary>
        /// UmbCheckout's single rounding policy: round to the currency's chargeable digits, half away from zero.
        /// </summary>
        public static decimal Round(decimal amount, string currency)
            => Math.Round(amount, ChargeableDigits(currency), MidpointRounding.AwayFromZero);

        /// <summary>
        /// Stripe's supported presentment currencies: the union of every country's list on https://docs.stripe.com/currencies (taken 2026-10-12).
        /// </summary>
        private static readonly HashSet<string> Supported = new(StringComparer.OrdinalIgnoreCase)
        {
            "aed", "afn", "all", "amd", "ang", "aoa", "ars", "aud", "awg", "azn", "bam", "bbd", "bdt", "bhd", "bif", "bmd", "bnd", "bob", "brl", "bsd",
            "bwp", "byn", "bzd", "cad", "cdf", "chf", "clp", "cny", "cop", "crc", "cve", "czk", "djf", "dkk", "dop", "dzd", "egp", "etb", "eur", "fjd",
            "fkp", "gbp", "gel", "gip", "gmd", "gnf", "gtq", "gyd", "hkd", "hnl", "htg", "huf", "idr", "ils", "inr", "isk", "jmd", "jod", "jpy", "kes",
            "kgs", "khr", "kmf", "krw", "kwd", "kyd", "kzt", "lak", "lbp", "lkr", "lrd", "lsl", "mad", "mdl", "mga", "mkd", "mmk", "mnt", "mop", "mur",
            "mvr", "mwk", "mxn", "myr", "mzn", "nad", "ngn", "nio", "nok", "npr", "nzd", "omr", "pab", "pen", "pgk", "php", "pkr", "pln", "pyg", "qar",
            "ron", "rsd", "rub", "rwf", "sar", "sbd", "scr", "sek", "sgd", "shp", "sle", "sos", "srd", "std", "szl", "thb", "tjs", "tnd", "top", "try",
            "ttd", "twd", "tzs", "uah", "ugx", "usd", "uyu", "uzs", "vnd", "vuv", "wst", "xaf", "xcd", "xcg", "xof", "xpf", "yer", "zar", "zmw"
        };

        /// <summary>True when <paramref name="currency"/> is shaped like an ISO 4217 code (three ASCII letters).</summary>
        public static bool IsCurrencyCode([NotNullWhen(true)] string? currency)
            => currency is { Length: 3 } && currency.All(char.IsAsciiLetter);

        /// <summary>True when Stripe can charge in <paramref name="currency"/> (so a typo such as "GPB" never reaches Stripe).</summary>
        public static bool IsSupported([NotNullWhen(true)] string? currency)
            => IsCurrencyCode(currency) && Supported.Contains(currency);

        /// <summary>The code as Stripe's API expects it (lowercase). Throws for anything Stripe cannot charge in.</summary>
        public static string ApiCode(string currency)
        {
            if (!IsSupported(currency))
            {
                throw new ArgumentException($"'{currency}' is not a currency Stripe supports (three-letter ISO 4217 code).", nameof(currency));
            }

            return currency.ToLowerInvariant();
        }

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
        /// Throws when <paramref name="amount"/> has more decimal places than the currency can be charged in: rounding money must be a deliberate choice by the caller (see <see cref="Round"/>).
        /// </summary>
        public static long ToMinorUnits(decimal amount, string currency)
        {
            if (Math.Round(amount, ChargeableDigits(currency)) != amount)
            {
                throw new ArgumentException($"{amount} has more decimal places than {currency.ToUpperInvariant()} can be charged in ({ChargeableDigits(currency)}).", nameof(amount));
            }

            return (long)(amount * Factor(currency));
        }

        private static decimal Factor(string currency) => MinorUnitDigits(currency) switch
        {
            0 => 1m,
            3 => 1000m,
            _ => 100m
        };
    }
}
