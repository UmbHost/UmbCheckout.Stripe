using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Trees;
using Umbraco.Cms.Web.BackOffice.Trees;

namespace UmbCheckout.Stripe.NotificationHandlers
{
    public class StripeTreeNotificationHandler : INotificationHandler<TreeNodesRenderingNotification>
    {
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly UmbracoApiControllerTypeCollection _apiControllers;
        private readonly ILocalizedTextService _localizedTextService;

        public StripeTreeNotificationHandler(IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor, UmbracoApiControllerTypeCollection apiControllers, ILocalizedTextService localizedTextService)
        {
            _urlHelperFactory = urlHelperFactory;
            _actionContextAccessor = actionContextAccessor;
            _apiControllers = apiControllers;
            _localizedTextService = localizedTextService;
        }

        public void Handle(TreeNodesRenderingNotification notification)
        {
            if (notification.TreeAlias.Equals(Shared.Consts.TreeAlias) && notification.Id == "-1")
            {
                notification.Nodes.Add(CreateTreeNode("2", "1", notification.QueryString, _localizedTextService.Localize(Consts.LocalizationKeys.Area, Consts.LocalizationKeys.Stripe, CultureInfo.CurrentUICulture), "icon-credit-card", true, $"{Constants.Applications.Settings}/{"UmbCheckout"}/{"dashboard"}"));
            }
        }

        public TreeNode CreateTreeNode(string id, string parentId, FormCollection queryStrings, string title, string icon, bool hasChildren, string routePath)
        {
            if (_actionContextAccessor.ActionContext != null)
            {
                var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

                if (_actionContextAccessor.ActionContext.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
                {
                    var jsonUrl = urlHelper.GetTreeUrl(_apiControllers, controllerActionDescriptor.ControllerTypeInfo, id, queryStrings);
                    var menuUrl = urlHelper.GetMenuUrl(_apiControllers, controllerActionDescriptor.ControllerTypeInfo, id, queryStrings);
                    return new TreeNode(id, parentId, jsonUrl, menuUrl) { Name = title, RoutePath = routePath, Icon = icon, HasChildren = hasChildren};
                }
            }

            return new TreeNode(id, parentId, string.Empty, string.Empty) { Name = title, RoutePath = routePath, Icon = icon };
        }
    }
}
