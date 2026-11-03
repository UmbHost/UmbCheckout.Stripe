using Microsoft.Extensions.DependencyInjection;
using UmbCheckout.Core.Interfaces;
using UmbCheckout.Stripe.Interfaces;
using UmbCheckout.Stripe.NotificationHandlers;
using UmbCheckout.Stripe.Notifications.Webhooks;
using UmbCheckout.Stripe.Services;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Extensions;

namespace UmbCheckout.Stripe.Composers
{
    /// <summary>
    /// The webhook records paid checkout sessions; basket reads ask this store, so a customer who paid but never returned to the
    /// success page doesn't keep the paid basket. Replaces Core's "none paid" default whichever composer runs first, and keeps a
    /// store the site registered itself.
    /// </summary>
    public class PaidCheckoutSessionsComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddTransient<IStripePaidCheckoutSessions, StripePaidCheckoutSessions>();

            // Only Core's default is replaced: a store the site registered itself is kept (the package stays extendable).
            var registered = builder.Services.Where(d => d.ServiceType == typeof(IPaidCheckoutSessions)).ToList();
            if (registered.TrueForAll(d => d.ImplementationType == typeof(NoPaidCheckoutSessions) || d.ImplementationInstance is NoPaidCheckoutSessions))
            {
                registered.ForEach(d => builder.Services.Remove(d));
                builder.Services.AddTransient<IPaidCheckoutSessions>(sp => sp.GetRequiredService<IStripePaidCheckoutSessions>());
            }

            builder.AddNotificationAsyncHandler<OnCheckoutSessionCompletedNotification, RecordPaidCheckoutSessionHandler>();
            builder.AddNotificationAsyncHandler<OnPaymentSuccessNotification, RecordPaidCheckoutSessionHandler>();
        }
    }
}
