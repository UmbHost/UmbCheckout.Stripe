import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import type { UmbCollectionFilterModel } from "@umbraco-cms/backoffice/collection";
import { tryExecute } from "@umbraco-cms/backoffice/resources";
import { ShippingRatesService } from "../../api/sdk.gen.js";
import { UMBCHECKOUT_SHIPPING_RATE_ENTITY_TYPE } from "../entity.js";
import type { UmbCheckoutShippingRateDetailModel } from "../types.js";

export class UmbCheckoutShippingRateCollectionServerDataSource {
  #host: UmbControllerHost;

  constructor(host: UmbControllerHost) {
    this.#host = host;
  }

  async getCollection(filter: UmbCollectionFilterModel) {
    const { data, error } = await tryExecute(
      this.#host,
      ShippingRatesService.getShippingRates({ query: { skip: filter.skip ?? 0, take: filter.take ?? 50 } }),
    );
    if (error || !data) return { error };
    const items: Array<UmbCheckoutShippingRateDetailModel> = data.items.map((rate) => ({
      entityType: UMBCHECKOUT_SHIPPING_RATE_ENTITY_TYPE,
      unique: rate.id,
      name: rate.name,
      value: rate.value,
    }));
    return { data: { items, total: data.total } };
  }
}
