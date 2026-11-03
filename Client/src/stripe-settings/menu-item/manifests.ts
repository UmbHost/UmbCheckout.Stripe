import { UMBCHECKOUT_STRIPE_MENU_ALIAS } from "@umbcheckout/core";
import { UMBCHECKOUT_STRIPE_SETTINGS_ENTITY_TYPE } from "../entity.js";

export const manifests: Array<UmbExtensionManifest> = [
  {
    type: "menuItem",
    alias: "UmbCheckout.MenuItem.StripeSettings",
    name: "UmbCheckout Stripe Settings Menu Item",
    // v13 order under Stripe: Settings, Shipping Rates (200), Tax Rates.
    weight: 300,
    meta: {
      label: "#umbcheckoutStripe_settings",
      icon: "icon-settings",
      entityType: UMBCHECKOUT_STRIPE_SETTINGS_ENTITY_TYPE,
      menus: [UMBCHECKOUT_STRIPE_MENU_ALIAS],
    },
  },
];
