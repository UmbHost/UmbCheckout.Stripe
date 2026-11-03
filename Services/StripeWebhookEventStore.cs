using System.Data.Common;
using UmbCheckout.Stripe.Interfaces;
using UmbCheckout.Stripe.Pocos;
using Umbraco.Cms.Infrastructure.Scoping;

namespace UmbCheckout.Stripe.Services
{
    /// <inheritdoc />
    internal sealed class StripeWebhookEventStore : IStripeWebhookEventStore
    {
        /// <summary>Well past Stripe's 3-day retry window.</summary>
        private static readonly TimeSpan Retention = TimeSpan.FromDays(30);

        /// <summary>How long a delivery owns an event before another may take it over (well past Stripe's request timeout).</summary>
        private static readonly TimeSpan DefaultLease = TimeSpan.FromMinutes(10);

        private const string Table = UmbCheckoutStripeWebhookEvent.TableName;

        private readonly IScopeProvider _scopeProvider;
        private readonly TimeSpan _lease;

        public StripeWebhookEventStore(IScopeProvider scopeProvider)
            : this(scopeProvider, DefaultLease)
        {
        }

        internal StripeWebhookEventStore(IScopeProvider scopeProvider, TimeSpan lease)
        {
            _scopeProvider = scopeProvider;
            _lease = lease;
        }

        /// <inheritdoc />
        public async Task<StripeWebhookEventClaimResult> TryClaimAsync(string eventId, CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            var token = Guid.NewGuid().ToString("N");
            UmbCheckoutStripeWebhookEvent? existing;
            using (var scope = _scopeProvider.CreateScope(autoComplete: true))
            {
                await scope.Database.ExecuteAsync($"DELETE FROM {Table} WHERE ReceivedUtc < @0", now - Retention);
                existing = await scope.Database.SingleOrDefaultAsync<UmbCheckoutStripeWebhookEvent>($"SELECT * FROM {Table} WHERE EventId = @0", eventId);
                if (existing != null)
                {
                    if (existing.Status == UmbCheckoutStripeWebhookEvent.Done)
                    {
                        return StripeWebhookEventClaimResult.Of(StripeWebhookEventClaim.AlreadyHandled);
                    }

                    // Take over an attempt whose lease has expired; the conditional update lets only one delivery win, and the new
                    // token stops the earlier attempt completing or releasing this delivery's claim.
                    var takenOver = await scope.Database.ExecuteAsync(
                        $"UPDATE {Table} SET LeaseUntilUtc = @0, LeaseToken = @1 WHERE EventId = @2 AND Status = @3 AND LeaseUntilUtc <= @4",
                        now + _lease, token, eventId, UmbCheckoutStripeWebhookEvent.Processing, now);
                    return takenOver == 1
                        ? new StripeWebhookEventClaimResult(StripeWebhookEventClaim.Claimed, token)
                        : StripeWebhookEventClaimResult.Of(StripeWebhookEventClaim.InProgress);
                }
            }

            return await InsertOrReportAsync(eventId, now, token, cancellationToken);
        }

        /// <summary>
        /// Inserts the claim. When another delivery inserted it between the check and the insert (the primary key refuses the second
        /// row), that delivery is running the handlers: in progress. Any other save failure (deadlock, timeout, lost connection)
        /// surfaces, so the webhook answers 500 and Stripe retries.
        /// </summary>
        internal async Task<StripeWebhookEventClaimResult> InsertOrReportAsync(string eventId, DateTime now, string token, CancellationToken cancellationToken)
        {
            try
            {
                using var scope = _scopeProvider.CreateScope(autoComplete: true);
                await scope.Database.InsertAsync(new UmbCheckoutStripeWebhookEvent
                {
                    EventId = eventId,
                    ReceivedUtc = now,
                    Status = UmbCheckoutStripeWebhookEvent.Processing,
                    LeaseUntilUtc = now + _lease,
                    LeaseToken = token
                });
                return new StripeWebhookEventClaimResult(StripeWebhookEventClaim.Claimed, token);
            }
            catch (DbException)
            {
                // A fresh scope: the failed one is finished.
                using var scope = _scopeProvider.CreateScope(autoComplete: true);
                var exists = await scope.Database.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM {Table} WHERE EventId = @0", eventId) > 0;
                if (exists)
                {
                    return StripeWebhookEventClaimResult.Of(StripeWebhookEventClaim.InProgress);
                }

                throw;
            }
        }

        /// <inheritdoc />
        public async Task CompleteAsync(string eventId, string leaseToken, CancellationToken cancellationToken = default)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            // Only while this delivery holds the claim: an attempt that outlived its lease leaves the takeover's claim alone.
            await scope.Database.ExecuteAsync($"UPDATE {Table} SET Status = @0, LeaseUntilUtc = NULL, LeaseToken = NULL WHERE EventId = @1 AND Status = @2 AND LeaseToken = @3",
                UmbCheckoutStripeWebhookEvent.Done, eventId, UmbCheckoutStripeWebhookEvent.Processing, leaseToken);
        }

        /// <inheritdoc />
        public async Task ForgetAsync(string eventId, string leaseToken, CancellationToken cancellationToken = default)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            // Only while this delivery holds the claim: an attempt that outlived its lease leaves the takeover's claim alone.
            await scope.Database.ExecuteAsync($"DELETE FROM {Table} WHERE EventId = @0 AND Status = @1 AND LeaseToken = @2",
                eventId, UmbCheckoutStripeWebhookEvent.Processing, leaseToken);
        }
    }
}
