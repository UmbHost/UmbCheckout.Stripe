using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UmbCheckout.Stripe.Pocos;

namespace UmbCheckout.Stripe.Persistence;

/// <summary>Checkout sessions the webhook reported paid, kept for 30 days.</summary>
internal sealed class UmbCheckoutStripePaidSessionConfiguration : IEntityTypeConfiguration<UmbCheckoutStripePaidSession>
{
    public void Configure(EntityTypeBuilder<UmbCheckoutStripePaidSession> entity)
    {
        entity.ToTable("UmbCheckoutStripePaidSessions");
        entity.HasKey(x => x.CheckoutSessionId).HasName("PK_UmbCheckoutStripePaidSessions");
        entity.Property(x => x.CheckoutSessionId).HasMaxLength(255).ValueGeneratedNever();
        entity.HasIndex(x => x.PaidUtc).HasDatabaseName("IX_UmbCheckoutStripePaidSessions_PaidUtc");
    }
}
