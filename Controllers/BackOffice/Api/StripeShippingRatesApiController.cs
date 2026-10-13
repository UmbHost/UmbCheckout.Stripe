using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Nodes;
using UmbCheckout.Stripe.Interfaces;
using UmbCheckout.Stripe.Money;
using UmbCheckout.Stripe.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;

namespace UmbCheckout.Stripe.Controllers.BackOffice.Api
{
    [PluginController(Shared.Consts.PackageName)]
    public class StripeShippingRatesApiController : UmbracoAuthorizedApiController
    {
        private readonly ILogger<StripeShippingRatesApiController> _logger;
        private readonly IStripeShippingRateDatabaseService _stripeDatabaseService;
        private readonly IStripeShippingRateApiService _stripeShippingRateApiService;
        private readonly ILocalizedTextService _localizedTextService;

        public StripeShippingRatesApiController(ILogger<StripeShippingRatesApiController> logger, IStripeShippingRateDatabaseService stripeDatabaseService, IStripeShippingRateApiService stripeShippingRateApiService, ILocalizedTextService localizedTextService)
        {
            _logger = logger;
            _stripeDatabaseService = stripeDatabaseService;
            _stripeShippingRateApiService = stripeShippingRateApiService;
            _localizedTextService = localizedTextService;
        }

        [HttpGet]
        public async Task<IActionResult> GetShippingRates()
        {
            try
            {
                var shippingRates = await _stripeDatabaseService.GetShippingRates();

                return new JsonResult(shippingRates, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStripeShippingRates()
        {
            try
            {
                var shippingRates = await _stripeShippingRateApiService.GetShippingRates();
                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

                // The response is serialised as before; each rate gains fixedAmountMoney (the amount in major units).
                var json = MoneyJson.Serialize(shippingRates, options);
                if (json?["data"] is JsonArray data && shippingRates?.Data is { } rates)
                {
                    for (var i = 0; i < data.Count && i < rates.Count; i++)
                    {
                        AddFixedAmountMoney(data[i], rates[i], options);
                    }
                }

                return new JsonResult(json, options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetShippingRate(Guid? key)
        {
            try
            {
                var shippingRate = key.HasValue ? await _stripeDatabaseService.GetShippingRate(key.Value) : new ShippingRate();
                var shippingRateProperties = await GetShippingRateProperties(shippingRate!);

                return new JsonResult(shippingRateProperties, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStripeShippingRate(string id)
        {
            try
            {
                var shippingRate = await _stripeShippingRateApiService.GetShippingRate(id);
                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

                // The response is serialised as before, plus fixedAmountMoney (the amount in major units).
                var json = MoneyJson.Serialize(shippingRate, options);
                AddFixedAmountMoney(json, shippingRate, options);

                return new JsonResult(json, options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        [HttpPut]
        public async Task<IActionResult> CreateShippingRate([FromBody] ShippingRate shippingRate)
        {
            if (ModelState.IsValid)
            {
                var createdShippingRate = await _stripeDatabaseService.CreateShippingRate(shippingRate);
                if (createdShippingRate != null)
                {
                    var shippingRateProperties = await GetShippingRateProperties(createdShippingRate);
                    return new JsonResult(shippingRateProperties, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }

            return BadRequest();
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateShippingRate([FromBody] ShippingRate shippingRate)
        {
            if (ModelState.IsValid)
            {
                var updatedShippingRate = await _stripeDatabaseService.UpdateShippingRate(shippingRate);
                if (updatedShippingRate == null)
                {
                    // Refused, e.g. the Stripe ID belongs to another shipping rate.
                    return BadRequest();
                }

                var shippingRateProperties = await GetShippingRateProperties(updatedShippingRate);
                return new JsonResult(shippingRateProperties, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            }

            return BadRequest();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteShippingRate([FromQuery] Guid key)
        {
            if (ModelState.IsValid)
            {
                var deletedShippingRate = await _stripeDatabaseService.DeleteShippingRate(key);

                if (deletedShippingRate == false)
                {
                    return BadRequest();
                }

                return Accepted();
            }

            return BadRequest();
        }

        private static void AddFixedAmountMoney(JsonNode? item, global::Stripe.ShippingRate? rate, JsonSerializerOptions options)
        {
            if (rate?.FixedAmount is { } fixedAmount)
            {
                MoneyJson.Add(item, "fixedAmountMoney", fixedAmount.Amount, fixedAmount.Currency, options);
            }
        }

        private Task<ShippingRateResponse> GetShippingRateProperties(ShippingRate shippingRate)
        {
            try
            {
                var backOfficeProperties = new List<Property>
                {
                    new()
                    {
                        Alias = "name",
                        Description = _localizedTextService.Localize(Consts.LocalizationKeys.Area, Consts.LocalizationKeys.ShippingRateDescription, CultureInfo.CurrentUICulture),
                        Label = _localizedTextService.Localize(Consts.LocalizationKeys.Area, Consts.LocalizationKeys.ShippingRateLabel, CultureInfo.CurrentUICulture),
                        Value = shippingRate != null ? shippingRate.Name : string.Empty,
                        View = "textbox",
                        Validation = new Validation
                        {
                            Mandatory = true
                        }
                    },
                    new()
                    {
                        Alias = "value",
                        Description = _localizedTextService.Localize(Consts.LocalizationKeys.Area, Consts.LocalizationKeys.ShippingRateIdDescription, CultureInfo.CurrentUICulture),
                        Label = _localizedTextService.Localize(Consts.LocalizationKeys.Area, Consts.LocalizationKeys.ShippingRateIdLabel, CultureInfo.CurrentUICulture),
                        Value = shippingRate != null ? shippingRate.Value : string.Empty,
                        View = "textbox",
                        Validation = new Validation
                        {
                            Mandatory = true
                        }
                    }
                };

                return Task.FromResult(new ShippingRateResponse{ Key = shippingRate!.Key, Properties = backOfficeProperties});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
