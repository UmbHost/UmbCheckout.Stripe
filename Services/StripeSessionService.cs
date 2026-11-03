using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using UmbCheckout.Core.Interfaces;
using UmbCheckout.Shared.Models;
using UmbCheckout.Shared.Notifications.PaymentProvider;
using UmbCheckout.Stripe.Helpers;
using UmbCheckout.Stripe.Interfaces;
using UmbCheckout.Stripe.Models;
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
        private readonly IPublishedContentCache _contentCache;
        private readonly IEventAggregator _eventAggregator;
        private readonly ICoreScopeProvider _coreScopeProvider;
        private readonly IConfigurationService _configurationService;
        private readonly ILogger<StripeSessionService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStripeShippingOptionsService _stripeShippingOptionsService;
        private readonly IStripeSettingsService _stripeSettingsService;
        private readonly IUmbCheckoutStripeClientFactory _stripeClientFactory;
        private readonly LicenseService? _licenseService;

        public StripeSessionService(IPublishedContentCache contentCache, ICoreScopeProvider coreScopeProvider, IEventAggregator eventAggregator, ILogger<StripeSessionService> logger, IConfigurationService configurationService, IHttpContextAccessor httpContextAccessor, IStripeShippingOptionsService stripeShippingOptionsService, LicenseService? licenseService, IStripeSettingsService stripeSettingsService, IUmbCheckoutStripeClientFactory stripeClientFactory)
        {
            _contentCache = contentCache;
            _coreScopeProvider = coreScopeProvider;
            _eventAggregator = eventAggregator;
            _logger = logger;
            _configurationService = configurationService;
            _httpContextAccessor = httpContextAccessor;
            _stripeShippingOptionsService = stripeShippingOptionsService;
            _stripeSettingsService = stripeSettingsService;
            _stripeClientFactory = stripeClientFactory;
            _licenseService = licenseService;
        }

        /// <summary>A live checkout session is reported to the licensing service.</summary>
        private void ReportLiveMode(Session session)
        {
            if (session.Livemode)
            {
                _licenseService?.ReportLiveModeActivity();
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

                var stripeClient = _stripeClientFactory.CreateAsync().GetAwaiter().GetResult();

                var service = new SessionService(stripeClient);
                var session = service.Get(id, options?.ToSessionGetOptions());

                scope.Notifications.Publish(new OnProviderGetSessionNotification(id));

                return session;

            }
            catch (OperationCanceledException ex)
            {
                // No caller token here: a cancellation is Stripe's HTTP timeout.
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
        public Task<Session> GetSessionAsync(string id) => GetSessionAsync(id, CancellationToken.None);

        /// <inheritdoc />
        public async Task<Session> GetSessionAsync(string id, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                using var scope = _coreScopeProvider.CreateCoreScope(autoComplete: true);
                await _eventAggregator.PublishAsync(new OnProviderGetSessionStartedNotification(id), cancellationToken);

                var service = new SessionService(await _stripeClientFactory.CreateAsync(cancellationToken));
                var session = await service.GetAsync(id, cancellationToken: cancellationToken);

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
                var configuration = _configurationService.GetConfigurationAsync().GetAwaiter().GetResult();
                var stripeSettings = _stripeSettingsService.GetStripeSettingsAsync().GetAwaiter().GetResult();
                using var scope = _coreScopeProvider.CreateCoreScope(autoComplete: true);
                _eventAggregator.Publish(new OnProviderCreateSessionStartedNotification(basket));

                var stripeClient = _stripeClientFactory.CreateAsync().GetAwaiter().GetResult();

                var service = new SessionService(stripeClient);
                var session = service.Create(CreateSessionOptions(basket, configuration, stripeSettings, CancellationToken.None).GetAwaiter().GetResult());
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
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Creating a Stripe checkout session for a basket of {LineItemCount} lines in {CurrencyCode} failed", basket.LineItems.Count(), basket.CurrencyCode);
                throw;
            }
        }

        /// <inheritdoc />
        public Task<Session> CreateSessionAsync(Basket basket) => CreateSessionAsync(basket, CancellationToken.None);

        /// <inheritdoc />
        public async Task<Session> CreateSessionAsync(Basket basket, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Read before opening a scope: a read inside a scope is never cached (it may yet roll back), so checkout would
                // otherwise never fill a cold settings cache.
                var configuration = await _configurationService.GetConfigurationAsync(cancellationToken);
                var stripeSettings = await _stripeSettingsService.GetStripeSettingsAsync(cancellationToken);
                using var scope = _coreScopeProvider.CreateCoreScope(autoComplete: true);
                await _eventAggregator.PublishAsync(new OnProviderCreateSessionStartedNotification(basket), cancellationToken);

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

                var stripeClient = _stripeClientFactory.CreateAsync().GetAwaiter().GetResult();

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
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Expiring Stripe checkout session {SessionId} failed", id);
                throw;
            }
        }

        /// <inheritdoc />
        public Task ClearSessionAsync(string id) => ClearSessionAsync(id, CancellationToken.None);

        /// <inheritdoc />
        public async Task ClearSessionAsync(string id, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                using var scope = _coreScopeProvider.CreateCoreScope(autoComplete: true);
                await _eventAggregator.PublishAsync(new OnProviderClearSessionStartedNotification(id), cancellationToken);

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

        /// <summary>Which basket a session pays for: its culture and its trimmed, upper-case currency (the basket's own, else the session's).</summary>
        internal static Dictionary<string, string> BasketMetadata(Basket basket, string currency) => new()
        {
            [Consts.SessionMetadata.BasketCulture] = basket.SiteRootCulture ?? string.Empty,
            [Consts.SessionMetadata.BasketCurrency] = (string.IsNullOrWhiteSpace(basket.CurrencyCode) ? currency : basket.CurrencyCode).Trim().ToUpperInvariant()
        };

        /// <summary>
        /// Converts the Basket into the Stripe SessionCreateOptions
        /// </summary>
        /// <param name="basket">The Basket to the converted</param>
        /// <param name="configuration">The configuration, read before any scope was opened</param>
        /// <param name="stripeSettings">The Stripe settings, read before any scope was opened</param>
        /// <param name="cancellationToken">Stops the shipping options lookup</param>
        /// <returns>The Stripe SessionCreateOptions</returns>
        private async Task<SessionCreateOptions> CreateSessionOptions(Basket basket, UmbCheckoutConfiguration? configuration, UmbCheckoutStripeSettings? stripeSettings, CancellationToken cancellationToken)
        {
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
                            var successPage = _contentCache.GetById(guid);
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
                            var cancelPage = _contentCache.GetById(guid);
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
                var currency = StripeLineItemPrice.BasketCurrency(basket.LineItems);
                foreach (var lineItem in basket.LineItems)
                {
                    var product = _contentCache.GetById(lineItem.Key);
                    if (product != null)
                    {
                        var stripeLineItem = new SessionLineItemOptions
                        {
                            PriceData = StripeLineItemPrice.PriceData(lineItem, currency, product.Name),
                            Quantity = lineItem.Quantity
                        };

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

                        // Read through the property itself: the friendly HasValue/Value extensions need Umbraco's static service provider.
                        var metaDataProperty = product.GetProperty(Shared.Consts.PropertyAlias.MetaDataAlias);
                        var productMetaData = metaDataProperty?.HasValue() == true ? metaDataProperty.GetValue() as Dictionary<string, string> : null;
                        stripeLineItem.PriceData.ProductData.Metadata = CheckoutMetadata.ForLine(lineItem.MetaData, productMetaData, product.Key);

                        await _eventAggregator.PublishAsync(new OnProviderSessionOptionsLineItemAdding(stripeLineItem), cancellationToken);

                        stripeLineItems.Add(stripeLineItem);

                        await _eventAggregator.PublishAsync(new OnProviderSessionOptionsLineItemAdded(stripeLineItem), cancellationToken);
                    }
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
                    // Which basket this session pays for: the success handler runs before the page's culture is known.
                    Metadata = BasketMetadata(basket, currency)
                };

                // Published now, not with a scope (which queues them until it ends, after Stripe has the options): handlers change the session.
                await _eventAggregator.PublishAsync(new OnProviderSessionOptionsStarted(options), cancellationToken);

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
                    // Stripe rejects an empty shipping_options list (sent as an empty string): only set it when a rate applies.
                    var shippingOptions = await _stripeShippingOptionsService.GetShippingOptions(basket, cancellationToken);
                    if (shippingOptions.Count > 0)
                    {
                        options.ShippingOptions = shippingOptions;
                    }

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

                await _eventAggregator.PublishAsync(new OnProviderSessionOptionsCreated(options), cancellationToken);

                return options;
            }
        }
    }
}
