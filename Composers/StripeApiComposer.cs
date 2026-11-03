using UmbCheckout.Core.ManagementApi;
using Umbraco.Cms.Api.Management.OpenApi;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace UmbCheckout.Stripe.Composers
{
    public class StripeApiComposer : IComposer
    {
        public const string DocumentName = "umbcheckout-stripe";

        public void Compose(IUmbracoBuilder builder)
            => builder.AddUmbCheckoutApiDocument<StripeSecurityFilter>(DocumentName, "UmbCheckout Stripe API", "UmbCheckout.Stripe", excludedNamespacePrefix: "UmbCheckout.Stripe.Addons");

        public class StripeSecurityFilter : BackOfficeSecurityRequirementsOperationFilterBase
        {
            protected override string ApiName => DocumentName;
        }
    }
}
