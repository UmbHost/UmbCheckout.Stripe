import { UmbSubmitWorkspaceAction as s } from "@umbraco-cms/backoffice/workspace";
import { UMBCHECKOUT_SETTINGS_MENU_ALIAS as m } from "@umbcheckout/core";
const l = [
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
], a = "umbcheckout-shipping-rate", n = "umbcheckout-shipping-rate-root", p = "UmbCheckout.Repository.ShippingRate.Detail", r = "UmbCheckout.Store.ShippingRate.Detail", c = "UmbCheckout.Repository.ShippingRate.Item", C = "UmbCheckout.Store.ShippingRate.Item", i = "UmbCheckout.Repository.ShippingRate.Collection", e = "UmbCheckout.Collection.ShippingRate", t = "UmbCheckout.Workspace.ShippingRate", o = "UmbCheckout.Workspace.ShippingRateRoot", S = [
  {
    type: "repository",
    alias: p,
    name: "UmbCheckout Shipping Rate Detail Repository",
    api: () => import("./shipping-rate-detail.repository-1qkPMrb2.js")
  },
  {
    type: "store",
    alias: r,
    name: "UmbCheckout Shipping Rate Detail Store",
    api: () => import("./shipping-rate-detail.store-DxFU_GlC.js")
  }
], k = [
  {
    type: "repository",
    alias: c,
    name: "UmbCheckout Shipping Rate Item Repository",
    api: () => import("./shipping-rate-item.repository-B6NyatbT.js")
  },
  {
    type: "itemStore",
    alias: C,
    name: "UmbCheckout Shipping Rate Item Store",
    api: () => import("./shipping-rate-item.store-BGZlqPk2.js")
  }
], R = [
  {
    type: "repository",
    alias: i,
    name: "UmbCheckout Shipping Rate Collection Repository",
    api: () => import("./shipping-rate-collection.repository-DaVKWrRQ.js")
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
    element: () => import("./shipping-rate-table-collection-view.element-CAttjCqO.js"),
    meta: { label: "Table", icon: "icon-list", pathName: "table" },
    conditions: [{ alias: "Umb.Condition.CollectionAlias", match: e }]
  },
  {
    type: "collectionAction",
    kind: "button",
    alias: "UmbCheckout.CollectionAction.ShippingRate.Create",
    name: "Create Shipping Rate",
    meta: { label: "#umbcheckoutStripe_create_shipping_rate", href: "section/umbcheckout/workspace/umbcheckout-shipping-rate/create" },
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
    api: () => import("./shipping-rate-workspace.context-BGr-F0L2.js"),
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
    api: s,
    meta: { look: "primary", color: "positive", label: "#buttons_save" },
    conditions: [{ alias: "Umb.Condition.WorkspaceAlias", match: t }]
  }
], u = [
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
], _ = [
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
], U = [...S, ...k, ...R, ...b, ...u, ..._], A = [...l, ...h, ...U];
export {
  a as U,
  p as a,
  t as b,
  n as c,
  A as m
};
//# sourceMappingURL=bundle.manifests-WznIAHn2.js.map
