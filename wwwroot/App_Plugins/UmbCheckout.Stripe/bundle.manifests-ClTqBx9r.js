import { UMBCHECKOUT_SETTINGS_MENU_ALIAS as S, UMBCHECKOUT_STRIPE_MENU_ALIAS as n } from "@umbcheckout/core";
import { UmbSubmitWorkspaceAction as s } from "@umbraco-cms/backoffice/workspace";
const r = [
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
    js: () => import("./en-DbsOn35s.js"),
    meta: { culture: "en" }
  }
], h = [
  {
    type: "menuItem",
    alias: "UmbCheckout.MenuItem.Stripe",
    name: "UmbCheckout Stripe Menu Item",
    // Below Configuration (weight 400), as in v13.
    weight: 300,
    element: () => import("./stripe-menu-item.element-DgZ51gdV.js"),
    meta: { label: "#umbcheckoutStripe_stripe", icon: "icon-credit-card", menus: [S] }
  }
], C = "umbcheckout-shipping-rate", o = "umbcheckout-shipping-rates", U = `section/settings/workspace/${o}`, p = "UmbCheckout.Repository.ShippingRate.Detail", u = "UmbCheckout.Store.ShippingRate.Detail", c = "UmbCheckout.Repository.ShippingRate.Item", _ = "UmbCheckout.Store.ShippingRate.Item", a = "UmbCheckout.Repository.ShippingRate.Collection", t = "UmbCheckout.Collection.ShippingRate", e = "UmbCheckout.Workspace.ShippingRate", k = [
  {
    type: "repository",
    alias: p,
    name: "UmbCheckout Shipping Rate Detail Repository",
    api: () => import("./shipping-rate-detail.repository-C-jaZEcL.js")
  },
  {
    type: "store",
    alias: u,
    name: "UmbCheckout Shipping Rate Detail Store",
    api: () => import("./shipping-rate-detail.store-DxFU_GlC.js")
  }
], b = [
  {
    type: "repository",
    alias: c,
    name: "UmbCheckout Shipping Rate Item Repository",
    api: () => import("./shipping-rate-item.repository-Bc2EUfRI.js")
  },
  {
    type: "itemStore",
    alias: _,
    name: "UmbCheckout Shipping Rate Item Store",
    api: () => import("./shipping-rate-item.store-BGZlqPk2.js")
  }
], T = [
  {
    type: "repository",
    alias: a,
    name: "UmbCheckout Shipping Rate Collection Repository",
    api: () => import("./shipping-rate-collection.repository-DdRZMqrA.js")
  },
  {
    type: "collection",
    kind: "default",
    alias: t,
    name: "UmbCheckout Shipping Rate Collection",
    meta: { repositoryAlias: a, noItemsLabel: "#umbcheckoutStripe_no_shipping_rates" }
  },
  {
    type: "collectionView",
    alias: "UmbCheckout.CollectionView.ShippingRate.Table",
    name: "UmbCheckout Shipping Rate Table View",
    element: () => import("./shipping-rate-table-collection-view.element-paaAtNDt.js"),
    meta: { label: "Table", icon: "icon-list", pathName: "table" },
    conditions: [{ alias: "Umb.Condition.CollectionAlias", match: t }]
  },
  {
    type: "collectionAction",
    kind: "button",
    alias: "UmbCheckout.CollectionAction.ShippingRate.Create",
    name: "Create Shipping Rate",
    meta: { label: "#umbcheckoutStripe_create_shipping_rate", href: `${U}/create` },
    conditions: [{ alias: "Umb.Condition.CollectionAlias", match: t }]
  }
], I = [
  {
    type: "workspace",
    kind: "routable",
    alias: e,
    name: "UmbCheckout Shipping Rate Workspace",
    api: () => import("./shipping-rate-workspace.context-CTYZ7V5_.js"),
    // One workspace for the list (""), create and edit, so the menu item stays highlighted on a shipping rate.
    meta: { entityType: o }
  },
  {
    type: "workspaceView",
    alias: "UmbCheckout.WorkspaceView.ShippingRate.Details",
    name: "UmbCheckout Shipping Rate Details View",
    element: () => import("./shipping-rate-details-workspace-view.element-DDTe0Zja.js"),
    weight: 100,
    meta: { label: "#general_details", pathname: "details", icon: "icon-truck" },
    conditions: [{ alias: "Umb.Condition.WorkspaceAlias", match: e }]
  },
  {
    type: "workspaceAction",
    kind: "default",
    alias: "UmbCheckout.WorkspaceAction.ShippingRate.Save",
    name: "Save Shipping Rate",
    api: s,
    meta: { look: "primary", color: "positive", label: "#buttons_save" },
    conditions: [{ alias: "Umb.Condition.WorkspaceAlias", match: e }]
  }
], E = [
  {
    type: "entityAction",
    kind: "delete",
    alias: "UmbCheckout.EntityAction.ShippingRate.Delete",
    name: "Delete Shipping Rate",
    // Umbraco's delete with v13's "Shipping Rate deleted" toast.
    api: () => import("./shipping-rate-delete.action-BIrRved4.js"),
    forEntityTypes: [C],
    meta: {
      detailRepositoryAlias: p,
      itemRepositoryAlias: c
    }
  }
], R = [
  {
    type: "menuItem",
    alias: "UmbCheckout.MenuItem.ShippingRates",
    name: "UmbCheckout Shipping Rates Menu Item",
    weight: 200,
    meta: {
      label: "#umbcheckoutStripe_shipping_rates",
      icon: "icon-truck",
      entityType: o,
      menus: [n]
    }
  }
], g = [...k, ...b, ...T, ...I, ...E, ...R], m = "umbcheckout-stripe-settings", N = "stripe-settings", A = "UmbCheckout.Repository.StripeSettings", y = "UmbCheckout.Store.StripeSettings", i = "UmbCheckout.Workspace.StripeSettings", P = [
  {
    type: "repository",
    alias: A,
    name: "UmbCheckout Stripe Settings Repository",
    api: () => import("./stripe-settings.repository-BxKA6mOO.js")
  },
  {
    type: "store",
    alias: y,
    name: "UmbCheckout Stripe Settings Store",
    api: () => import("./stripe-settings.store-BE9uC4MY.js")
  }
], O = [
  {
    type: "workspace",
    kind: "routable",
    alias: i,
    name: "UmbCheckout Stripe Settings Workspace",
    api: () => import("./stripe-settings-workspace.context-Jz2hbsw4.js"),
    meta: { entityType: m }
  },
  {
    type: "workspaceView",
    alias: "UmbCheckout.WorkspaceView.StripeSettings",
    name: "UmbCheckout Stripe Settings View",
    element: () => import("./stripe-settings-workspace-view.element-BzU3HBf3.js"),
    weight: 100,
    meta: { label: "#umbcheckoutStripe_settings", pathname: "settings", icon: "icon-settings" },
    conditions: [{ alias: "Umb.Condition.WorkspaceAlias", match: i }]
  },
  {
    type: "workspaceAction",
    kind: "default",
    alias: "UmbCheckout.WorkspaceAction.StripeSettings.Save",
    name: "Save UmbCheckout Stripe Settings",
    api: s,
    meta: { look: "primary", color: "positive", label: "#buttons_save" },
    conditions: [{ alias: "Umb.Condition.WorkspaceAlias", match: i }]
  }
], d = [
  {
    type: "menuItem",
    alias: "UmbCheckout.MenuItem.StripeSettings",
    name: "UmbCheckout Stripe Settings Menu Item",
    // v13 order under Stripe: Settings, Shipping Rates (200), Tax Rates.
    weight: 300,
    meta: {
      label: "#umbcheckoutStripe_settings",
      icon: "icon-settings",
      entityType: m,
      menus: [n]
    }
  }
], H = [...P, ...O, ...d], w = [...r, ...l, ...h, ...H, ...g];
export {
  C as U,
  U as a,
  t as b,
  p as c,
  e as d,
  o as e,
  N as f,
  m as g,
  A as h,
  i,
  w as m
};
//# sourceMappingURL=bundle.manifests-ClTqBx9r.js.map
