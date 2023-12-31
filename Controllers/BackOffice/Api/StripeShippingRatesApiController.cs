﻿using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using UmbCheckout.Stripe.Interfaces;
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

                return new JsonResult(shippingRates, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
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

                return new JsonResult(shippingRate, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
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
                var shippingRateProperties = await GetShippingRateProperties(updatedShippingRate!);
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
