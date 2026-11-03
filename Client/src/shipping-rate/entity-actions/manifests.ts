import {
  UMBCHECKOUT_SHIPPING_RATE_DETAIL_REPOSITORY_ALIAS,
  UMBCHECKOUT_SHIPPING_RATE_ENTITY_TYPE,
  UMBCHECKOUT_SHIPPING_RATE_ITEM_REPOSITORY_ALIAS,
} from "../entity.js";

export const manifests: Array<UmbExtensionManifest> = [
  {
    type: "entityAction",
    kind: "delete",
    alias: "UmbCheckout.EntityAction.ShippingRate.Delete",
    name: "Delete Shipping Rate",
    // Umbraco's delete with v13's "Shipping Rate deleted" toast.
    api: () => import("./shipping-rate-delete.action.js"),
    forEntityTypes: [UMBCHECKOUT_SHIPPING_RATE_ENTITY_TYPE],
    meta: {
      detailRepositoryAlias: UMBCHECKOUT_SHIPPING_RATE_DETAIL_REPOSITORY_ALIAS,
      itemRepositoryAlias: UMBCHECKOUT_SHIPPING_RATE_ITEM_REPOSITORY_ALIAS,
    },
  },
];
