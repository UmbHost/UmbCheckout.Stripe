using UmbCheckout.Core.Interfaces;
using UmbCheckout.Shared.Models;
using UmbCheckout.Stripe.Interfaces;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Web;

namespace UmbCheckout.Stripe.Services
{
    /// <inheritdoc />
    internal sealed class UmbCheckoutSuccessPageMatcher : IUmbCheckoutSuccessPageMatcher
    {
        private const string DocumentUdiPrefix = "umb://document/";

        private readonly IConfigurationService _configurationService;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly IPublishedUrlProvider _urlProvider;

        public UmbCheckoutSuccessPageMatcher(IConfigurationService configurationService, IUmbracoContextAccessor umbracoContextAccessor, IPublishedUrlProvider urlProvider)
        {
            _configurationService = configurationService;
            _umbracoContextAccessor = umbracoContextAccessor;
            _urlProvider = urlProvider;
        }

        /// <inheritdoc />
        public async Task<bool> IsSuccessPageAsync(IPublishedContent? routedPage, Uri requestUri, CancellationToken cancellationToken = default)
        {
            var picker = await SuccessPageAsync();
            if (picker == null)
            {
                return false;
            }

            // The routed page is the success page itself, whatever its culture or domain.
            if (routedPage != null && PageKey(picker) is { } key)
            {
                return routedPage.Key == key;
            }

            return Matches(picker, requestUri);
        }

        /// <inheritdoc />
        public async Task<bool> IsSuccessPageAsync(Uri requestUri, CancellationToken cancellationToken = default)
        {
            var picker = await SuccessPageAsync();
            return picker != null && Matches(picker, requestUri);
        }

        private async Task<MultiUrlPicker?> SuccessPageAsync()
            => (await _configurationService.GetConfiguration())?.SuccessPageUrl?.FirstOrDefault();

        private static Guid? PageKey(MultiUrlPicker picker)
            => picker.Udi?.StartsWith(DocumentUdiPrefix, StringComparison.Ordinal) == true
               && Guid.TryParse(picker.Udi.Substring(DocumentUdiPrefix.Length), out var key)
                ? key
                : null;

        private bool Matches(MultiUrlPicker picker, Uri requestUri)
        {
            var request = Normalise(requestUri);
            var page = PageKey(picker) is { } key && _umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext)
                ? umbracoContext.Content?.GetById(key)
                : null;

            if (page == null)
            {
                // Not in the cache (unpublished, or a link that isn't a document): compare the stored URL's path.
                return !string.IsNullOrEmpty(picker.Url)
                       && Uri.TryCreate(requestUri, picker.Url, out var stored)
                       && string.Equals(request.Path, Normalise(stored).Path, StringComparison.Ordinal);
            }

            // Umbraco keys an invariant page's single culture as "".
            IEnumerable<string?> cultures = page.Cultures.Count > 0
                ? page.Cultures.Keys.Select(culture => string.IsNullOrEmpty(culture) ? null : culture)
                : new string?[] { null };
            foreach (var culture in cultures)
            {
                var url = _urlProvider.GetUrl(page, UrlMode.Absolute, culture, requestUri);
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
