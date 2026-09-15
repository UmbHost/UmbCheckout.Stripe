using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Web.Common.Authorization;
using Umbraco.Cms.Web.Common.Routing;

namespace UmbCheckout.Stripe.Controllers.BackOffice
{
    /// <summary>
    /// Base controller for the UmbCheckout Stripe backoffice Management API, available at /umbraco/umbcheckout/api/v1/stripe
    /// </summary>
    [ApiController]
    [BackOfficeRoute(Shared.Consts.ApiName + "/api/v{version:apiVersion}/stripe")]
    [Authorize(Policy = AuthorizationPolicies.SectionAccessSettings)]
    [MapToApi(Shared.Consts.ApiName)]
    public abstract class UmbCheckoutStripeManagementApiControllerBase : ControllerBase
    {
    }
}
