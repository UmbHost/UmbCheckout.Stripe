﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using UmbCheckout.Shared;
using UmbCheckout.Stripe.Interfaces;
using UmbCheckout.Stripe.Models;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;

namespace UmbCheckout.Stripe.Controllers.BackOffice.Api
{
    [PluginController(Consts.PackageName)]
    public class StripeShippingRatesApiController : UmbracoAuthorizedApiController
    {
        private readonly ILogger<StripeShippingRatesApiController> _logger;
        private readonly IStripeShippingRateDatabaseService _stripeDatabaseService;
        private readonly IStripeShippingRateApiService _stripeShippingRateApiService;

        public StripeShippingRatesApiController(ILogger<StripeShippingRatesApiController> logger, IStripeShippingRateDatabaseService stripeDatabaseService, IStripeShippingRateApiService stripeShippingRateApiService)
        {
            _logger = logger;
            _stripeDatabaseService = stripeDatabaseService;
            _stripeShippingRateApiService = stripeShippingRateApiService;
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
                var shippingRateProperties = await GetShippingRateProperties(key);

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

        [HttpPatch]
        public async Task<IActionResult> UpdateShippingRate([FromBody] ShippingRate shippingRate)
        {
            if (ModelState.IsValid)
            {
                var updatedShippingRate = await _stripeDatabaseService.UpdateShippingRate(shippingRate);
                var shippingRateProperties = await GetShippingRateProperties(updatedShippingRate?.Key);
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

        private async Task<List<Property>> GetShippingRateProperties(Guid? key)
        {
            try
            {
                var shippingRate = new ShippingRate();
                if (key != null)
                {
                    shippingRate = await _stripeDatabaseService.GetShippingRate(key.Value);
                }
                var backOfficeProperties = new List<Property>
                {
                    new()
                    {
                        Alias = "name",
                        Description = "The Shipping Rate Name",
                        Label = "Shipping Rate Name",
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
                        Description = "The Shipping Rate ID set in Stripe",
                        Label = "Shipping Rate ID",
                        Value = shippingRate != null ? shippingRate.Value : string.Empty,
                        View = "textbox",
                        Validation = new Validation
                        {
                            Mandatory = true
                        }
                    }
                };

                return backOfficeProperties;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}