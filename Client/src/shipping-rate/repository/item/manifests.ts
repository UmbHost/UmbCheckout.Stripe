import { UMBCHECKOUT_SHIPPING_RATE_ITEM_REPOSITORY_ALIAS, UMBCHECKOUT_SHIPPING_RATE_ITEM_STORE_ALIAS } from "../../entity.js";

export const manifests: Array<UmbExtensionManifest> = [
  {
    type: "repository",
    alias: UMBCHECKOUT_SHIPPING_RATE_ITEM_REPOSITORY_ALIAS,
    name: "UmbCheckout Shipping Rate Item Repository",
    api: () => import("./shipping-rate-item.repository.js"),
  },
  {
    type: "itemStore",
    alias: UMBCHECKOUT_SHIPPING_RATE_ITEM_STORE_ALIAS,
    name: "UmbCheckout Shipping Rate Item Store",
    api: () => import("./shipping-rate-item.store.js"),
  },
];
