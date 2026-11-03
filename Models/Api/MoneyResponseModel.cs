using UmbCheckout.Stripe.Money;

namespace UmbCheckout.Stripe.Models.Api
{
    /// <summary>
    /// A monetary amount with its currency. <see cref="MinorUnits"/> is Stripe's integer amount; <see cref="Amount"/> is the
    /// same value in major units, converted once on the server so clients never divide by a hard-coded 100.
    /// </summary>
    public class MoneyResponseModel
    {
        public long MinorUnits { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;

        public static MoneyResponseModel From(long minorUnits, string currency) => new()
        {
            MinorUnits = minorUnits,
            Amount = StripeCurrency.ToMajorUnits(minorUnits, currency),
            Currency = currency
        };
    }
}
