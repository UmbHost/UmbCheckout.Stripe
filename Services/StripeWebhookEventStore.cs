using Microsoft.EntityFrameworkCore;
using UmbCheckout.Stripe.Interfaces;
using UmbCheckout.Stripe.Persistence;
using UmbCheckout.Stripe.Pocos;
using Umbraco.Cms.Persistence.EFCore.Scoping;

namespace UmbCheckout.Stripe.Services
{
    /// <inheritdoc />
    internal sealed class StripeWebhookEventStore(IEFCoreScopeProvider<UmbCheckoutStripeDbContext> scopeProvider) : IStripeWebhookEventStore
    {
        /// <summary>Well past Stripe's 3-day retry window.</summary>
        private static readonly TimeSpan Retention = TimeSpan.FromDays(30);

        /// <inheritdoc />
        public async Task<bool> TryBeginAsync(string eventId, CancellationToken cancellationToken = default)
        {
            using var scope = scopeProvider.CreateScope();
            bool begun;
            try
            {
                begun = await scope.ExecuteWithContextAsync(async db =>
                {
                    var cutoff = DateTime.UtcNow - Retention;
                    await db.WebhookEvents.Where(x => x.ReceivedUtc < cutoff).ExecuteDeleteAsync(cancellationToken);
                    if (await db.WebhookEvents.AnyAsync(x => x.EventId == eventId, cancellationToken))
                    {
                        return false;
                    }

                    db.WebhookEvents.Add(new UmbCheckoutStripeWebhookEvent { EventId = eventId, ReceivedUtc = DateTime.UtcNow });
                    await db.SaveChangesAsync(cancellationToken);
                    return true;
                });
            }
            catch (DbUpdateException)
            {
                // Another server (or a concurrent delivery) recorded it between the check and the insert.
                return false;
            }

            scope.Complete();
            return begun;
        }

        /// <inheritdoc />
        public async Task ForgetAsync(string eventId, CancellationToken cancellationToken = default)
        {
            using var scope = scopeProvider.CreateScope();
            await scope.ExecuteWithContextAsync(db => db.WebhookEvents.Where(x => x.EventId == eventId).ExecuteDeleteAsync(cancellationToken));
            scope.Complete();
        }
    }
}
