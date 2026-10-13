using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Web.Common.ApplicationBuilder;

namespace UmbCheckout.Stripe.Composers
{
    /// <summary>
    /// Routes the Stripe webhook (an attribute-routed controller) on every host. In Umbraco 17 only the Management API and the
    /// Delivery API map attribute-routed controllers, so a front-end-only host (the public servers of a load-balanced shop) would
    /// answer Stripe with 404. Mapping controllers again on a full host reuses the same endpoints, so nothing is registered twice.
    /// </summary>
    public class StripeWebhookRoutingComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
            => builder.Services.Configure<UmbracoPipelineOptions>(options => options.AddFilter(new UmbracoPipelineFilter(
                "UmbCheckout.Stripe.Webhook",
                endpoints: app => app.UseEndpoints(endpoints => endpoints.MapControllers()))));
    }
}
