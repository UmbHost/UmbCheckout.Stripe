import { UmbSubmitWorkspaceAction } from "@umbraco-cms/backoffice/workspace";
import {
  UMBCHECKOUT_SHIPPING_RATE_COLLECTION_ALIAS,
  UMBCHECKOUT_SHIPPING_RATE_ENTITY_TYPE,
  UMBCHECKOUT_SHIPPING_RATE_ROOT_ENTITY_TYPE,
  UMBCHECKOUT_SHIPPING_RATE_ROOT_WORKSPACE_ALIAS,
  UMBCHECKOUT_SHIPPING_RATE_WORKSPACE_ALIAS,
} from "../entity.js";

export const manifests: Array<UmbExtensionManifest> = [
  {
    // Same pattern as Umbraco's own webhook root: a default-kind workspace plus a collection-kind workspace view.
    type: "workspace",
    kind: "default",
    alias: UMBCHECKOUT_SHIPPING_RATE_ROOT_WORKSPACE_ALIAS,
    name: "UmbCheckout Shipping Rate Root Workspace",
    meta: { entityType: UMBCHECKOUT_SHIPPING_RATE_ROOT_ENTITY_TYPE, headline: "#umbcheckoutStripe_shipping_rates" },
  },
  {
    type: "workspaceView",
    kind: "collection",
    alias: "UmbCheckout.WorkspaceView.ShippingRateRoot.Collection",
    name: "UmbCheckout Shipping Rate Root Collection View",
    meta: { label: "Collection", pathname: "collection", icon: "icon-layers", collectionAlias: UMBCHECKOUT_SHIPPING_RATE_COLLECTION_ALIAS },
    conditions: [{ alias: "Umb.Condition.WorkspaceAlias", match: UMBCHECKOUT_SHIPPING_RATE_ROOT_WORKSPACE_ALIAS }],
  },
  {
    type: "workspace",
    kind: "routable",
    alias: UMBCHECKOUT_SHIPPING_RATE_WORKSPACE_ALIAS,
    name: "UmbCheckout Shipping Rate Workspace",
    api: () => import("./shipping-rate-workspace.context.js"),
    meta: { entityType: UMBCHECKOUT_SHIPPING_RATE_ENTITY_TYPE },
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
