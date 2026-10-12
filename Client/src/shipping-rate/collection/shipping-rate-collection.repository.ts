import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import type { UmbCollectionFilterModel, UmbCollectionRepository } from "@umbraco-cms/backoffice/collection";
import { UmbRepositoryBase } from "@umbraco-cms/backoffice/repository";
import { UmbCheckoutShippingRateCollectionServerDataSource } from "./shipping-rate-collection.server.data-source.js";

export class UmbCheckoutShippingRateCollectionRepository extends UmbRepositoryBase implements UmbCollectionRepository {
  #dataSource: UmbCheckoutShippingRateCollectionServerDataSource;

  constructor(host: UmbControllerHost) {
    super(host);
    this.#dataSource = new UmbCheckoutShippingRateCollectionServerDataSource(host);
  }

  requestCollection(filter: UmbCollectionFilterModel = {}) {
    return this.#dataSource.getCollection(filter);
  }
}

export { UmbCheckoutShippingRateCollectionRepository as api };
