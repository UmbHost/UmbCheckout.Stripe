using UmbCheckout.Shared;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Manifest;

#if NET9_0
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Infrastructure.Manifest;
#endif

namespace UmbCheckout.Stripe
{
    public class UmbCheckoutStripeManifest : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
#if NET8_0
            builder.ManifestFilters().Append<UmbCheckoutStripeManifestFilter>();
#endif

#if NET9_0
            builder.Services.AddSingleton<IPackageManifestReader, UmbCheckoutStripeReader>();
#endif
        }
    }

#if NET8_0
    internal sealed class UmbCheckoutStripeManifestFilter : IManifestFilter
    {
        public void Filter(List<PackageManifest> manifests)
        {
            manifests.Add(new PackageManifest
            {
                PackageName = $"{Shared.Consts.PackageName}.{Consts.AppSettingsSectionName}",
                Version = UmbCheckoutVersion.Version.ToString(3),
                AllowPackageTelemetry = true,
                Scripts = new[]
                {
                    "/App_Plugins/UmbCheckout/js/umbcheckout.stripe.resources.js",
                    "/App_Plugins/UmbCheckout/js/umbcheckout.stripe.settings.controller.js",
                    "/App_Plugins/UmbCheckout/js/umbcheckout.stripe.shippingrate.controller.js",
                    "/App_Plugins/UmbCheckout/js/umbcheckout.stripe.shippingrates.controller.js"
                }
            });
        }
    }
#endif

#if NET9_0
    internal sealed class UmbCheckoutStripeReader : IPackageManifestReader
    {
        public Task<IEnumerable<PackageManifest>> ReadPackageManifestsAsync()
        {
            List<PackageManifest> manifest = [
                new()
                {
                    Id = $"{Shared.Consts.PackageName}.{Consts.AppSettingsSectionName}",
                    Name = $"{Shared.Consts.PackageName}.{Consts.AppSettingsSectionName}",
                    AllowTelemetry = true,
                    Version = UmbCheckoutVersion.Version.ToString(3),
                    Extensions = []
                }
            ];

            return Task.FromResult(manifest.AsEnumerable());
        }
    }
#endif
}