using Microsoft.EntityFrameworkCore;
using UmbCheckout.Stripe.Interfaces;
using UmbCheckout.Stripe.Persistence;
using UmbCheckout.Stripe.Pocos;
using Umbraco.Cms.Persistence.EFCore.Scoping;

namespace UmbCheckout.Stripe.Services
{
    /// <inheritdoc />
    internal sealed class StripeWebhookEventStore : IStripeWebhookEventStore
    {
        /// <summary>Well past Stripe's 3-day retry window.</summary>
        private static readonly TimeSpan Retention = TimeSpan.FromDays(30);

        /// <summary>How long a delivery owns an event before another may take it over (well past Stripe's request timeout).</summary>
        private static readonly TimeSpan DefaultLease = TimeSpan.FromMinutes(10);

        private readonly IEFCoreScopeProvider<UmbCheckoutStripeDbContext> _scopeProvider;
        private readonly TimeSpan _lease;

        public StripeWebhookEventStore(IEFCoreScopeProvider<UmbCheckoutStripeDbContext> scopeProvider)
            : this(scopeProvider, DefaultLease)
        {
        }

        internal StripeWebhookEventStore(IEFCoreScopeProvider<UmbCheckoutStripeDbContext> scopeProvider, TimeSpan lease)
        {
            _scopeProvider = scopeProvider;
            _lease = lease;
        }

        /// <inheritdoc />
        public async Task<StripeWebhookEventClaim> TryClaimAsync(string eventId, CancellationToken cancellationToken = default) =>
            (await ClaimAsync(eventId, cancellationToken)).Claim;

        /// <inheritdoc />
        public async Task<StripeWebhookEventClaimResult> ClaimAsync(string eventId, CancellationToken cancellationToken = default)
        {
            var token = Guid.NewGuid().ToString("N");
            try
            {
                using var scope = _scopeProvider.CreateScope();
                var claim = await scope.ExecuteWithContextAsync(async db =>
                {
                    var now = DateTime.UtcNow;
                    await db.WebhookEvents.Where(x => x.ReceivedUtc < now - Retention).ExecuteDeleteAsync(cancellationToken);

                    var existing = await db.WebhookEvents.AsNoTracking().SingleOrDefaultAsync(x => x.EventId == eventId, cancellationToken);
                    if (existing == null)
                    {
                        db.WebhookEvents.Add(new UmbCheckoutStripeWebhookEvent
                        {
                            EventId = eventId, ReceivedUtc = now, Status = UmbCheckoutStripeWebhookEvent.Processing, LeaseUntilUtc = now + _lease, LeaseToken = token
                        });
                        await db.SaveChangesAsync(cancellationToken);
                        return new StripeWebhookEventClaimResult(StripeWebhookEventClaim.Claimed, token);
                    }

                    if (existing.Status == UmbCheckoutStripeWebhookEvent.Done)
                    {
                        return StripeWebhookEventClaimResult.Of(StripeWebhookEventClaim.AlreadyHandled);
                    }

                    // Take over an attempt whose lease has expired; the conditional update lets only one delivery win, and the new
                    // token stops the earlier attempt completing or releasing this delivery's claim.
                    var takenOver = await db.WebhookEvents
                        .Where(x => x.EventId == eventId && x.Status == UmbCheckoutStripeWebhookEvent.Processing && x.LeaseUntilUtc <= now)
                        .ExecuteUpdateAsync(set => set.SetProperty(x => x.LeaseUntilUtc, now + _lease).SetProperty(x => x.LeaseToken, token), cancellationToken);
                    return takenOver == 1
                        ? new StripeWebhookEventClaimResult(StripeWebhookEventClaim.Claimed, token)
                        : StripeWebhookEventClaimResult.Of(StripeWebhookEventClaim.InProgress);
                });
                scope.Complete();
                return claim;
            }
            catch (DbUpdateException)
            {
                // Another delivery inserted it between the check and the insert: it is running the handlers now. Any other
                // save failure (deadlock, timeout, lost connection) surfaces, so the webhook answers 500 and Stripe retries.
                if (await ExistsAsync(eventId, cancellationToken))
                {
                    return StripeWebhookEventClaimResult.Of(StripeWebhookEventClaim.InProgress);
                }

                throw;
            }
        }

        /// <inheritdoc />
        public async Task CompleteAsync(string eventId, CancellationToken cancellationToken = default)
        {
            using var scope = _scopeProvider.CreateScope();
            await scope.ExecuteWithContextAsync(db => db.WebhookEvents.Where(x => x.EventId == eventId)
                .ExecuteUpdateAsync(set => set.SetProperty(x => x.Status, UmbCheckoutStripeWebhookEvent.Done).SetProperty(x => x.LeaseUntilUtc, (DateTime?)null).SetProperty(x => x.LeaseToken, (string?)null), cancellationToken));
            scope.Complete();
        }

        /// <inheritdoc />
        public async Task CompleteAsync(string eventId, string leaseToken, CancellationToken cancellationToken = default)
        {
            using var scope = _scopeProvider.CreateScope();
            // Only while this delivery holds the claim: an attempt that outlived its lease leaves the takeover's claim alone.
            await scope.ExecuteWithContextAsync(db => db.WebhookEvents
                .Where(x => x.EventId == eventId && x.Status == UmbCheckoutStripeWebhookEvent.Processing && x.LeaseToken == leaseToken)
                .ExecuteUpdateAsync(set => set.SetProperty(x => x.Status, UmbCheckoutStripeWebhookEvent.Done).SetProperty(x => x.LeaseUntilUtc, (DateTime?)null).SetProperty(x => x.LeaseToken, (string?)null), cancellationToken));
            scope.Complete();
        }

        /// <inheritdoc />
        public async Task ForgetAsync(string eventId, CancellationToken cancellationToken = default)
        {
            using var scope = _scopeProvider.CreateScope();
            await scope.ExecuteWithContextAsync(db => db.WebhookEvents.Where(x => x.EventId == eventId).ExecuteDeleteAsync(cancellationToken));
            scope.Complete();
        }

        /// <inheritdoc />
        public async Task ForgetAsync(string eventId, string leaseToken, CancellationToken cancellationToken = default)
        {
            using var scope = _scopeProvider.CreateScope();
            // Only while this delivery holds the claim: an attempt that outlived its lease leaves the takeover's claim alone.
            await scope.ExecuteWithContextAsync(db => db.WebhookEvents
                .Where(x => x.EventId == eventId && x.Status == UmbCheckoutStripeWebhookEvent.Processing && x.LeaseToken == leaseToken)
                .ExecuteDeleteAsync(cancellationToken));
            scope.Complete();
        }

        /// <summary>A fresh scope: the failed one has been rolled back.</summary>
        private async Task<bool> ExistsAsync(string eventId, CancellationToken cancellationToken)
        {
            using var scope = _scopeProvider.CreateScope();
            var exists = await scope.ExecuteWithContextAsync(db => db.WebhookEvents.AnyAsync(x => x.EventId == eventId, cancellationToken));
            scope.Complete();
            return exists;
        }
    }
}
