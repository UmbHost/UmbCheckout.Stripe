using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using UmbCheckout.Core.Interfaces;
using UmbCheckout.Shared.Extensions;
using UmbCheckout.Shared.Models;
using UmbCheckout.Shared.Notifications.PaymentProvider;
using UmbCheckout.Stripe.Interfaces;
using UmbCheckout.Stripe.Models;
using UmbCheckout.Stripe.Helpers;
using UmbCheckout.Stripe.Money;
using UmbCheckout.Stripe.Notifications;
using UmbHost.Licensing.Models;
using UmbHost.Licensing.Services;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Extensions;

namespace UmbCheckout.Stripe.Services
{
    /// <summary>
    /// A service which handles all things around the Stripe Session
    /// </summary>
    internal class StripeSessionService : IStripeSessionService
    {
        private readonly IPublishedSnapshotAccessor _snapshotAccessor;
        private readonly IEventAggregator _eventAggregator;
        private readonly ICoreScopeProvider _coreScopeProvider;
        private readonly IConfigurationService _configurationService;
        private readonly ILogger<StripeSessionService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStripeShippingOptionsService _stripeShippingOptionsService;
        private readonly StripeSettings _stripeSettings;
        private readonly IStripeSettingsService _stripeSettingsService;
        private readonly LicenseService _licenseService;
        private readonly IUmbCheckoutStripeClientFactory _stripeClientFactory;

        public StripeSessionService(IPublishedSnapshotAccessor snapshotAccessor, ICoreScopeProvider coreScopeProvider, IEventAggregator eventAggregator, ILogger<StripeSessionService> logger, IConfigurationService configurationService, IHttpContextAccessor httpContextAccessor, IStripeShippingOptionsService stripeShippingOptionsService, IOptionsMonitor<StripeSettings> stripeSettings, LicenseService licenseService, IStripeSettingsService stripeSettingsService,
            IUmbCheckoutStripeClientFactory stripeClientFactory)
        {
            _stripeClientFactory = stripeClientFactory;
            _snapshotAccessor = snapshotAccessor;
            _coreScopeProvider = coreScopeProvider;
            _eventAggregator = eventAggregator;
            _logger = logger;
            _configurationService = configurationService;
            _httpContextAccessor = httpContextAccessor;
            _stripeShippingOptionsService = stripeShippingOptionsService;
            _stripeSettingsService = stripeSettingsService;
            _stripeSettings = stripeSettings.CurrentValue;
            _licenseService = licenseService;
        }

        /// <summary>
        /// The Checkout line items for a basket: one currency for the session, each unit amount in that currency's minor unit
        /// (rounded once by <see cref="StripeCurrency.Round"/>). Lines whose product is no longer published are left out.
        /// </summary>
        /// <exception cref="InvalidOperationException">The basket is empty or mixes currencies.</exception>
        /// <exception cref="ArgumentException">The basket's currency is empty (a basket saved before the upgrade) or not one Stripe supports.</exception>
        internal static List<(Shared.Models.LineItem LineItem, IPublishedContent Product, SessionLineItemOptions Options)> BuildLineItems(IEnumerable<Shared.Models.LineItem> lineItems, Func<Guid, IPublishedContent?> getProduct)
        {
            var lines = lineItems.ToList();
            var currency = StripeLineItemPrice.BasketCurrency(lines);
            var built = new List<(Shared.Models.LineItem LineItem, IPublishedContent Product, SessionLineItemOptions Options)>();
            foreach (var lineItem in lines)
            {
                var product = getProduct(lineItem.Key);
                if (product == null)
                {
                    continue;
                }

                built.Add((lineItem, product, new SessionLineItemOptions
                {
                    PriceData = StripeLineItemPrice.PriceData(lineItem, currency, product.Name ?? string.Empty),
                    Quantity = lineItem.Quantity
                }));
            }

            return built;
        }

        /// <summary>
        /// Which basket a checkout session pays for (its site root culture and currency), stored on the session so the success page
        /// can clear that basket: it runs while routing, before the page's culture is known.
        /// </summary>
        internal static Dictionary<string, string> BasketMetadata(Basket basket) => new()
        {
            [Consts.SessionMetadata.BasketCulture] = basket.SiteRootCulture ?? string.Empty,
            [Consts.SessionMetadata.BasketCurrency] = basket.CurrencyCode?.Trim().ToUpperInvariant() ?? basket.LineItems.SingleCurrency() ?? string.Empty
        };

