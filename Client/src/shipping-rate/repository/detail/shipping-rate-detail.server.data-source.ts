import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import type { UmbDetailDataSource } from "@umbraco-cms/backoffice/repository";
import { tryExecute } from "@umbraco-cms/backoffice/resources";
import { ShippingRatesService } from "../../../api/sdk.gen.js";
import { UMBCHECKOUT_SHIPPING_RATE_ENTITY_TYPE } from "../../entity.js";
import type { UmbCheckoutShippingRateDetailModel } from "../../types.js";

export class UmbCheckoutShippingRateDetailServerDataSource implements UmbDetailDataSource<UmbCheckoutShippingRateDetailModel> {
  #host: UmbControllerHost;

  constructor(host: UmbControllerHost) {
    this.#host = host;
  }

  async createScaffold(preset: Partial<UmbCheckoutShippingRateDetailModel> = {}) {
    const data: UmbCheckoutShippingRateDetailModel = {
      entityType: UMBCHECKOUT_SHIPPING_RATE_ENTITY_TYPE,
      unique: crypto.randomUUID(),
      name: "",
      value: "",
      ...preset,
    };
    return { data };
  }

  async read(unique: string) {
    const { data, error } = await tryExecute(this.#host, ShippingRatesService.getShippingRateById({ path: { id: unique } }));
    if (error || !data) return { error };
    return { data: { entityType: UMBCHECKOUT_SHIPPING_RATE_ENTITY_TYPE, unique: data.id, name: data.name, value: data.value } };
  }

  async create(model: UmbCheckoutShippingRateDetailModel) {
    const { error } = await tryExecute(
      this.#host,
      ShippingRatesService.createShippingRate({ body: { id: model.unique, name: model.name, value: model.value } }),
    );
    if (error) return { error };
    return this.read(model.unique);
  }

  async update(model: UmbCheckoutShippingRateDetailModel) {
    const { error } = await tryExecute(
      this.#host,
      ShippingRatesService.updateShippingRate({ path: { id: model.unique }, body: { name: model.name, value: model.value } }),
    );
    if (error) return { error };
    return this.read(model.unique);
  }

  async delete(unique: string) {
    return tryExecute(this.#host, ShippingRatesService.deleteShippingRate({ path: { id: unique } }));
  }
}
