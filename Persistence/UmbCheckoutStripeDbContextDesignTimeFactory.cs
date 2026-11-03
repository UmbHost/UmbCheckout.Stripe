using Microsoft.EntityFrameworkCore.Design;
using UmbCheckout.Shared.Persistence;

namespace UmbCheckout.Stripe.Persistence;

internal sealed class UmbCheckoutStripeDbContextDesignTimeFactory : IDesignTimeDbContextFactory<UmbCheckoutStripeDbContext>
{
    public UmbCheckoutStripeDbContext CreateDbContext(string[] args) => new(DesignTimeOptions.For<UmbCheckoutStripeDbContext>(UmbCheckoutStripeDbContext.HistoryTable));
}
