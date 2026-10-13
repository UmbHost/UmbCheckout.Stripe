using Umbraco.Cms.Web.Common.Authorization;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UmbCheckout.Core.ManagementApi;
using UmbCheckout.Stripe.Helpers;
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
    // Settings > UmbCheckout: changing these requires Settings section access, not just a back-office login.
    [Authorize(Policy = AuthorizationPolicies.SectionAccessSettings)]
    // Editors manage what is stored: reads here skip the Loaded hooks, so display changes aren't saved back.
    [SuppressLoadedHooks]
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
            var all = (await _database.GetShippingRatesAsync(HttpContext?.RequestAborted ?? default)).ToList();
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
            var rate = await _database.GetShippingRateAsync(id, HttpContext?.RequestAborted ?? default);
            return rate is null ? StatusResult(ShippingRateOperationStatus.NotFound) : Ok(ToResponse(rate));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateShippingRate([FromBody] CreateShippingRateRequestModel model)
        {
            if (await _database.GetShippingRateAsync(model.Value, HttpContext?.RequestAborted ?? default) is not null)
            {
                return StatusResult(ShippingRateOperationStatus.DuplicateValue);
            }

            var created = await _database.CreateShippingRateAsync(new ShippingRate { Key = model.Id ?? Guid.NewGuid(), Name = model.Name, Value = model.Value }, HttpContext?.RequestAborted ?? default);
            return created is null
                ? UmbCheckoutCancellation.Result(HttpContext) ?? StatusResult(ShippingRateOperationStatus.SaveFailed)
                : CreatedAtId<StripeShippingRatesApiController>(controller => nameof(controller.GetShippingRateById), created.Key);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateShippingRate(Guid id, [FromBody] UpdateShippingRateRequestModel model)
        {
            var existing = await _database.GetShippingRateAsync(id, HttpContext?.RequestAborted ?? default);
            if (existing is null)
            {
                return StatusResult(ShippingRateOperationStatus.NotFound);
            }

            var duplicate = await _database.GetShippingRateAsync(model.Value, HttpContext?.RequestAborted ?? default);
            if (duplicate is not null && duplicate.Key != id)
            {
                return StatusResult(ShippingRateOperationStatus.DuplicateValue);
            }

            existing.Name = model.Name;
            existing.Value = model.Value;
            return await _database.UpdateShippingRateAsync(existing, HttpContext?.RequestAborted ?? default) is null
                ? UmbCheckoutCancellation.Result(HttpContext) ?? StatusResult(ShippingRateOperationStatus.SaveFailed)
                : Ok();
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteShippingRate(Guid id)
            => await _database.DeleteShippingRateAsync(id, HttpContext?.RequestAborted ?? default)
                ? Ok()
                : UmbCheckoutCancellation.Result(HttpContext, UmbCheckoutProblemDetails.DeleteCancelledTitle) ?? StatusResult(ShippingRateOperationStatus.NotFound);

        [HttpGet("stripe")]
        [ProducesResponseType(typeof(IEnumerable<StripeShippingRateResponseModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetStripeShippingRates()
        {
            try
            {
                var rates = await _stripe.GetShippingRates(HttpContext?.RequestAborted ?? default);
                return Ok(rates.Data.Select(StripeShippingRateResponseModel.From));
            }
            catch (global::Stripe.StripeException ex)
            {
                return StripeErrorResult(ex);
            }
            catch (Exception ex) when (IsStripeUnavailable(ex))
            {
                return StripeUnavailableResult(ex);
            }
        }

        [HttpGet("stripe/{stripeId}")]
        [ProducesResponseType(typeof(StripeShippingRateResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStripeShippingRateById(string stripeId)
        {
            try
            {
                return Ok(StripeShippingRateResponseModel.From(await _stripe.GetShippingRate(stripeId, HttpContext?.RequestAborted ?? default)));
            }
            catch (global::Stripe.StripeException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return StatusResult(ShippingRateOperationStatus.NotFound);
            }
            catch (global::Stripe.StripeException ex)
            {
                return StripeErrorResult(ex);
            }
            catch (Exception ex) when (IsStripeUnavailable(ex))
            {
                return StripeUnavailableResult(ex);
            }
        }

        /// <summary>Stripe has no API key for the active mode, or could not be reached (network failure or timeout).</summary>
        private bool IsStripeUnavailable(Exception ex)
            => ex is StripeNotConfiguredException
               || (ex is HttpRequestException or TaskCanceledException && HttpContext?.RequestAborted.IsCancellationRequested != true);

        private IActionResult StripeUnavailableResult(Exception ex) => ex is StripeNotConfiguredException
            ? OperationStatusResult(ShippingRateOperationStatus.StripeNotConfigured, problemDetails => BadRequest(problemDetails
                .WithTitle(UmbCheckoutProblemDetails.StripeNotConfiguredTitle).WithDetail(ex.Message).Build()))
            : OperationStatusResult(ShippingRateOperationStatus.StripeUnreachable, problemDetails => StatusCode(StatusCodes.Status502BadGateway, problemDetails
                .WithTitle(UmbCheckoutProblemDetails.StripeUnreachableTitle).WithDetail(ex.Message).Build()));

        private static ShippingRateResponseModel ToResponse(ShippingRate rate) => new() { Id = rate.Key, Name = rate.Name, Value = rate.Value };

        private IActionResult StripeErrorResult(global::Stripe.StripeException ex)
        {
            // A rejected key's message echoes the key's last 4 characters: a fixed detail is shown and logged instead.
            var detail = StripeKeyRejection.DetailFor(ex);
            if (StripeKeyRejection.IsRejected(ex))
            {
                HttpContext?.RequestServices?.GetService<ILoggerFactory>()?.CreateLogger<StripeShippingRatesApiController>()
                    .LogWarning("Stripe request failed in {Controller}: {StripeError}", nameof(StripeShippingRatesApiController), detail);
            }

            return OperationStatusResult(ShippingRateOperationStatus.StripeError, problemDetails => BadRequest(problemDetails
                .WithTitle(UmbCheckoutProblemDetails.StripeErrorTitle)
                .WithDetail(detail)
                .Build()));
        }

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
