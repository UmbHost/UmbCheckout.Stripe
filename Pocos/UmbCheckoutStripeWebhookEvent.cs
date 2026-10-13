using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace UmbCheckout.Stripe.Pocos
{
    /// <summary>A Stripe webhook event being handled (with a lease) or handled, so a redelivery is recognised. Kept for 30 days.</summary>
    [TableName(TableName)]
    [PrimaryKey("EventId", AutoIncrement = false)]
    [ExplicitColumns]
    internal sealed class UmbCheckoutStripeWebhookEvent
    {
        public const string TableName = "UmbCheckoutStripeWebhookEvents";
        public const int Processing = 0;
        public const int Done = 1;

        [Column("EventId")]
        [PrimaryKeyColumn(AutoIncrement = false, Name = "PK_UmbCheckoutStripeWebhookEvents")]
        [Length(255)]
        public string EventId { get; set; } = string.Empty;

        [Column("ReceivedUtc")]
        [Index(IndexTypes.NonClustered, Name = "IX_UmbCheckoutStripeWebhookEvents_ReceivedUtc")]
        public DateTime ReceivedUtc { get; set; }

        /// <summary><see cref="Processing"/> or <see cref="Done"/>.</summary>
        [Column("Status")]
        public int Status { get; set; }

        /// <summary>While processing: when another delivery may take the event over (the first attempt is presumed dead).</summary>
        [Column("LeaseUntilUtc")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public DateTime? LeaseUntilUtc { get; set; }
    }
}
