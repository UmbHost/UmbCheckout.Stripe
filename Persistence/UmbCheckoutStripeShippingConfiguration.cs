using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UmbCheckout.Stripe.Pocos;

namespace UmbCheckout.Stripe.Persistence;

/// <summary>Maps a shipping rate row onto the table NPoco created before v17, column for column (see the legacy fixtures).</summary>
internal sealed class UmbCheckoutStripeShippingConfiguration(bool isSqlite) : IEntityTypeConfiguration<UmbCheckoutStripeShipping>
{
    public void Configure(EntityTypeBuilder<UmbCheckoutStripeShipping> entity)
    {
        entity.ToTable("UmbCheckoutStripeShipping");
        entity.HasKey(x => x.Id).HasName("PK_UmbCheckoutStripeShipping");
        entity.Property(x => x.Id).ValueGeneratedOnAdd();
        entity.Property(x => x.Key).IsRequired(false);
        entity.Property(x => x.Name).HasMaxLength(255).IsRequired();
        entity.Property(x => x.Value).HasMaxLength(255).IsRequired();
        var keyIndex = entity.HasIndex(x => x.Key).IsUnique().HasDatabaseName("IX_shippingKey");
        entity.HasIndex(x => x.Value).IsUnique().HasDatabaseName("IX_stripeIdShippingRate");
        if (isSqlite)
        {
            entity.Property(x => x.Name).UseCollation("NOCASE");
            entity.Property(x => x.Value).UseCollation("NOCASE");
        }
        else
        {
            // The NPoco index has no filter (EF would add "[Key] IS NOT NULL" to a unique index on a nullable column).
            keyIndex.HasFilter(null);
        }
    }
}
