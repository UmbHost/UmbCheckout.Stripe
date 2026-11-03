import { UMBCHECKOUT_SHIPPING_RATE_DETAIL_REPOSITORY_ALIAS, UMBCHECKOUT_SHIPPING_RATE_DETAIL_STORE_ALIAS } from "../../entity.js";

export const manifests: Array<UmbExtensionManifest> = [
  {
    type: "repository",
    alias: UMBCHECKOUT_SHIPPING_RATE_DETAIL_REPOSITORY_ALIAS,
    name: "UmbCheckout Shipping Rate Detail Repository",
    api: () => import("./shipping-rate-detail.repository.js"),
  },
  {
    type: "store",
    alias: UMBCHECKOUT_SHIPPING_RATE_DETAIL_STORE_ALIAS,
    name: "UmbCheckout Shipping Rate Detail Store",
    api: () => import("./shipping-rate-detail.store.js"),
  },
];
