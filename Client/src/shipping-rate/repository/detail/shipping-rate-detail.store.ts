import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { UmbContextToken } from "@umbraco-cms/backoffice/context-api";
import { UmbDetailStoreBase } from "@umbraco-cms/backoffice/store";
import type { UmbCheckoutShippingRateDetailModel } from "../../types.js";

export class UmbCheckoutShippingRateDetailStore extends UmbDetailStoreBase<UmbCheckoutShippingRateDetailModel> {
  constructor(host: UmbControllerHost) {
    super(host, UMBCHECKOUT_SHIPPING_RATE_DETAIL_STORE_CONTEXT.toString());
  }
}

export const UMBCHECKOUT_SHIPPING_RATE_DETAIL_STORE_CONTEXT = new UmbContextToken<UmbCheckoutShippingRateDetailStore>("UmbCheckoutShippingRateDetailStore");

export { UmbCheckoutShippingRateDetailStore as api };
