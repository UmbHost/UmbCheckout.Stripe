using Microsoft.EntityFrameworkCore;
using UmbCheckout.Stripe.Interfaces;
using UmbCheckout.Stripe.Persistence;
using UmbCheckout.Stripe.Pocos;
using Umbraco.Cms.Persistence.EFCore.Scoping;

namespace UmbCheckout.Stripe.Services
{
    /// <inheritdoc />
    internal sealed class StripePaidCheckoutSessions : IStripePaidCheckoutSessions
    {
        /// <summary>A customer returning after this long has long since lost the session basket; cookies last the configured days.</summary>
        private static readonly TimeSpan Retention = TimeSpan.FromDays(30);

        private readonly IEFCoreScopeProvider<UmbCheckoutStripeDbContext> _scopeProvider;
        private readonly TimeProvider _time;

        public StripePaidCheckoutSessions(IEFCoreScopeProvider<UmbCheckoutStripeDbContext> scopeProvider)
            : this(scopeProvider, TimeProvider.System)
        {
        }

        internal StripePaidCheckoutSessions(IEFCoreScopeProvider<UmbCheckoutStripeDbContext> scopeProvider, TimeProvider time)
        {
            _scopeProvider = scopeProvider;
            _time = time;
        }

        /// <inheritdoc />
        public async Task MarkPaidAsync(string checkoutSessionId, CancellationToken cancellationToken = default)
        {
            try
            {
                using var scope = _scopeProvider.CreateScope();
                await scope.ExecuteWithContextAsync<Task>(async db =>
                {
                    var now = _time.GetUtcNow().UtcDateTime;
                    await db.PaidSessions.Where(x => x.PaidUtc < now - Retention).ExecuteDeleteAsync(cancellationToken);
                    if (!await db.PaidSessions.AnyAsync(x => x.CheckoutSessionId == checkoutSessionId, cancellationToken))
                    {
                        db.PaidSessions.Add(new UmbCheckoutStripePaidSession { CheckoutSessionId = checkoutSessionId, PaidUtc = now });
                        await db.SaveChangesAsync(cancellationToken);
                    }
                });
                scope.Complete();
            }
            catch (DbUpdateException)
            {
                // Recorded by a concurrent delivery (completed and async_payment_succeeded together): already done. Any other save
                // failure surfaces, so the webhook answers 500 and Stripe retries.
                if (!await IsPaidAsync(checkoutSessionId, cancellationToken))
                {
                    throw;
                }
            }
        }

        /// <inheritdoc />
        public async Task<bool> IsPaidAsync(string checkoutSessionId, CancellationToken cancellationToken = default)
        {
            using var scope = _scopeProvider.CreateScope();
            var paid = await scope.ExecuteWithContextAsync(db => db.PaidSessions.AnyAsync(x => x.CheckoutSessionId == checkoutSessionId, cancellationToken));
            scope.Complete();
            return paid;
        }
    }
}
