using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stripe;
using UmbCheckout.Stripe.Interfaces;

namespace UmbCheckout.Stripe.Controllers.BackOffice
{
    /// <summary>
    /// Retrieves and manages the Stripe shipping rates for the backoffice
    /// </summary>
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "Stripe")]
    public class StripeShippingRatesController : UmbCheckoutStripeManagementApiControllerBase
    {
        private readonly IStripeShippingRateDatabaseService _stripeDatabaseService;
        private readonly IStripeShippingRateApiService _stripeShippingRateApiService;
        private readonly ILogger<StripeShippingRatesController> _logger;

        public StripeShippingRatesController(IStripeShippingRateDatabaseService stripeDatabaseService, IStripeShippingRateApiService stripeShippingRateApiService, ILogger<StripeShippingRatesController> logger)
        {
            _stripeDatabaseService = stripeDatabaseService;
            _stripeShippingRateApiService = stripeShippingRateApiService;
            _logger = logger;
        }

        /// <summary>
        /// Gets the Shipping Rates stored in the database
        /// </summary>
        [HttpGet("shipping-rates")]
        [ProducesResponseType<IEnumerable<Models.ShippingRate>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetShippingRates()
        {
            try
            {
                var shippingRates = await _stripeDatabaseService.GetShippingRates();

                return Ok(shippingRates);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Gets a Shipping Rate stored in the database
        /// </summary>
        /// <param name="key">Key of the Shipping Rate</param>
        [HttpGet("shipping-rates/{key:guid}")]
        [ProducesResponseType<Models.ShippingRate>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetShippingRate(Guid key)
        {
            try
            {
                var shippingRate = await _stripeDatabaseService.GetShippingRate(key);

                if (shippingRate == null)
                {
                    return NotFound();
                }

                return Ok(shippingRate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Creates a Shipping Rate
        /// </summary>
        /// <param name="model">The Shipping Rate values</param>
        [HttpPost("shipping-rates")]
        [ProducesResponseType<Models.ShippingRate>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateShippingRate([FromBody] Models.ShippingRateRequestModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                var createdShippingRate = await _stripeDatabaseService.CreateShippingRate(new Models.ShippingRate
                {
                    Name = model.Name,
                    Value = model.Value
                });

                if (createdShippingRate == null)
                {
                    return BadRequest();
                }

                return Ok(createdShippingRate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Updates a Shipping Rate
        /// </summary>
        /// <param name="key">Key of the Shipping Rate</param>
        /// <param name="model">The Shipping Rate values</param>
        [HttpPut("shipping-rates/{key:guid}")]
        [ProducesResponseType<Models.ShippingRate>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateShippingRate(Guid key, [FromBody] Models.ShippingRateRequestModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                var existingShippingRate = await _stripeDatabaseService.GetShippingRate(key);
                if (existingShippingRate == null)
                {
                    return NotFound();
                }

                var updatedShippingRate = await _stripeDatabaseService.UpdateShippingRate(new Models.ShippingRate
                {
                    Key = key,
                    Name = model.Name,
                    Value = model.Value
                });

                return Ok(updatedShippingRate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Deletes a Shipping Rate
        /// </summary>
        /// <param name="key">Key of the Shipping Rate</param>
        [HttpDelete("shipping-rates/{key:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteShippingRate(Guid key)
        {
            try
            {
                var existingShippingRate = await _stripeDatabaseService.GetShippingRate(key);
                if (existingShippingRate == null)
                {
                    return NotFound();
                }

                await _stripeDatabaseService.DeleteShippingRate(key);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Gets the Shipping Rates from the Stripe API
        /// </summary>
        [HttpGet("stripe-shipping-rates")]
        [ProducesResponseType<IEnumerable<Models.StripeShippingRateModel>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStripeShippingRates()
        {
            try
            {
                var shippingRates = await _stripeShippingRateApiService.GetShippingRates();

                return Ok(shippingRates.Select(ToModel));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Gets a Shipping Rate from the Stripe API
        /// </summary>
        /// <param name="id">Id of the Stripe Shipping Rate</param>
        [HttpGet("stripe-shipping-rates/{id}")]
        [ProducesResponseType<Models.StripeShippingRateModel>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStripeShippingRate(string id)
        {
            try
            {
                var shippingRate = await _stripeShippingRateApiService.GetShippingRate(id);

                return Ok(ToModel(shippingRate));
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, ex.Message);
                return NotFound();
            }
        }

        private static Models.StripeShippingRateModel ToModel(ShippingRate shippingRate)
        {
            return new Models.StripeShippingRateModel
            {
                Id = shippingRate.Id,
                DisplayName = shippingRate.DisplayName ?? string.Empty,
                Active = shippingRate.Active,
                Amount = shippingRate.FixedAmount?.Amount,
                Currency = shippingRate.FixedAmount?.Currency,
                DeliveryEstimateMinimumValue = shippingRate.DeliveryEstimate?.Minimum?.Value,
                DeliveryEstimateMinimumUnit = shippingRate.DeliveryEstimate?.Minimum?.Unit,
                DeliveryEstimateMaximumValue = shippingRate.DeliveryEstimate?.Maximum?.Value,
                DeliveryEstimateMaximumUnit = shippingRate.DeliveryEstimate?.Maximum?.Unit
            };
        }
    }
}
