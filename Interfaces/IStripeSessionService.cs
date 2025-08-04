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
        Session GetSession(string id, UmbCheckoutSessionGetOptions? options = null);

        /// <summary>
        /// Gets a Stripe Session asynchronously
        /// </summary>
        /// <param name="id">Id of the Stripe Session</param>
        /// <returns>The Stripe Session</returns>
        Task<Session> GetSessionAsync(string id);

        /// <summary>
        /// Creates a Stripe Session
        /// </summary>
        /// <param name="basket">The basket to be stored in the Stripe Session</param>
        /// <returns>The Stripe Session</returns>
        Session CreateSession(Basket basket);

        /// <summary>
        /// Creates a Stripe Session asynchronously
        /// </summary>
        /// <param name="basket">The basket to be stored in the Stripe Session</param>
        /// <returns>The Stripe Session</returns>
        Task<Session> CreateSessionAsync(Basket basket);

        /// <summary>
        /// Clears a Stripe Session
        /// </summary>
        /// <param name="id">Id of the Stripe Session</param>
        void ClearSession(string id);

        /// <summary>
        /// Clears a Stripe Session asynchronously
        /// </summary>
        /// <param name="id">Id of the Stripe Session</param>
        Task ClearSessionAsync(string id);
    }
}