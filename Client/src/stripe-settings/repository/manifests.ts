import { UMBCHECKOUT_STRIPE_SETTINGS_REPOSITORY_ALIAS, UMBCHECKOUT_STRIPE_SETTINGS_STORE_ALIAS } from "../entity.js";

export const manifests: Array<UmbExtensionManifest> = [
  {
    type: "repository",
    alias: UMBCHECKOUT_STRIPE_SETTINGS_REPOSITORY_ALIAS,
    name: "UmbCheckout Stripe Settings Repository",
    api: () => import("./stripe-settings.repository.js"),
  },
  {
    type: "store",
    alias: UMBCHECKOUT_STRIPE_SETTINGS_STORE_ALIAS,
    name: "UmbCheckout Stripe Settings Store",
    api: () => import("./stripe-settings.store.js"),
  },
];
