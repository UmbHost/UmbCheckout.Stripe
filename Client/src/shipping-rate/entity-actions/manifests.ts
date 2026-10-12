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
    forEntityTypes: [UMBCHECKOUT_SHIPPING_RATE_ENTITY_TYPE],
    meta: {
      detailRepositoryAlias: UMBCHECKOUT_SHIPPING_RATE_DETAIL_REPOSITORY_ALIAS,
      itemRepositoryAlias: UMBCHECKOUT_SHIPPING_RATE_ITEM_REPOSITORY_ALIAS,
    },
  },
];
