import { UMBCHECKOUT_STRIPE_MENU_ALIAS } from "@umbcheckout/core";
import { UMBCHECKOUT_SHIPPING_RATE_ROOT_ENTITY_TYPE } from "../entity.js";

export const manifests: Array<UmbExtensionManifest> = [
  {
    type: "menuItem",
    alias: "UmbCheckout.MenuItem.ShippingRates",
    name: "UmbCheckout Shipping Rates Menu Item",
    weight: 200,
    meta: {
      label: "#umbcheckoutStripe_shipping_rates",
      icon: "icon-truck",
      entityType: UMBCHECKOUT_SHIPPING_RATE_ROOT_ENTITY_TYPE,
      menus: [UMBCHECKOUT_STRIPE_MENU_ALIAS],
    },
  },
];