        /// <summary>The shipping options to send, or null when none applies: Stripe rejects an empty shipping_options list.</summary>
        internal static List<SessionShippingOptionOptions>? ShippingOptionsToSend(List<SessionShippingOptionOptions>? shippingOptions)
            => shippingOptions is { Count: > 0 } ? shippingOptions : null;

        /// <summary>A live checkout session means real payments: development mode (licensed features without a licence) ends.</summary>
        private void ReportLiveMode(Session? session)
        {
            if (session != null && (session.Livemode || session.Id?.StartsWith("cs_live_", StringComparison.Ordinal) == true))
            {
                _licenseService.ReportLiveModeActivity();
            }
        }

        /// <inheritdoc />
        [Obsolete("Blocks a thread on a Stripe call; use the async method with a CancellationToken. Scheduled for removal in 17.1.")]
        public Session GetSession(string id, UmbCheckoutSessionGetOptions? options = null)
        {
            try
            {
                using var scope = _coreScopeProvider.CreateCoreScope(autoComplete: true);
                _eventAggregator.Publish(new OnProviderGetSessionStartedNotification(id));

                var apiKey = string.Empty;
                var stripeSettings = _stripeSettingsService.GetStripeSettings().Result;
                if (stripeSettings != null)
                {
                    apiKey = stripeSettings.UseLiveApiDetails ? _stripeSettings.Live.ApiKey : _stripeSettings.Test.ApiKey;
                }

                var stripeClient = new StripeClient(apiKey);

                var service = new SessionService(stripeClient);
                var session = service.Get(id, options?.ToSessionGetOptions());
                ReportLiveMode(session);

                scope.Notifications.Publish(new OnProviderGetSessionNotification(id));

                return session;

            }
            catch (OperationCanceledException ex)
            {
                // No caller token here: a cancellation is Stripe's HTTP timeout.
                _logger.LogWarning(ex, "Getting Stripe checkout session {SessionId} timed out", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Getting Stripe checkout session {SessionId} failed", id);
                throw;
            }
        }

        /// <inheritdoc />
        [Obsolete("Use GetSessionAsync(string, CancellationToken) (the overload with a CancellationToken). Scheduled for removal in 17.1.")]
        public Task<Session> GetSessionAsync(string id) => GetSessionAsync(id, CancellationToken.None);

        /// <inheritdoc />
        public async Task<Session> GetSessionAsync(string id, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                using var scope = _coreScopeProvider.CreateCoreScope(autoComplete: true);
                await _eventAggregator.PublishAsync(new OnProviderGetSessionStartedNotification(id), cancellationToken);

                // The client for the saved mode (live keys only when live mode is saved), as every other Stripe call.
                var stripeClient = await _stripeClientFactory.CreateAsync(cancellationToken);

                var service = new SessionService(stripeClient);
                var session = await service.GetAsync(id, cancellationToken: cancellationToken);
                ReportLiveMode(session);

                scope.Notifications.Publish(new OnProviderGetSessionNotification(id));

                return session;
            }
            catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested)
            {
                // Not the caller's cancellation (that stays silent): Stripe's HTTP timeout.
                _logger.LogWarning(ex, "Getting Stripe checkout session {SessionId} timed out", id);
                throw;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Getting Stripe checkout session {SessionId} failed", id);
                throw;
            }
        }

        /// <inheritdoc />
        [Obsolete("Blocks a thread on a Stripe call; use the async method with a CancellationToken. Scheduled for removal in 17.1.")]
        public Session CreateSession(Basket basket)
        {
            try
            {
                using var scope = _coreScopeProvider.CreateCoreScope(autoComplete: true);
                _eventAggregator.Publish(new OnProviderCreateSessionStartedNotification(basket));

                var apiKey = string.Empty;
                var stripeSettings = _stripeSettingsService.GetStripeSettings().Result;
                if (stripeSettings != null)
                {
                    apiKey = stripeSettings.UseLiveApiDetails ? _stripeSettings.Live.ApiKey : _stripeSettings.Test.ApiKey;
                }

                var stripeClient = new StripeClient(apiKey);

                var service = new SessionService(stripeClient);
                var session = service.Create(CreateSessionOptions(basket, CancellationToken.None).Result);
                ReportLiveMode(session);

                scope.Notifications.Publish(new OnProviderSessionCreatedNotification(basket));

                return session;
            }
            catch (OperationCanceledException ex)
            {
                // No caller token here: a cancellation is Stripe's HTTP timeout.
                _logger.LogWarning(ex, "Creating a Stripe checkout session for a basket of {LineItemCount} lines in {CurrencyCode} timed out", basket.LineItems.Count(), basket.CurrencyCode);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Creating a Stripe checkout session for a basket of {LineItemCount} lines in {CurrencyCode} failed", basket.LineItems.Count(), basket.CurrencyCode);
                throw;
            }
        }

