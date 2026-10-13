using Stripe.Checkout;
using UmbCheckout.Shared.Models;
using UmbCheckout.Stripe.Models;

namespace UmbCheckout.Stripe.Interfaces
{
    /// <summary>
    /// A service which handles all things around the Stripe Session
    /// </summary>
    public interface IStripeSessionService
    {
        /// <summary>
        /// Gets a Stripe Session
        /// </summary>
        /// <param name="id">Id of the Stripe Session</param>
        /// <param name="options">Optional Get Options</param>
        /// <returns>The Stripe Session</returns>
        [Obsolete("Blocks a thread on a Stripe call; use the async method with a CancellationToken. Scheduled for removal in the next major version.")]
        Session GetSession(string id, UmbCheckoutSessionGetOptions? options = null);

        /// <summary>
        /// Gets a Stripe Session asynchronously
        /// </summary>
        /// <param name="id">Id of the Stripe Session</param>
        /// <returns>The Stripe Session</returns>
        [Obsolete("Use GetSessionAsync(string, CancellationToken) (the overload with a CancellationToken). Scheduled for removal in the next major version.")]
        Task<Session> GetSessionAsync(string id);

        /// <summary>Gets a Stripe Session, stopping when <paramref name="cancellationToken"/> is cancelled.</summary>
#pragma warning disable CS0618 // forwards to the old overload so implementations written before the token overload keep working
        Task<Session> GetSessionAsync(string id, CancellationToken cancellationToken) => GetSessionAsync(id);
#pragma warning restore CS0618

        /// <summary>
        /// Creates a Stripe Session
        /// </summary>
        /// <param name="basket">The basket to be stored in the Stripe Session</param>
        /// <returns>The Stripe Session</returns>
        [Obsolete("Blocks a thread on a Stripe call; use the async method with a CancellationToken. Scheduled for removal in the next major version.")]
        Session CreateSession(Basket basket);

        /// <summary>
        /// Creates a Stripe Session asynchronously
        /// </summary>
        /// <param name="basket">The basket to be stored in the Stripe Session</param>
        /// <returns>The Stripe Session</returns>
        [Obsolete("Use CreateSessionAsync(Basket, CancellationToken) (the overload with a CancellationToken). Scheduled for removal in the next major version.")]
        Task<Session> CreateSessionAsync(Basket basket);

        /// <summary>Creates a Stripe Session, stopping when <paramref name="cancellationToken"/> is cancelled (e.g. the shopper left).</summary>
#pragma warning disable CS0618 // forwards to the old overload so implementations written before the token overload keep working
        Task<Session> CreateSessionAsync(Basket basket, CancellationToken cancellationToken) => CreateSessionAsync(basket);
#pragma warning restore CS0618

        /// <summary>
        /// Clears a Stripe Session
        /// </summary>
        /// <param name="id">Id of the Stripe Session</param>
        [Obsolete("Blocks a thread on a Stripe call; use the async method with a CancellationToken. Scheduled for removal in the next major version.")]
        void ClearSession(string id);

        /// <summary>
        /// Clears a Stripe Session asynchronously
        /// </summary>
        /// <param name="id">Id of the Stripe Session</param>
        [Obsolete("Use ClearSessionAsync(string, CancellationToken) (the overload with a CancellationToken). Scheduled for removal in the next major version.")]
        Task ClearSessionAsync(string id);

        /// <summary>Clears a Stripe Session, stopping when <paramref name="cancellationToken"/> is cancelled.</summary>
#pragma warning disable CS0618 // forwards to the old overload so implementations written before the token overload keep working
        Task ClearSessionAsync(string id, CancellationToken cancellationToken) => ClearSessionAsync(id);
#pragma warning restore CS0618
    }
}