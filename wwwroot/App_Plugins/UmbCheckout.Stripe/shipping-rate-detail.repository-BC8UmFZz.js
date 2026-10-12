import { UmbDetailRepositoryBase as p } from "@umbraco-cms/backoffice/repository";
import { tryExecute as a } from "@umbraco-cms/backoffice/resources";
import { S as r } from "./sdk.gen-DEbZmfQN.js";
import { U as n } from "./bundle.manifests-MLL5cmAF.js";
import { UMBCHECKOUT_SHIPPING_RATE_DETAIL_STORE_CONTEXT as s } from "./shipping-rate-detail.store-DxFU_GlC.js";
class o {
  #e;
  constructor(e) {
    this.#e = e;
  }
  async createScaffold(e = {}) {
    return { data: {
      entityType: n,
      unique: crypto.randomUUID(),
      name: "",
      value: "",
      ...e
    } };
  }
  async read(e) {
    const { data: t, error: i } = await a(this.#e, r.getShippingRateById({ path: { id: e } }));
    return i || !t ? { error: i } : { data: { entityType: n, unique: t.id, name: t.name, value: t.value } };
  }
  async create(e) {
    const { error: t } = await a(
      this.#e,
      r.createShippingRate({ body: { id: e.unique, name: e.name, value: e.value } })
    );
    return t ? { error: t } : this.read(e.unique);
  }
  async update(e) {
    const { error: t } = await a(
      this.#e,
      r.updateShippingRate({ path: { id: e.unique }, body: { name: e.name, value: e.value } })
    );
    return t ? { error: t } : this.read(e.unique);
  }
  async delete(e) {
    return a(this.#e, r.deleteShippingRate({ path: { id: e } }));
  }
}
class S extends p {
  constructor(e) {
    super(e, o, s);
  }
}
export {
  S as UmbCheckoutShippingRateDetailRepository,
  S as api
};
//# sourceMappingURL=shipping-rate-detail.repository-BC8UmFZz.js.map
