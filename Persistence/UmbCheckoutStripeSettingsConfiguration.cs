using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UmbCheckout.Stripe.Pocos;

namespace UmbCheckout.Stripe.Persistence;

/// <summary>Maps the Stripe settings row onto the table NPoco created before v17, column for column (see the legacy fixtures).</summary>
internal sealed class UmbCheckoutStripeSettingsConfiguration(bool isSqlite) : IEntityTypeConfiguration<UmbCheckoutStripeSettings>
{
    public void Configure(EntityTypeBuilder<UmbCheckoutStripeSettings> entity)
    {
        entity.ToTable("UmbCheckoutStripeSettings");
        entity.HasKey(x => x.Id).HasName("PK_UmbCheckoutStripeSettings");
        entity.Property(x => x.Id).ValueGeneratedOnAdd();
        entity.Property(x => x.Key).IsRequired(false);
        entity.Property(x => x.ShippingAllowedCountries).HasMaxLength(255).IsRequired(false);
        if (isSqlite)
        {
            entity.Property(x => x.ShippingAllowedCountries).UseCollation("NOCASE");
        }
    }
}
