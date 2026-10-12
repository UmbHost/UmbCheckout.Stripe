import { UMBCHECKOUT_SETTINGS_MENU_ALIAS as p, UMBCHECKOUT_STRIPE_MENU_ALIAS as s } from "@umbcheckout/core";
import { UmbSubmitWorkspaceAction as c } from "@umbraco-cms/backoffice/workspace";
const m = [
  {
    name: "UmbCheckout.Stripe.Entrypoint",
    alias: "UmbCheckout.Stripe.Entrypoint",
    type: "backofficeEntryPoint",
    js: () => import("./entrypoint-CR81ki1e.js")
  }
], l = [
  {
    type: "localization",
    alias: "UmbCheckout.Stripe.Localization.En",
    name: "UmbCheckout Stripe English",
    js: () => import("./en-BcE1oaSe.js"),
    meta: { culture: "en" }
  }
], r = [
  {
    type: "menuItem",
    alias: "UmbCheckout.MenuItem.Stripe",
    name: "UmbCheckout Stripe Menu Item",
    // Below Configuration (weight 400), as in v13.
    weight: 300,
    element: () => import("./stripe-menu-item.element-DgZ51gdV.js"),
    meta: { label: "#umbcheckoutStripe_stripe", icon: "icon-credit-card", menus: [p] }
  }
], h = "umbcheckout-shipping-rate", i = "umbcheckout-shipping-rates", S = `section/settings/workspace/${i}`, o = "UmbCheckout.Repository.ShippingRate.Detail", C = "UmbCheckout.Store.ShippingRate.Detail", n = "UmbCheckout.Repository.ShippingRate.Item", _ = "UmbCheckout.Store.ShippingRate.Item", a = "UmbCheckout.Repository.ShippingRate.Collection", e = "UmbCheckout.Collection.ShippingRate", t = "UmbCheckout.Workspace.ShippingRate", U = [
  {
    type: "repository",
    alias: o,
    name: "UmbCheckout Shipping Rate Detail Repository",
    api: () => import("./shipping-rate-detail.repository-BC8UmFZz.js")
  },
  {
    type: "store",
    alias: C,
    name: "UmbCheckout Shipping Rate Detail Store",
    api: () => import("./shipping-rate-detail.store-DxFU_GlC.js")
  }
], u = [
  {
    type: "repository",
    alias: n,
    name: "UmbCheckout Shipping Rate Item Repository",
    api: () => import("./shipping-rate-item.repository-E6vC0ZNW.js")
  },
  {
    type: "itemStore",
    alias: _,
    name: "UmbCheckout Shipping Rate Item Store",
    api: () => import("./shipping-rate-item.store-BGZlqPk2.js")
  }
], b = [
  {
    type: "repository",
    alias: a,
    name: "UmbCheckout Shipping Rate Collection Repository",
    api: () => import("./shipping-rate-collection.repository-CXcITiE4.js")
  },
  {
    type: "collection",
    kind: "default",
    alias: e,
    name: "UmbCheckout Shipping Rate Collection",
    meta: { repositoryAlias: a, noItemsLabel: "#umbcheckoutStripe_no_shipping_rates" }
  },
  {
    type: "collectionView",
    alias: "UmbCheckout.CollectionView.ShippingRate.Table",
    name: "UmbCheckout Shipping Rate Table View",
    element: () => import("./shipping-rate-table-collection-view.element-Dc8wrBFi.js"),
    meta: { label: "Table", icon: "icon-list", pathName: "table" },
    conditions: [{ alias: "Umb.Condition.CollectionAlias", match: e }]
  },
  {
    type: "collectionAction",
    kind: "button",
    alias: "UmbCheckout.CollectionAction.ShippingRate.Create",
    name: "Create Shipping Rate",
    meta: { label: "#umbcheckoutStripe_create_shipping_rate", href: `${S}/create` },
    conditions: [{ alias: "Umb.Condition.CollectionAlias", match: e }]
  }
], k = [
  {
    type: "workspace",
    kind: "routable",
    alias: t,
    name: "UmbCheckout Shipping Rate Workspace",
    api: () => import("./shipping-rate-workspace.context-dPJ1dEQq.js"),
    // One workspace for the list (""), create and edit, so the menu item stays highlighted on a shipping rate.
    meta: { entityType: i }
  },
  {
    type: "workspaceView",
    alias: "UmbCheckout.WorkspaceView.ShippingRate.Details",
    name: "UmbCheckout Shipping Rate Details View",
    element: () => import("./shipping-rate-details-workspace-view.element-C0FrsUWs.js"),
    weight: 100,
    meta: { label: "#general_details", pathname: "details", icon: "icon-truck" },
    conditions: [{ alias: "Umb.Condition.WorkspaceAlias", match: t }]
  },
  {
    type: "workspaceAction",
    kind: "default",
    alias: "UmbCheckout.WorkspaceAction.ShippingRate.Save",
    name: "Save Shipping Rate",
    api: c,
    meta: { look: "primary", color: "positive", label: "#buttons_save" },
    conditions: [{ alias: "Umb.Condition.WorkspaceAlias", match: t }]
  }
], R = [
  {
    type: "entityAction",
    kind: "delete",
    alias: "UmbCheckout.EntityAction.ShippingRate.Delete",
    name: "Delete Shipping Rate",
    forEntityTypes: [h],
    meta: {
      detailRepositoryAlias: o,
      itemRepositoryAlias: n
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
      entityType: i,
      menus: [s]
    }
  }
], E = [...U, ...u, ...b, ...k, ...R, ...I], y = [...m, ...l, ...r, ...E];
export {
  h as U,
  S as a,
  e as b,
  o as c,
  t as d,
  i as e,
  y as m
};
//# sourceMappingURL=bundle.manifests-MLL5cmAF.js.map
