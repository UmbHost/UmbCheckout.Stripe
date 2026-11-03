using UmbCheckout.Core.Interfaces;
using UmbCheckout.Stripe.Interfaces;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Routing;

namespace UmbCheckout.Stripe.Services
{
    /// <inheritdoc />
    internal sealed class UmbCheckoutSuccessPageMatcher(IConfigurationService configurationService, IPublishedContentCache contentCache, IPublishedUrlProvider urlProvider)
        : IUmbCheckoutSuccessPageMatcher
    {
        private const string DocumentUdiPrefix = "umb://document/";

        public async Task<bool> IsSuccessPageAsync(Uri requestUri, CancellationToken cancellationToken = default)
        {
            var picker = (await configurationService.GetConfigurationAsync(cancellationToken))?.SuccessPageUrl.FirstOrDefault();
            if (picker == null)
            {
                return false;
            }

            var request = Normalise(requestUri);
            var page = picker.Udi?.StartsWith(DocumentUdiPrefix, StringComparison.Ordinal) == true
                       && Guid.TryParse(picker.Udi[DocumentUdiPrefix.Length..], out var key)
                ? contentCache.GetById(key)
                : null;

            if (page == null)
            {
                // Not in the cache (unpublished, or a value saved by v13): compare the stored URL's path.
                return !string.IsNullOrEmpty(picker.Url)
                       && string.Equals(request.Path, TrimSlash(new Uri(requestUri, picker.Url).AbsolutePath).ToLowerInvariant(), StringComparison.Ordinal);
            }

            // Umbraco keys an invariant page's single culture as "".
            IEnumerable<string?> cultures = page.Cultures.Count > 0
                ? page.Cultures.Keys.Select(culture => string.IsNullOrEmpty(culture) ? null : culture)
                : [null];
            foreach (var culture in cultures)
            {
                var url = urlProvider.GetUrl(page, UrlMode.Absolute, culture, requestUri);
                if (Uri.TryCreate(url, UriKind.Absolute, out var pageUri) && Normalise(pageUri) == request)
                {
                    return true;
                }
            }

            return false;
        }

        private static (string Host, string Path) Normalise(Uri uri) => (uri.Host.ToLowerInvariant(), TrimSlash(uri.AbsolutePath).ToLowerInvariant());

        private static string TrimSlash(string path) => path.Length > 1 ? path.TrimEnd('/') : path;
    }
}
