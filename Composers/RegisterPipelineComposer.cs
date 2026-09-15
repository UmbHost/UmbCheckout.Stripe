using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using UmbCheckout.Stripe.Middleware;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Web.Common.ApplicationBuilder;

namespace UmbCheckout.Stripe.Composers
{
    /// <summary>
    /// Registers the Stripe middleware in the Umbraco pipeline
    /// </summary>
    internal class RegisterPipelineComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.Configure<UmbracoPipelineOptions>(options =>
            {
                options.AddFilter(new UmbracoPipelineFilter($"{Shared.Consts.PackageName}.{Consts.AppSettingsSectionName}")
                {
                    // PostPipeline runs after Umbraco's default middleware (including UseSession)
                    PostPipeline = app => app.UseMiddleware<StripeCheckoutReturnMiddleware>()
                });
            });
        }
    }
}
