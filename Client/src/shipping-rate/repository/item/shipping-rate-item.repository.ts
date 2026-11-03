import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { UmbItemRepositoryBase } from "@umbraco-cms/backoffice/repository";
import type { UmbCheckoutShippingRateItemModel } from "../../types.js";
import { UmbCheckoutShippingRateItemServerDataSource } from "./shipping-rate-item.server.data-source.js";
import { UMBCHECKOUT_SHIPPING_RATE_ITEM_STORE_CONTEXT } from "./shipping-rate-item.store.js";

export class UmbCheckoutShippingRateItemRepository extends UmbItemRepositoryBase<UmbCheckoutShippingRateItemModel> {
  constructor(host: UmbControllerHost) {
    super(host, UmbCheckoutShippingRateItemServerDataSource, UMBCHECKOUT_SHIPPING_RATE_ITEM_STORE_CONTEXT);
  }
}

export { UmbCheckoutShippingRateItemRepository as api };
