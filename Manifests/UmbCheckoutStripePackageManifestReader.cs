using UmbCheckout.Shared;
using Umbraco.Cms.Core.Manifest;
using Umbraco.Cms.Infrastructure.Manifest;

namespace UmbCheckout.Stripe.Manifests
{
    /// <summary>
    /// Registers the UmbCheckout Stripe package manifest, which loads the Stripe backoffice extensions bundle
    /// </summary>
    internal class UmbCheckoutStripePackageManifestReader : IPackageManifestReader
    {
        public Task<IEnumerable<PackageManifest>> ReadPackageManifestsAsync()
        {
            IEnumerable<PackageManifest> manifests =
            [
                new()
                {
                    Id = $"{Shared.Consts.PackageName}.{Consts.AppSettingsSectionName}",
                    Name = $"{Shared.Consts.PackageName}.{Consts.AppSettingsSectionName}",
                    Version = UmbCheckoutVersion.Version.ToString(3),
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
