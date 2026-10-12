import { UMBCHECKOUT_SETTINGS_MENU_ALIAS as s, UMBCHECKOUT_STRIPE_MENU_ALIAS as m } from "@umbcheckout/core";
import { UmbSubmitWorkspaceAction as l } from "@umbraco-cms/backoffice/workspace";
const r = [
  {
    name: "UmbCheckout.Stripe.Entrypoint",
    alias: "UmbCheckout.Stripe.Entrypoint",
    type: "backofficeEntryPoint",
    js: () => import("./entrypoint-CR81ki1e.js")
  }
], h = [
  {
    type: "localization",
    alias: "UmbCheckout.Stripe.Localization.En",
    name: "UmbCheckout Stripe English",
    js: () => import("./en-CU3-f1_k.js"),
    meta: { culture: "en" }
  }
], C = [
  {
    type: "menuItem",
    alias: "UmbCheckout.MenuItem.Stripe",
    name: "UmbCheckout Stripe Menu Item",
    // Below Configuration (weight 400), as in v13.
    weight: 300,
    element: () => import("./stripe-menu-item.element-CP8G9Na8.js"),
    meta: { label: "#umbcheckoutStripe_stripe", icon: "icon-credit-card", menus: [s] }
  }
], a = "umbcheckout-shipping-rate", n = "umbcheckout-shipping-rate-root", p = "UmbCheckout.Repository.ShippingRate.Detail", S = "UmbCheckout.Store.ShippingRate.Detail", c = "UmbCheckout.Repository.ShippingRate.Item", k = "UmbCheckout.Store.ShippingRate.Item", i = "UmbCheckout.Repository.ShippingRate.Collection", e = "UmbCheckout.Collection.ShippingRate", t = "UmbCheckout.Workspace.ShippingRate", o = "UmbCheckout.Workspace.ShippingRateRoot", u = [
  {
    type: "repository",
    alias: p,
    name: "UmbCheckout Shipping Rate Detail Repository",
    api: () => import("./shipping-rate-detail.repository-DkD1NlMs.js")
  },
  {
    type: "store",
    alias: S,
    name: "UmbCheckout Shipping Rate Detail Store",
    api: () => import("./shipping-rate-detail.store-DxFU_GlC.js")
  }
], U = [
  {
    type: "repository",
    alias: c,
    name: "UmbCheckout Shipping Rate Item Repository",
    api: () => import("./shipping-rate-item.repository-BGeUj005.js")
  },
  {
    type: "itemStore",
    alias: k,
    name: "UmbCheckout Shipping Rate Item Store",
    api: () => import("./shipping-rate-item.store-BGZlqPk2.js")
  }
], _ = [
  {
    type: "repository",
    alias: i,
    name: "UmbCheckout Shipping Rate Collection Repository",
    api: () => import("./shipping-rate-collection.repository-BDqJvWYn.js")
  },
  {
    type: "collection",
    kind: "default",
    alias: e,
    name: "UmbCheckout Shipping Rate Collection",
    meta: { repositoryAlias: i, noItemsLabel: "#umbcheckoutStripe_no_shipping_rates" }
  },
  {
    type: "collectionView",
    alias: "UmbCheckout.CollectionView.ShippingRate.Table",
    name: "UmbCheckout Shipping Rate Table View",
    element: () => import("./shipping-rate-table-collection-view.element-Cs34SgI_.js"),
    meta: { label: "Table", icon: "icon-list", pathName: "table" },
    conditions: [{ alias: "Umb.Condition.CollectionAlias", match: e }]
  },
  {
    type: "collectionAction",
    kind: "button",
    alias: "UmbCheckout.CollectionAction.ShippingRate.Create",
    name: "Create Shipping Rate",
    meta: { label: "#umbcheckoutStripe_create_shipping_rate", href: "section/settings/workspace/umbcheckout-shipping-rate/create" },
    conditions: [{ alias: "Umb.Condition.CollectionAlias", match: e }]
  }
], b = [
  {
    // Same pattern as Umbraco's own webhook root: a default-kind workspace plus a collection-kind workspace view.
    type: "workspace",
    kind: "default",
    alias: o,
    name: "UmbCheckout Shipping Rate Root Workspace",
    meta: { entityType: n, headline: "#umbcheckoutStripe_shipping_rates" }
  },
  {
    type: "workspaceView",
    kind: "collection",
    alias: "UmbCheckout.WorkspaceView.ShippingRateRoot.Collection",
    name: "UmbCheckout Shipping Rate Root Collection View",
    meta: { label: "Collection", pathname: "collection", icon: "icon-layers", collectionAlias: e },
    conditions: [{ alias: "Umb.Condition.WorkspaceAlias", match: o }]
  },
  {
    type: "workspace",
    kind: "routable",
    alias: t,
    name: "UmbCheckout Shipping Rate Workspace",
    api: () => import("./shipping-rate-workspace.context-C93ups2T.js"),
    meta: { entityType: a }
  },
  {
    type: "workspaceView",
    alias: "UmbCheckout.WorkspaceView.ShippingRate.Details",
    name: "UmbCheckout Shipping Rate Details View",
    element: () => import("./shipping-rate-details-workspace-view.element-DOxQQdrM.js"),
    weight: 100,
    meta: { label: "#general_details", pathname: "details", icon: "icon-truck" },
    conditions: [{ alias: "Umb.Condition.WorkspaceAlias", match: t }]
  },
  {
    type: "workspaceAction",
    kind: "default",
    alias: "UmbCheckout.WorkspaceAction.ShippingRate.Save",
    name: "Save Shipping Rate",
    api: l,
    meta: { look: "primary", color: "positive", label: "#buttons_save" },
    conditions: [{ alias: "Umb.Condition.WorkspaceAlias", match: t }]
  }
], R = [
  {
    type: "entityAction",
    kind: "delete",
    alias: "UmbCheckout.EntityAction.ShippingRate.Delete",
    name: "Delete Shipping Rate",
    forEntityTypes: [a],
    meta: {
      detailRepositoryAlias: p,
      itemRepositoryAlias: c
    }
  }
], I = [
  {
    type: "menuItem",
    alias: "UmbCheckout.MenuItem.ShippingRates",
    name: "UmbCheckout Shipping Rates Menu Item",
    weight: 200,
    meta: {
      label: "#umbcheckoutStripe_shipping_rates",
      icon: "icon-truck",
      entityType: n,
      menus: [m]
    }
  }
], T = [...u, ...U, ..._, ...b, ...R, ...I], y = [...r, ...h, ...C, ...T];
export {
  a as U,
  p as a,
  t as b,
  n as c,
  y as m
};
//# sourceMappingURL=bundle.manifests-Cs23f-QK.js.map
