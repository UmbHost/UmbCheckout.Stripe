using UmbCheckout.Shared;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Manifest;
using Microsoft.Extensions.DependencyInjection;
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
}