import { UmbItemRepositoryBase as o } from "@umbraco-cms/backoffice/repository";
import { tryExecute as a } from "@umbraco-cms/backoffice/resources";
import { S as m } from "./sdk.gen-DEbZmfQN.js";
import { U as p } from "./bundle.manifests-BmnazK2-.js";
import { UMBCHECKOUT_SHIPPING_RATE_ITEM_STORE_CONTEXT as n } from "./shipping-rate-item.store-BGZlqPk2.js";
class c {
  #t;
  constructor(t) {
    this.#t = t;
  }
  async getItems(t) {
    const { data: i, error: r } = await a(this.#t, m.getShippingRates({ query: { skip: 0, take: 1e3 } }));
    return r || !i ? { error: r } : { data: i.items.filter((e) => t.includes(e.id)).map((e) => ({ unique: e.id, entityType: p, name: e.name })) };
  }
}
class R extends o {
  constructor(t) {
    super(t, c, n);
  }
}
export {
  R as UmbCheckoutShippingRateItemRepository,
  R as api
};
//# sourceMappingURL=shipping-rate-item.repository-tImzaeje.js.map
