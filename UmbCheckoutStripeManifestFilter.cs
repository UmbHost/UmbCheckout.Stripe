using Microsoft.Extensions.DependencyInjection;
using UmbCheckout.Shared;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Manifest;
using Umbraco.Cms.Infrastructure.Manifest;

namespace UmbCheckout.Stripe
{
    public class UmbCheckoutStripeManifest : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddSingleton<IPackageManifestReader, UmbCheckoutStripeReader>();
        }
    }

    internal sealed class UmbCheckoutStripeReader : IPackageManifestReader
    {
        public Task<IEnumerable<PackageManifest>> ReadPackageManifestsAsync()
        {
            IEnumerable<PackageManifest> manifests =
            [
                new PackageManifest
                {
                    Id = $"{Shared.Consts.PackageName}.{Consts.AppSettingsSectionName}",
                    Name = $"{Shared.Consts.PackageName}.{Consts.AppSettingsSectionName}",
                    Version = UmbCheckoutVersion.PackageVersion,
                    AllowTelemetry = true,
                    Extensions =
                    [
                        new
                        {
                            type = "bundle",
                            alias = "UmbCheckout.Stripe.Bundle",
                            name = "UmbCheckout Stripe Bundle",
                            js = "/App_Plugins/UmbCheckout.Stripe/umbcheckout-stripe.js"
                        }
                    ]
                }
            ];
            return Task.FromResult(manifests);
        }
    }
}
