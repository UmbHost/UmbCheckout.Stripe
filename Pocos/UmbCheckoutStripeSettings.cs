using NPoco;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
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
    }
}
