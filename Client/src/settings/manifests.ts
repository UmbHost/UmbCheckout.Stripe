import { UMB_CHECKOUT_MENU_ALIAS, UMB_CHECKOUT_STRIPE_SETTINGS_ENTITY_TYPE } from "../constants.js";

export const manifests: Array<UmbExtensionManifest> = [
  {
    type: "menuItem",
    alias: "UmbCheckout.Stripe.MenuItem.Settings",
    name: "UmbCheckout Stripe Settings Menu Item",
    weight: 900,
    meta: {
      label: "#umbcheckoutstripe_stripe_settings",
      icon: "icon-credit-card",
      entityType: UMB_CHECKOUT_STRIPE_SETTINGS_ENTITY_TYPE,
      menus: [UMB_CHECKOUT_MENU_ALIAS],
    },
  },
  {
    type: "workspace",
    alias: "UmbCheckout.Stripe.Workspace.Settings",
    name: "UmbCheckout Stripe Settings Workspace",
    element: () => import("./stripe-settings-workspace.element.js"),
    meta: {
      entityType: UMB_CHECKOUT_STRIPE_SETTINGS_ENTITY_TYPE,
    },
  },
];
