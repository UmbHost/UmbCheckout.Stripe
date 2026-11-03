import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { UmbContextToken } from "@umbraco-cms/backoffice/context-api";
import { UmbItemStoreBase } from "@umbraco-cms/backoffice/store";
import type { UmbCheckoutShippingRateItemModel } from "../../types.js";

export class UmbCheckoutShippingRateItemStore extends UmbItemStoreBase<UmbCheckoutShippingRateItemModel> {
  constructor(host: UmbControllerHost) {
    super(host, UMBCHECKOUT_SHIPPING_RATE_ITEM_STORE_CONTEXT.toString());
  }
}

export const UMBCHECKOUT_SHIPPING_RATE_ITEM_STORE_CONTEXT = new UmbContextToken<UmbCheckoutShippingRateItemStore>("UmbCheckoutShippingRateItemStore");

export { UmbCheckoutShippingRateItemStore as api };
