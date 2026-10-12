import {
  UMBCHECKOUT_SHIPPING_RATE_COLLECTION_ALIAS,
  UMBCHECKOUT_SHIPPING_RATE_COLLECTION_REPOSITORY_ALIAS,
} from "../entity.js";

export const manifests: Array<UmbExtensionManifest> = [
  {
    type: "repository",
    alias: UMBCHECKOUT_SHIPPING_RATE_COLLECTION_REPOSITORY_ALIAS,
    name: "UmbCheckout Shipping Rate Collection Repository",
    api: () => import("./shipping-rate-collection.repository.js"),
  },
  {
    type: "collection",
    kind: "default",
    alias: UMBCHECKOUT_SHIPPING_RATE_COLLECTION_ALIAS,
    name: "UmbCheckout Shipping Rate Collection",
    meta: { repositoryAlias: UMBCHECKOUT_SHIPPING_RATE_COLLECTION_REPOSITORY_ALIAS, noItemsLabel: "#umbcheckoutStripe_no_shipping_rates" },
  },
  {
    type: "collectionView",
    alias: "UmbCheckout.CollectionView.ShippingRate.Table",
    name: "UmbCheckout Shipping Rate Table View",
    element: () => import("./views/shipping-rate-table-collection-view.element.js"),
    meta: { label: "Table", icon: "icon-list", pathName: "table" },
    conditions: [{ alias: "Umb.Condition.CollectionAlias", match: UMBCHECKOUT_SHIPPING_RATE_COLLECTION_ALIAS }],
  },
  {
    type: "collectionAction",
    kind: "button",
    alias: "UmbCheckout.CollectionAction.ShippingRate.Create",
    name: "Create Shipping Rate",
    meta: { label: "#umbcheckoutStripe_create_shipping_rate", href: "section/settings/workspace/umbcheckout-shipping-rate/create" },
    conditions: [{ alias: "Umb.Condition.CollectionAlias", match: UMBCHECKOUT_SHIPPING_RATE_COLLECTION_ALIAS }],
  },
];

