using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UmbCheckout.Stripe.Pocos;

namespace UmbCheckout.Stripe.Persistence;

/// <summary>Handled Stripe webhook event ids, kept for 30 days (Stripe retries a delivery for up to 3 days).</summary>
internal sealed class UmbCheckoutStripeWebhookEventConfiguration : IEntityTypeConfiguration<UmbCheckoutStripeWebhookEvent>
{
    public void Configure(EntityTypeBuilder<UmbCheckoutStripeWebhookEvent> entity)
    {
        entity.ToTable("UmbCheckoutStripeWebhookEvents");
        entity.HasKey(x => x.EventId).HasName("PK_UmbCheckoutStripeWebhookEvents");
        entity.Property(x => x.EventId).HasMaxLength(255).ValueGeneratedNever();
        entity.Property(x => x.Status).IsRequired();
        entity.Property(x => x.LeaseUntilUtc).IsRequired(false);
        entity.Property(x => x.LeaseToken).HasMaxLength(64).IsRequired(false);
        entity.HasIndex(x => x.ReceivedUtc).HasDatabaseName("IX_UmbCheckoutStripeWebhookEvents_ReceivedUtc");
    }
}
