using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using UmbCheckout.Core.Interfaces;
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
        public StripeSessionService(IPublishedSnapshotAccessor snapshotAccessor, ICoreScopeProvider coreScopeProvider, IEventAggregator eventAggregator, ILogger<StripeSessionService> logger, IConfigurationService configurationService, IHttpContextAccessor httpContextAccessor, IStripeShippingOptionsService stripeShippingOptionsService, IOptionsMonitor<StripeSettings> stripeSettings, LicenseService licenseService, IStripeSettingsService stripeSettingsService)
        {
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

        /// <summary>A live checkout session means real payments: development mode (licensed features without a licence) ends.</summary>
        private void ReportLiveMode(Session? session)
        {
            if (session != null && (session.Livemode || session.Id?.StartsWith("cs_live_", StringComparison.Ordinal) == true))
            {
                _licenseService.ReportLiveModeActivity();
            }
        }

        /// <inheritdoc />
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
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<Session> GetSessionAsync(string id)
        {
            try
            {
                using var scope = _coreScopeProvider.CreateCoreScope(autoComplete: true);
                await _eventAggregator.PublishAsync(new OnProviderGetSessionStartedNotification(id));

                var apiKey = string.Empty;
                var stripeSettings = _stripeSettingsService.GetStripeSettings().Result;
                if (stripeSettings != null)
                {
                    apiKey = stripeSettings.UseLiveApiDetails ? _stripeSettings.Live.ApiKey : _stripeSettings.Test.ApiKey;
                }

                var stripeClient = new StripeClient(apiKey);

                var service = new SessionService(stripeClient);
                var session = await service.GetAsync(id);
                ReportLiveMode(session);

                scope.Notifications.Publish(new OnProviderGetSessionNotification(id));

                return session;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <inheritdoc />
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
                var session = service.Create(CreateSessionOptions(basket).Result);
                ReportLiveMode(session);

                scope.Notifications.Publish(new OnProviderSessionCreatedNotification(basket));

                return session;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<Session> CreateSessionAsync(Basket basket)
        {
            try
            {
                using var scope = _coreScopeProvider.CreateCoreScope(autoComplete: true);
                await _eventAggregator.PublishAsync(new OnProviderCreateSessionStartedNotification(basket));

                var apiKey = string.Empty;
                var stripeSettings = await _stripeSettingsService.GetStripeSettings();
                if (stripeSettings != null)
                {
                    apiKey = stripeSettings.UseLiveApiDetails ? _stripeSettings.Live.ApiKey : _stripeSettings.Test.ApiKey;
                }

                var stripeClient = new StripeClient(apiKey);

                var service = new SessionService(stripeClient);
                var session = await service.CreateAsync(await CreateSessionOptions(basket));
                ReportLiveMode(session);

                scope.Notifications.Publish(new OnProviderSessionCreatedNotification(basket));

                return session;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <inheritdoc />
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
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task ClearSessionAsync(string id)
        {
            try
            {
                using var scope = _coreScopeProvider.CreateCoreScope(autoComplete: true);
                await _eventAggregator.PublishAsync(new OnProviderClearSessionStartedNotification(id));

                var service = new SessionService();
                await service.ExpireAsync(id);

                scope.Notifications.Publish(new OnProviderSessionClearedNotification(id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Converts the Basket into the Stripe SessionCreateOptions
        /// </summary>
        /// <param name="basket">The Basket to the converted</param>
        /// <returns>The Stripe SessionCreateOptions</returns>
        private async Task<SessionCreateOptions?> CreateSessionOptions(Basket basket)
        {
            try
            {
                using var scope = _coreScopeProvider.CreateCoreScope(autoComplete: true);
                var hasPublishedSnapshot = _snapshotAccessor.TryGetPublishedSnapshot(out var publishedSnapshot);
                if (hasPublishedSnapshot)
                {
                    var configuration = await _configurationService.GetConfiguration();
                    var stripeSettings = await _stripeSettingsService.GetStripeSettings();
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

                        if (UmbCheckoutSettings.IsLicensed)
                        {
                            if (product.HasValue(Shared.Consts.PropertyAlias.TaxRatesAlias))
                            {
                                var taxRates = product.Value<IEnumerable<string>>(Shared.Consts.PropertyAlias.TaxRatesAlias)?.ToList();
                                stripeLineItem.DynamicTaxRates = taxRates;
                            }
                        }

                        scope.Notifications.Publish(new OnProviderSessionOptionsLineItemAdding(stripeLineItem));

                        stripeLineItems.Add(stripeLineItem);

                        scope.Notifications.Publish(new OnProviderSessionOptionsLineItemAdded(stripeLineItem));
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
                        CancelUrl = cancelUri != null ? string.Concat(cancelUri.ToString(), "?session_id={CHECKOUT_SESSION_ID}&success=false") : string.Empty
                    };

                    scope.Notifications.Publish(new OnProviderSessionOptionsStarted(options));

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
                        options.ShippingOptions = await _stripeShippingOptionsService.GetShippingOptions(basket);

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

                    scope.Notifications.Publish(new OnProviderSessionOptionsCreated(options));

                    return options;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