        /// <inheritdoc />
        [Obsolete("Use CreateSessionAsync(Basket, CancellationToken) (the overload with a CancellationToken). Scheduled for removal in 17.1.")]
        public Task<Session> CreateSessionAsync(Basket basket) => CreateSessionAsync(basket, CancellationToken.None);

        /// <inheritdoc />
        public async Task<Session> CreateSessionAsync(Basket basket, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Read before opening a scope: a read inside a scope is never cached (it may yet roll back), so checkout would
                // otherwise never fill a cold settings cache.
                var configuration = await _configurationService.GetConfiguration();
                var stripeSettings = await _stripeSettingsService.GetStripeSettings();
                cancellationToken.ThrowIfCancellationRequested();

                using var scope = _coreScopeProvider.CreateCoreScope(autoComplete: true);
                await _eventAggregator.PublishAsync(new OnProviderCreateSessionStartedNotification(basket), cancellationToken);

                // The client for the saved mode (live keys only when live mode is saved), as every other Stripe call.
                var stripeClient = await _stripeClientFactory.CreateAsync(cancellationToken);

                var service = new SessionService(stripeClient);
                var session = await service.CreateAsync(await CreateSessionOptions(basket, configuration, stripeSettings, cancellationToken), cancellationToken: cancellationToken);
                ReportLiveMode(session);

                scope.Notifications.Publish(new OnProviderSessionCreatedNotification(basket));

                return session;
            }
            catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested)
            {
                // Not the caller's cancellation (that stays silent): Stripe's HTTP timeout.
                _logger.LogWarning(ex, "Creating a Stripe checkout session for a basket of {LineItemCount} lines in {CurrencyCode} timed out", basket.LineItems.Count(), basket.CurrencyCode);
                throw;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Creating a Stripe checkout session for a basket of {LineItemCount} lines in {CurrencyCode} failed", basket.LineItems.Count(), basket.CurrencyCode);
                throw;
            }
        }

        /// <inheritdoc />
        [Obsolete("Blocks a thread on a Stripe call; use the async method with a CancellationToken. Scheduled for removal in 17.1.")]
        public void ClearSession(string id)
        {
            try
            {
                using var scope = _coreScopeProvider.CreateCoreScope(autoComplete: true);
                _eventAggregator.Publish(new OnProviderClearSessionStartedNotification(id));

                var apiKey = string.Empty;
                var stripeSettings = _stripeSettingsService.GetStripeSettings().Result;
                if (stripeSettings != null)
                {
                    apiKey = stripeSettings.UseLiveApiDetails ? _stripeSettings.Live.ApiKey : _stripeSettings.Test.ApiKey;
                }

                var stripeClient = new StripeClient(apiKey);

                var service = new SessionService(stripeClient);
                service.Expire(id);

                scope.Notifications.Publish(new OnProviderSessionClearedNotification(id));
            }
            catch (OperationCanceledException ex)
            {
                // No caller token here: a cancellation is Stripe's HTTP timeout.
                _logger.LogWarning(ex, "Expiring Stripe checkout session {SessionId} timed out", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Expiring Stripe checkout session {SessionId} failed", id);
                throw;
            }
        }

        /// <inheritdoc />
        [Obsolete("Use ClearSessionAsync(string, CancellationToken) (the overload with a CancellationToken). Scheduled for removal in 17.1.")]
        public Task ClearSessionAsync(string id) => ClearSessionAsync(id, CancellationToken.None);

        /// <inheritdoc />
        public async Task ClearSessionAsync(string id, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                using var scope = _coreScopeProvider.CreateCoreScope(autoComplete: true);
                await _eventAggregator.PublishAsync(new OnProviderClearSessionStartedNotification(id), cancellationToken);

                // The site's own client (this used Stripe.net's global client, which UmbCheckout never configures).
                var service = new SessionService(await _stripeClientFactory.CreateAsync(cancellationToken));
                await service.ExpireAsync(id, cancellationToken: cancellationToken);

                scope.Notifications.Publish(new OnProviderSessionClearedNotification(id));
            }
            catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested)
            {
                // Not the caller's cancellation (that stays silent): Stripe's HTTP timeout.
                _logger.LogWarning(ex, "Expiring Stripe checkout session {SessionId} timed out", id);
                throw;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Expiring Stripe checkout session {SessionId} failed", id);
                throw;
            }
        }

        /// <summary>
        /// Converts the Basket into the Stripe SessionCreateOptions
        /// </summary>
        /// <param name="basket">The Basket to the converted</param>
        /// <returns>The Stripe SessionCreateOptions</returns>
        private async Task<SessionCreateOptions?> CreateSessionOptions(Basket basket, CancellationToken cancellationToken)
            => await CreateSessionOptions(basket, await _configurationService.GetConfiguration(), await _stripeSettingsService.GetStripeSettings(), cancellationToken);

        /// <summary>
        /// Converts the Basket into the Stripe SessionCreateOptions, with the configuration and Stripe settings already read
        /// </summary>
        private async Task<SessionCreateOptions?> CreateSessionOptions(Basket basket, Shared.Models.UmbCheckoutConfiguration? configuration, UmbCheckoutStripeSettings? stripeSettings, CancellationToken cancellationToken)
        {
            var hasPublishedSnapshot = _snapshotAccessor.TryGetPublishedSnapshot(out var publishedSnapshot);
            if (hasPublishedSnapshot)
            {
                Uri? successUri = null;
                Uri? cancelUri = null;
                if (configuration != null)
                {
                    var pickedSuccessPage = configuration.SuccessPageUrl.FirstOrDefault();
                    if (pickedSuccessPage != null && pickedSuccessPage.Udi.StartsWith("umb://document/"))
                    {
                        var guidString = pickedSuccessPage.Udi.Replace("umb://document/", "");
                        if (Guid.TryParse(guidString, out var guid))
                        {
                            var successPage = publishedSnapshot!.Content!.GetById(guid);
                            if (successPage != null)
                            {
                                var url = successPage.Url(mode: UrlMode.Absolute);
                                var isValidSuccessUrl = Uri.TryCreate(url, UriKind.Absolute, out successUri)
                                                        && (successUri.Scheme == Uri.UriSchemeHttp || successUri.Scheme == Uri.UriSchemeHttps);
                                if (!isValidSuccessUrl)
                                {
                                    var request = _httpContextAccessor.HttpContext?.Request;
                                    if (request != null)
                                    {
                                        successUri =
                                            new Uri($"{request.Scheme}://{request.Host.Value}/{pickedSuccessPage.Url.Trim('/')}");
                                    }
                                }
                                else
                                {
                                    successUri = new Uri(url);
                                }
                            }
                        }
                    }

                    var pickedCancelPage = configuration.CancelPageUrl.FirstOrDefault();
                    if (pickedCancelPage != null && pickedCancelPage.Udi.StartsWith("umb://document/"))
                    {
                        var guidString = pickedCancelPage.Udi.Replace("umb://document/", "");
                        if (Guid.TryParse(guidString, out var guid))
                        {
                            var cancelPage = publishedSnapshot!.Content!.GetById(guid);
                            if (cancelPage != null)
                            {
                                var url = cancelPage.Url(mode: UrlMode.Absolute);
                                var isValidCancelUrl = Uri.TryCreate(url, UriKind.Absolute, out cancelUri)
                                                        && (cancelUri.Scheme == Uri.UriSchemeHttp || cancelUri.Scheme == Uri.UriSchemeHttps);
                                if (!isValidCancelUrl)
                                {
                                    var request = _httpContextAccessor.HttpContext?.Request;
                                    if (request != null)
                                    {
                                        cancelUri =
                                            new Uri($"{request.Scheme}://{request.Host.Value}/{pickedCancelPage.Url.Trim('/')}");
                                    }
                                }
                                else
                                {
                                    cancelUri = new Uri(url);
                                }
                            }
                        }
                    }
                }

                var stripeLineItems = new List<SessionLineItemOptions>();
                foreach (var (lineItem, product, stripeLineItem) in BuildLineItems(basket.LineItems, key => publishedSnapshot?.Content?.GetById(key)))
                {
                    if (!string.IsNullOrEmpty(lineItem.Description))
                    {
                        stripeLineItem.PriceData.ProductData.Description = lineItem.Description;
                    }
                    else if (product.HasValue(Shared.Consts.PropertyAlias.DescriptionAlias))
                    {
                        stripeLineItem.PriceData.ProductData.Description = product.Value<string>(Shared.Consts.PropertyAlias.DescriptionAlias);
                    }
                    else if (product.HasValue(Shared.Consts.PropertyAlias.FallbackDescriptionAlias))
                    {
                        stripeLineItem.PriceData.ProductData.Description = product.Value<string>(Shared.Consts.PropertyAlias.FallbackDescriptionAlias);
                    }

                    // The line's values, then the product's for keys the line doesn't set, then the node key: duplicates never throw (as v17).
                    var productMetaData = product.HasValue(Shared.Consts.PropertyAlias.MetaDataAlias)
                        ? product.Value<Dictionary<string, string>>(Shared.Consts.PropertyAlias.MetaDataAlias)
                        : null;
                    stripeLineItem.PriceData.ProductData.Metadata = CheckoutMetadata.ForLine(lineItem.MetaData, productMetaData, product.Key);

                    await _eventAggregator.PublishAsync(new OnProviderSessionOptionsLineItemAdding(stripeLineItem));

                    stripeLineItems.Add(stripeLineItem);

                    await _eventAggregator.PublishAsync(new OnProviderSessionOptionsLineItemAdded(stripeLineItem));
                }

                var options = new SessionCreateOptions
                {
                    LineItems = stripeLineItems,
                    PhoneNumberCollection = new SessionPhoneNumberCollectionOptions
                    {
                        Enabled = true,
                    },
                    Mode = Shared.Consts.SessionMode,
                    SuccessUrl = successUri != null ? string.Concat(successUri.ToString(), "?session_id={CHECKOUT_SESSION_ID}&success=true") : string.Empty,
                    CancelUrl = cancelUri != null ? string.Concat(cancelUri.ToString(), "?session_id={CHECKOUT_SESSION_ID}&success=false") : string.Empty,
                    Metadata = BasketMetadata(basket)
                };

                // Published now, not with a scope (which queues them until it ends, after Stripe has the options): handlers change the session.
                await _eventAggregator.PublishAsync(new OnProviderSessionOptionsStarted(options));

                if (!string.IsNullOrEmpty(basket.CustomerReferenceId))
                {
                    options.ClientReferenceId = basket.CustomerReferenceId;
                }

                if (basket.Customer != null)
                {
                    if (!string.IsNullOrEmpty(basket.Customer.EmailAddress))
                    {
                        options.CustomerEmail = basket.Customer.EmailAddress;
                    }
                }

                if (stripeSettings != null)
                {
                    options.PhoneNumberCollection = new SessionPhoneNumberCollectionOptions
                    {
                        Enabled = stripeSettings.CollectPhoneNumber
                    };
                }

                if (configuration is { EnableShipping: true })
                {
                    // Only the rates that charge in the basket's currency; none is sent when none applies (Stripe rejects an empty list).
                    options.ShippingOptions = ShippingOptionsToSend(await _stripeShippingOptionsService.GetShippingOptions(basket, cancellationToken));

                    options.BillingAddressCollection = "required";

                    if (stripeSettings != null)
                    {
                        if (!string.IsNullOrEmpty(stripeSettings.ShippingAllowedCountries))
                        {
                            var allowedCountries = stripeSettings.ShippingAllowedCountries.Split(",");

                            if (allowedCountries.Any())
                            {
                                options.ShippingAddressCollection = new SessionShippingAddressCollectionOptions
                                {
                                    AllowedCountries = allowedCountries.ToList()
                                };
                            }
                        }
                    }
                }

                if (stripeSettings is { CollectPromotionalEmailsConsent: true })
                {
                    options.ConsentCollection = new SessionConsentCollectionOptions
                    {
                        Promotions = "auto",
                    };

                    if (stripeSettings is { EnableAbandonedCartRecovery: true })
                    {
                        var afterExpirationOptions = new SessionAfterExpirationOptions
                        {
                            Recovery = new SessionAfterExpirationRecoveryOptions
                            {
                                Enabled = stripeSettings.EnableAbandonedCartRecovery
                            }
                        };

                        if (stripeSettings is { AllowPromotionalCodesOnRecoveredCarts: true })
                        {
                            afterExpirationOptions.Recovery.AllowPromotionCodes =
                                stripeSettings.AllowPromotionalCodesOnRecoveredCarts;
                        }

                        options.AfterExpiration = afterExpirationOptions;
                    }
                }

                if (stripeSettings is { AllowPromotionalCodes: true })
                {
                    options.AllowPromotionCodes = true;
                }

                await _eventAggregator.PublishAsync(new OnProviderSessionOptionsCreated(options));

                return options;
            }

            return null;
        }
    }
}
