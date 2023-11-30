using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace UmbCheckout.Stripe.Pocos
{
    [TableName("UmbCheckoutStripeSettings")]
    [PrimaryKey("Id", AutoIncrement = true)]
    [ExplicitColumns]
    internal class UmbCheckoutStripeSettings
    {
        [PrimaryKeyColumn(AutoIncrement = true, IdentitySeed = 1)]
        [Column("Id")]
        public long Id { get; set; }

        [Column("Key")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public Guid Key { get; set; } = Guid.NewGuid();

        [Column("UseLiveApiDetails")]
        [NullSetting(NullSetting = NullSettings.NotNull)]
        public bool UseLiveApiDetails { get; set; } = false;

        [Column("CollectPhoneNumber")]
        [NullSetting(NullSetting = NullSettings.NotNull)]
        public bool CollectPhoneNumber { get; set; } = false;

        [Column("ShippingAllowedCountries")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string? ShippingAllowedCountries { get; set; } = null;
    }
}
