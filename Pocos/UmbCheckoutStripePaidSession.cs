using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace UmbCheckout.Stripe.Pocos
{
    /// <summary>A Stripe checkout session the webhook reported paid: the next basket read clears the basket it paid for. Kept 30 days.</summary>
    [TableName(TableName)]
    [PrimaryKey("CheckoutSessionId", AutoIncrement = false)]
    [ExplicitColumns]
    internal sealed class UmbCheckoutStripePaidSession
    {
        public const string TableName = "UmbCheckoutStripePaidSessions";

        [Column("CheckoutSessionId")]
        [PrimaryKeyColumn(AutoIncrement = false, Name = "PK_UmbCheckoutStripePaidSessions")]
        [Length(255)]
        public string CheckoutSessionId { get; set; } = string.Empty;

        [Column("PaidUtc")]
        [Index(IndexTypes.NonClustered, Name = "IX_UmbCheckoutStripePaidSessions_PaidUtc")]
        public DateTime PaidUtc { get; set; }
    }
}
