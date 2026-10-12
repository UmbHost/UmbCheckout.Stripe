using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UmbCheckout.Core.ManagementApi;
using UmbCheckout.Stripe.Composers;
using UmbCheckout.Stripe.Interfaces;
using UmbCheckout.Stripe.Models;
using UmbCheckout.Stripe.Models.Api;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Management.Controllers;
using Umbraco.Cms.Api.Management.Routing;
using Umbraco.Cms.Api.Common.ViewModels.Pagination;

namespace UmbCheckout.Stripe.Controllers.BackOffice.Api
{
    [ApiVersion("1.0")]
    [VersionedApiBackOfficeRoute("umbcheckout/stripe-shipping-rates")]
    [ApiExplorerSettings(GroupName = "Shipping Rates")]
    [MapToApi(StripeApiComposer.DocumentName)]
    public class StripeShippingRatesApiController : ManagementApiControllerBase
    {
        private readonly IStripeShippingRateDatabaseService _database;
        private readonly IStripeShippingRateApiService _stripe;

        public StripeShippingRatesApiController(IStripeShippingRateDatabaseService database, IStripeShippingRateApiService stripe)
        {
            _database = database;
            _stripe = stripe;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedViewModel<ShippingRateResponseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetShippingRates(int skip = 0, int take = 100)
        {
            var all = (await _database.GetShippingRates()).ToList();
            return Ok(new PagedViewModel<ShippingRateResponseModel>
            {
                Total = all.Count,
                Items = all.Skip(skip).Take(take).Select(ToResponse)
            });
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ShippingRateResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetShippingRateById(Guid id)
        {
            var rate = await _database.GetShippingRate(id);
            return rate is null ? StatusResult(ShippingRateOperationStatus.NotFound) : Ok(ToResponse(rate));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateShippingRate([FromBody] CreateShippingRateRequestModel model)
        {
            if (await _database.GetShippingRate(model.Value) is not null)
            {
                return StatusResult(ShippingRateOperationStatus.DuplicateValue);
            }

            var created = await _database.CreateShippingRate(new ShippingRate { Key = model.Id ?? Guid.NewGuid(), Name = model.Name, Value = model.Value });
            return created is null
                ? StatusResult(ShippingRateOperationStatus.SaveFailed)
                : CreatedAtId<StripeShippingRatesApiController>(controller => nameof(controller.GetShippingRateById), created.Key);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateShippingRate(Guid id, [FromBody] UpdateShippingRateRequestModel model)
        {
            var existing = await _database.GetShippingRate(id);
            if (existing is null)
            {
                return StatusResult(ShippingRateOperationStatus.NotFound);
            }

            var duplicate = await _database.GetShippingRate(model.Value);
            if (duplicate is not null && duplicate.Key != id)
            {
                return StatusResult(ShippingRateOperationStatus.DuplicateValue);
            }

            existing.Name = model.Name;
            existing.Value = model.Value;
            return await _database.UpdateShippingRate(existing) is null ? StatusResult(ShippingRateOperationStatus.SaveFailed) : Ok();
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteShippingRate(Guid id)
            => await _database.DeleteShippingRate(id) ? Ok() : StatusResult(ShippingRateOperationStatus.NotFound);

        [HttpGet("stripe")]
        [ProducesResponseType(typeof(IEnumerable<StripeShippingRateResponseModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetStripeShippingRates()
        {
            try
            {
                var rates = await _stripe.GetShippingRates();
                return Ok(rates.Data.Select(StripeShippingRateResponseModel.From));
            }
            catch (global::Stripe.StripeException ex)
            {
                return StripeErrorResult(ex);
            }
        }

        [HttpGet("stripe/{stripeId}")]
        [ProducesResponseType(typeof(StripeShippingRateResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStripeShippingRateById(string stripeId)
        {
            try
            {
                return Ok(StripeShippingRateResponseModel.From(await _stripe.GetShippingRate(stripeId)));
            }
            catch (global::Stripe.StripeException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return StatusResult(ShippingRateOperationStatus.NotFound);
            }
            catch (global::Stripe.StripeException ex)
            {
                return StripeErrorResult(ex);
            }
        }

        private static ShippingRateResponseModel ToResponse(ShippingRate rate) => new() { Id = rate.Key, Name = rate.Name, Value = rate.Value };

        private IActionResult StripeErrorResult(global::Stripe.StripeException ex)
            => OperationStatusResult(ShippingRateOperationStatus.StripeError, problemDetails => BadRequest(problemDetails
                .WithTitle(UmbCheckoutProblemDetails.StripeErrorTitle)
                .WithDetail(ex.StripeError?.Message ?? ex.Message)
                .Build()));

        private IActionResult StatusResult(ShippingRateOperationStatus status)
            => OperationStatusResult(status, problemDetails => status switch
            {
                ShippingRateOperationStatus.NotFound => NotFound(problemDetails
                    .WithTitle(UmbCheckoutProblemDetails.NotFoundTitle)
                    .WithDetail("The shipping rate could not be found.")
                    .Build()),
                ShippingRateOperationStatus.DuplicateValue => BadRequest(problemDetails
                    .WithTitle("A shipping rate with this Stripe ID already exists")
                    .WithDetail("Choose a different Stripe shipping rate ID.")
                    .Build()),
                _ => BadRequest(problemDetails
                    .WithTitle("Shipping rate not saved")
                    .WithDetail("The shipping rate could not be saved.")
                    .Build())
            });
    }
}
