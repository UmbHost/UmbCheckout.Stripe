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
using UmbHost.Licensing.Models;
using UmbHost.Licensing.Services;
using Umbraco.Cms.Core.Events;
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
        private readonly IStripeShippingRateDatabaseService _stripeDatabaseService;
        private readonly StripeSettings _stripeSettings;
        private readonly IStripeSettingsService _stripeSettingsService;

        public StripeSessionService(IPublishedSnapshotAccessor snapshotAccessor, ICoreScopeProvider coreScopeProvider, IEventAggregator eventAggregator, ILogger<StripeSessionService> logger, IConfigurationService configurationService, IHttpContextAccessor httpContextAccessor, IStripeShippingRateDatabaseService stripeDatabaseService, IOptionsMonitor<StripeSettings> stripeSettings, LicenseService licenseService, IStripeSettingsService stripeSettingsService)
        {
            _snapshotAccessor = snapshotAccessor;
            _coreScopeProvider = coreScopeProvider;
            _eventAggregator = eventAggregator;
            _logger = logger;
            _configurationService = configurationService;
            _httpContextAccessor = httpContextAccessor;
            _stripeDatabaseService = stripeDatabaseService;
            _stripeSettingsService = stripeSettingsService;
            _stripeSettings = stripeSettings.CurrentValue;
            licenseService.RunLicenseCheck();
        }

        /// <inheritdoc />
        public Session GetSession(string id)
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
                var session = service.Get(id);

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
                var stripeSettings = _stripeSettingsService.GetStripeSettings().Result;
                if (stripeSettings != null)
                {
                    apiKey = stripeSettings.UseLiveApiDetails ? _stripeSettings.Live.ApiKey : _stripeSettings.Test.ApiKey;
                }

                var stripeClient = new StripeClient(apiKey);

                var service = new SessionService(stripeClient);
                var session = await service.CreateAsync(await CreateSessionOptions(basket));

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
                var hasPublishedSnapshot = _snapshotAccessor.TryGetPublishedSnapshot(out var publishedSnapshot);
                if (hasPublishedSnapshot)
                {
                    var configuration = await _configurationService.GetConfiguration();
                    Uri? successUri = null;
                    Uri? cancelUri = null;
                    if (configuration != null)
                    {
                        var isValidSuccessUrl = Uri.TryCreate(configuration.SuccessPageUrl.First().Url, UriKind.Absolute, out successUri)
                                                && (successUri.Scheme == Uri.UriSchemeHttp || successUri.Scheme == Uri.UriSchemeHttps);
                        if (!isValidSuccessUrl)
                        {
                            var request = _httpContextAccessor.HttpContext?.Request;
                            if (request != null)
                            {
                                successUri =
                                    new Uri($"{request.Scheme}://{request.Host.Value}/{configuration.SuccessPageUrl.First().Url.Trim('/')}");
                            }
                        }
                        else
                        {
                            successUri = new Uri(configuration.SuccessPageUrl.First().Url);
                        }

                        var isValidCancelUrl = Uri.TryCreate(configuration.CancelPageUrl.First().Url, UriKind.Absolute, out cancelUri)
                                               && (cancelUri.Scheme == Uri.UriSchemeHttp || cancelUri.Scheme == Uri.UriSchemeHttps);
                        if (!isValidCancelUrl)
                        {
                            var request = _httpContextAccessor.HttpContext?.Request;

                            if (request != null)
                            {
                                cancelUri =
                                    new Uri($"{request.Scheme}://{request.Host.Value}/{configuration.CancelPageUrl.First().Url.Trim('/')}");
                            }
                        }
                        else
                        {
                            cancelUri = new Uri(configuration.CancelPageUrl.First().Url);
                        }
                    }

                    var stripeLineItems = new List<SessionLineItemOptions>();
                    foreach (var lineItem in basket.LineItems)
                    {
                        if (publishedSnapshot == null && publishedSnapshot?.Content == null)
                        {
                            continue;
                        }

                        var product = publishedSnapshot.Content?.GetById(lineItem.Key);
                        if (product != null)
                        {
                            var stripeLineItem = new SessionLineItemOptions
                            {
                                PriceData = new SessionLineItemPriceDataOptions
                                {
                                    Currency = lineItem.CurrencyCode,
                                    ProductData = new SessionLineItemPriceDataProductDataOptions
                                    {
                                        Name = !string.IsNullOrEmpty(lineItem.Name) ? lineItem.Name : product.Name
                                    },
                                    UnitAmountDecimal = lineItem.Price * 100
                                },
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

                            var metaData = lineItem.MetaData;
                            if (metaData.ContainsKey("nodeKey"))
                            {
                                metaData.Add("nodeKey", product.Key.ToString());
                            }

                            if (metaData.Any())
                            {
                                if (product.HasValue(Shared.Consts.PropertyAlias.MetaDataAlias))
                                {
                                    var productMetaData = product.Value<Dictionary<string, string>>(Shared.Consts.PropertyAlias.MetaDataAlias);

                                    if (productMetaData != null)
                                    {
                                        foreach (var pm in productMetaData.Where(pm => metaData.ContainsKey(pm.Key)))
                                        {
                                            metaData.Add(pm.Key, pm.Value);
                                        }
                                    }
                                }

                                stripeLineItem.PriceData.ProductData.Metadata = metaData;
                            }

                            if (UmbCheckoutSettings.IsLicensed)
                            {
                                if (product.HasValue(Shared.Consts.PropertyAlias.TaxRatesAlias))
                                {
                                    var taxRates = product.Value<IEnumerable<string>>(Shared.Consts.PropertyAlias.TaxRatesAlias)?.ToList();
                                    stripeLineItem.DynamicTaxRates = taxRates;
                                }
                            }

                            stripeLineItems.Add(stripeLineItem);
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
                        CancelUrl = cancelUri != null ? string.Concat(cancelUri.ToString(), "?session_id={CHECKOUT_SESSION_ID}&success=false") : string.Empty
                    };

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

                    if (configuration is { EnableShipping: true })
                    {
                        var shippingRates = await _stripeDatabaseService.GetShippingRates();
                        var shippingRatesList = shippingRates.ToList();

                        if (shippingRatesList.Any())
                        {
                            var shippingRateOptions = shippingRatesList.Select(shippingRate =>
                                new SessionShippingOptionOptions { ShippingRate = shippingRate.Value }).ToList();

                            options.ShippingOptions = shippingRateOptions;
                        }
                    }

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
