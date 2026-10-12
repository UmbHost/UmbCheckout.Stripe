import { UmbSubmitWorkspaceAction } from "@umbraco-cms/backoffice/workspace";
import {
  UMBCHECKOUT_SHIPPING_RATES_WORKSPACE_ENTITY_TYPE,
  UMBCHECKOUT_SHIPPING_RATE_WORKSPACE_ALIAS,
} from "../entity.js";

export const manifests: Array<UmbExtensionManifest> = [
  {
    type: "workspace",
    kind: "routable",
    alias: UMBCHECKOUT_SHIPPING_RATE_WORKSPACE_ALIAS,
    name: "UmbCheckout Shipping Rate Workspace",
    api: () => import("./shipping-rate-workspace.context.js"),
    // One workspace for the list (""), create and edit, so the menu item stays highlighted on a shipping rate.
    meta: { entityType: UMBCHECKOUT_SHIPPING_RATES_WORKSPACE_ENTITY_TYPE },
  },
  {
    type: "workspaceView",
    alias: "UmbCheckout.WorkspaceView.ShippingRate.Details",
    name: "UmbCheckout Shipping Rate Details View",
    element: () => import("./views/shipping-rate-details-workspace-view.element.js"),
    weight: 100,
    meta: { label: "#general_details", pathname: "details", icon: "icon-truck" },
    conditions: [{ alias: "Umb.Condition.WorkspaceAlias", match: UMBCHECKOUT_SHIPPING_RATE_WORKSPACE_ALIAS }],
  },
  {
    type: "workspaceAction",
    kind: "default",
    alias: "UmbCheckout.WorkspaceAction.ShippingRate.Save",
    name: "Save Shipping Rate",
    api: UmbSubmitWorkspaceAction,
    meta: { look: "primary", color: "positive", label: "#buttons_save" },
    conditions: [{ alias: "Umb.Condition.WorkspaceAlias", match: UMBCHECKOUT_SHIPPING_RATE_WORKSPACE_ALIAS }],
  },
];
