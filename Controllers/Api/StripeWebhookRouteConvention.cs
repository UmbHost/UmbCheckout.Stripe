using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace UmbCheckout.Stripe.Controllers.Api
{
    /// <summary>
    /// Lets a site replace the webhook controller by subclassing <see cref="StripeWebhookApiController"/>: exactly one webhook
    /// endpoint answers. A subclass without its own [Route] inherits the package's route, which matched both controllers
    /// (AmbiguousMatchException, a 500 Stripe retries forever); a subclass with its own [Route] left the package's controller
    /// handling events at the old URL as well. When a subclass is registered, the package's own controller is removed, and a
    /// subclass that declares its own [Route] keeps only its own routes. Without a subclass nothing changes.
    /// </summary>
    internal sealed class StripeWebhookRouteConvention : IApplicationModelConvention
    {
        public void Apply(ApplicationModel application)
        {
            var baseType = typeof(StripeWebhookApiController);
            var subclasses = application.Controllers
                .Where(c => c.ControllerType != baseType.GetTypeInfo() && baseType.IsAssignableFrom(c.ControllerType))
                .ToList();
            if (subclasses.Count == 0)
            {
                return;
            }

            var inheritedTemplates = baseType.GetCustomAttributes<RouteAttribute>(inherit: false).Select(r => r.Template).ToHashSet(StringComparer.OrdinalIgnoreCase);
            foreach (var subclass in subclasses)
            {
                var ownTemplates = subclass.ControllerType.GetCustomAttributes<RouteAttribute>(inherit: false).Select(r => r.Template).ToHashSet(StringComparer.OrdinalIgnoreCase);
                if (ownTemplates.Count == 0)
                {
                    continue; // takes over the package's URL
                }

                foreach (var selector in subclass.Selectors.Where(s => s.AttributeRouteModel?.Template is { } template
                                                                        && inheritedTemplates.Contains(template) && !ownTemplates.Contains(template)).ToList())
                {
                    subclass.Selectors.Remove(selector);
                }
            }

            foreach (var replaced in application.Controllers.Where(c => c.ControllerType == baseType.GetTypeInfo()).ToList())
            {
                application.Controllers.Remove(replaced);
            }
        }
    }
}
