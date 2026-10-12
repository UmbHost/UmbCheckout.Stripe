import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { UmbDetailRepositoryBase } from "@umbraco-cms/backoffice/repository";
import type { UmbCheckoutShippingRateDetailModel } from "../../types.js";
import { UmbCheckoutShippingRateDetailServerDataSource } from "./shipping-rate-detail.server.data-source.js";
import { UMBCHECKOUT_SHIPPING_RATE_DETAIL_STORE_CONTEXT } from "./shipping-rate-detail.store.js";

export class UmbCheckoutShippingRateDetailRepository extends UmbDetailRepositoryBase<UmbCheckoutShippingRateDetailModel> {
  constructor(host: UmbControllerHost) {
    super(host, UmbCheckoutShippingRateDetailServerDataSource, UMBCHECKOUT_SHIPPING_RATE_DETAIL_STORE_CONTEXT);
  }
}

export { UmbCheckoutShippingRateDetailRepository as api };
