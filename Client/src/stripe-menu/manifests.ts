import { UMBCHECKOUT_SETTINGS_MENU_ALIAS } from "@umbcheckout/core";

export const manifests: Array<UmbExtensionManifest> = [
  {
    type: "menuItem",
    alias: "UmbCheckout.MenuItem.Stripe",
    name: "UmbCheckout Stripe Menu Item",
    // Below Configuration (weight 400), as in v13.
    weight: 300,
    element: () => import("./stripe-menu-item.element.js"),
    meta: { label: "#umbcheckoutStripe_stripe", icon: "icon-credit-card", menus: [UMBCHECKOUT_SETTINGS_MENU_ALIAS] },
  },
];
