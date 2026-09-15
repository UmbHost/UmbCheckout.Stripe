import { UMB_CHECKOUT_MENU_ALIAS, UMB_CHECKOUT_STRIPE_SHIPPING_RATES_ENTITY_TYPE } from "../constants.js";

export const manifests: Array<UmbExtensionManifest> = [
  {
    type: "menuItem",
    alias: "UmbCheckout.Stripe.MenuItem.ShippingRates",
    name: "UmbCheckout Stripe Shipping Rates Menu Item",
    weight: 800,
    meta: {
      label: "#umbcheckoutstripe_shipping_rates",
      icon: "icon-truck",
      entityType: UMB_CHECKOUT_STRIPE_SHIPPING_RATES_ENTITY_TYPE,
      menus: [UMB_CHECKOUT_MENU_ALIAS],
    },
  },
  {
    type: "workspace",
    alias: "UmbCheckout.Stripe.Workspace.ShippingRates",
    name: "UmbCheckout Stripe Shipping Rates Workspace",
    element: () => import("./shipping-rates-workspace.element.js"),
    meta: {
      entityType: UMB_CHECKOUT_STRIPE_SHIPPING_RATES_ENTITY_TYPE,
    },
  },
];
