using UmbCheckout.Shared;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Manifest;

namespace UmbCheckout.Stripe
{
    public class UmbCheckoutStripeManifest : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.ManifestFilters().Append<UmbCheckoutStripeManifestFilter>();
        }
    }

    public class UmbCheckoutStripeManifestFilter : IManifestFilter
    {
        public void Filter(List<PackageManifest> manifests)
        {
            manifests.Add(new PackageManifest
            {
                PackageName = $"{Shared.Consts.PackageName}.{Consts.AppSettingsSectionName}",
                Version = UmbCheckoutVersion.Version.ToString(3),
                AllowPackageTelemetry = true,
                BundleOptions = BundleOptions.None
            });
        }
    }
}
