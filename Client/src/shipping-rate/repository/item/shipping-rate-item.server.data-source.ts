import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import type { UmbItemDataSource } from "@umbraco-cms/backoffice/repository";
import { tryExecute } from "@umbraco-cms/backoffice/resources";
import { ShippingRatesService } from "../../../api/sdk.gen.js";
import { UMBCHECKOUT_SHIPPING_RATE_ENTITY_TYPE } from "../../entity.js";
import type { UmbCheckoutShippingRateItemModel } from "../../types.js";

export class UmbCheckoutShippingRateItemServerDataSource implements UmbItemDataSource<UmbCheckoutShippingRateItemModel> {
  #host: UmbControllerHost;

  constructor(host: UmbControllerHost) {
    this.#host = host;
  }

  async getItems(uniques: Array<string>) {
    const { data, error } = await tryExecute(this.#host, ShippingRatesService.getShippingRates({ query: { skip: 0, take: 1000 } }));
    if (error || !data) return { error };
    const items = data.items
      .filter((rate) => uniques.includes(rate.id))
      .map((rate) => ({ unique: rate.id, entityType: UMBCHECKOUT_SHIPPING_RATE_ENTITY_TYPE, name: rate.name }));
    return { data: items };
  }
}
