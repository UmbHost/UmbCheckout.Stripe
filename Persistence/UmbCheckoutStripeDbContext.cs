using Microsoft.EntityFrameworkCore;
using UmbCheckout.Stripe.Pocos;

namespace UmbCheckout.Stripe.Persistence;

/// <summary>UmbCheckout's Stripe settings and shipping rate tables, on Umbraco's database.</summary>
public sealed class UmbCheckoutStripeDbContext(DbContextOptions<UmbCheckoutStripeDbContext> options) : DbContext(options)
{
    public const string HistoryTable = "__UmbCheckoutStripeMigrations";

    internal DbSet<UmbCheckoutStripeSettings> StripeSettings => Set<UmbCheckoutStripeSettings>();

    internal DbSet<UmbCheckoutStripeShipping> ShippingRates => Set<UmbCheckoutStripeShipping>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var isSqlite = Database.IsSqlite();
        modelBuilder.ApplyConfiguration(new UmbCheckoutStripeSettingsConfiguration(isSqlite));
        modelBuilder.ApplyConfiguration(new UmbCheckoutStripeShippingConfiguration(isSqlite));
    }
}
