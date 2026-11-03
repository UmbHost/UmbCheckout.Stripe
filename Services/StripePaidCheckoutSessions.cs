using System.Data.Common;
using UmbCheckout.Stripe.Interfaces;
using UmbCheckout.Stripe.Pocos;
using Umbraco.Cms.Infrastructure.Scoping;

namespace UmbCheckout.Stripe.Services
{
    /// <inheritdoc />
    internal sealed class StripePaidCheckoutSessions : IStripePaidCheckoutSessions
    {
        /// <summary>A customer returning after this long has long since lost the session basket; cookies last the configured days.</summary>
        private static readonly TimeSpan Retention = TimeSpan.FromDays(30);

        private const string Table = UmbCheckoutStripePaidSession.TableName;

        private readonly IScopeProvider _scopeProvider;

        public StripePaidCheckoutSessions(IScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }

        /// <inheritdoc />
        public async Task MarkPaidAsync(string checkoutSessionId, CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            using (var scope = _scopeProvider.CreateScope(autoComplete: true))
            {
                await scope.Database.ExecuteAsync($"DELETE FROM {Table} WHERE PaidUtc < @0", now - Retention);
                if (await scope.Database.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM {Table} WHERE CheckoutSessionId = @0", checkoutSessionId) > 0)
                {
                    return;
                }
            }

            await InsertOrConfirmAsync(checkoutSessionId, now);
        }

        /// <summary>
        /// Inserts the row. When a concurrent delivery (completed and async_payment_succeeded together) inserted it between the
        /// check and the insert, the primary key refuses the second row and the session is already recorded. Any other save
        /// failure surfaces, so the webhook answers 500 and Stripe retries.
        /// </summary>
        internal async Task InsertOrConfirmAsync(string checkoutSessionId, DateTime now)
        {
            try
            {
                using var scope = _scopeProvider.CreateScope(autoComplete: true);
                await scope.Database.InsertAsync(new UmbCheckoutStripePaidSession { CheckoutSessionId = checkoutSessionId, PaidUtc = now });
            }
            catch (DbException)
            {
                // A fresh scope: the failed one is finished.
                if (!await IsPaidAsync(checkoutSessionId))
                {
                    throw;
                }
            }
        }

        /// <inheritdoc />
        public async Task<bool> IsPaidAsync(string checkoutSessionId, CancellationToken cancellationToken = default)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            return await scope.Database.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM {Table} WHERE CheckoutSessionId = @0", checkoutSessionId) > 0;
        }
    }
}
