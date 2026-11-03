using UmbCheckout.Stripe.Money;

namespace UmbCheckout.Stripe.Models.Api
{
    /// <summary>
    /// A monetary amount with its currency. <see cref="MinorUnits"/> is Stripe's integer amount; <see cref="Amount"/> is the
    /// same value in major units, converted once on the server so clients never divide by a hard-coded 100.
    /// </summary>
    public class MoneyResponseModel
    {
        /// <summary>Stripe's amount, in the currency's minor unit (495 for £4.95, 500 for ¥500).</summary>
        public long MinorUnits { get; set; }

        /// <summary>The amount in major units (4.95), or null when there is no currency to convert with (never a guess).</summary>
        public decimal? Amount { get; set; }

        /// <summary>The currency code as Stripe gave it, or empty when there was none.</summary>
        public string Currency { get; set; } = string.Empty;

        /// <summary>
        /// The money object for a Stripe amount. A null, empty or white-space <paramref name="currency"/> doesn't throw: the minor
        /// units are kept and <see cref="Amount"/> is null, so a client shows the raw value rather than a wrong conversion.
        /// </summary>
        public static MoneyResponseModel From(long minorUnits, string? currency)
        {
            if (string.IsNullOrWhiteSpace(currency))
            {
                return new MoneyResponseModel { MinorUnits = minorUnits };
            }

            return new MoneyResponseModel
            {
                MinorUnits = minorUnits,
                Amount = StripeCurrency.ToMajorUnits(minorUnits, currency),
                Currency = currency
            };
        }
    }
}
