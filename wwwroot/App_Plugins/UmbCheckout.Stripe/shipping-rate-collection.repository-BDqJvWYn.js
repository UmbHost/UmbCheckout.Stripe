import { UmbRepositoryBase as a } from "@umbraco-cms/backoffice/repository";
import { tryExecute as s } from "@umbraco-cms/backoffice/resources";
import { S as n } from "./sdk.gen-DEbZmfQN.js";
import { U as p } from "./bundle.manifests-Cs23f-QK.js";
class c {
  #t;
  constructor(t) {
    this.#t = t;
  }
  async getCollection(t) {
    const { data: e, error: i } = await s(
      this.#t,
      n.getShippingRates({ query: { skip: t.skip ?? 0, take: t.take ?? 50 } })
    );
    return i || !e ? { error: i } : { data: { items: e.items.map((o) => ({
      entityType: p,
      unique: o.id,
      name: o.name,
      value: o.value
    })), total: e.total } };
  }
}
class C extends a {
  #t;
  constructor(t) {
    super(t), this.#t = new c(t);
  }
  requestCollection(t = {}) {
    return this.#t.getCollection(t);
  }
}
export {
  C as UmbCheckoutShippingRateCollectionRepository,
  C as api
};
//# sourceMappingURL=shipping-rate-collection.repository-BDqJvWYn.js.map
