using Microsoft.Extensions.DependencyInjection;
using UmbCheckout.Stripe.Models;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace UmbCheckout.Stripe.Composers
{
    public class AddAppSettingsComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.Configure<StripeSettings>(builder.Config.GetSection(Shared.Consts.PackageName).GetSection(Consts.AppSettingsSectionName));
        }
    }
}
