using Stripe.Checkout;
using UmbCheckout.Stripe.Interfaces;

namespace UmbCheckout.Stripe.Models;

/// <summary>
/// Wrapper for the <see cref="SessionGetOptions"/> used to specify options when calling <see cref="IStripeSessionService.GetSession"/>>
/// </summary>
public class UmbCheckoutSessionGetOptions
{
    /// <summary>
    /// List of Property names in the <see cref="Session"/> to be expanded when calling <see cref="IStripeSessionService.GetSession"/>
    /// Reference https://docs.stripe.com/api/expanding_objects
    /// </summary>
    public List<string>? Expand { get; set; }

    public SessionGetOptions ToSessionGetOptions()
    {
        return new SessionGetOptions
        {
            Expand = Expand
        };
    }
}