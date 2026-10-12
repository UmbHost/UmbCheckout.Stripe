import { UmbLocalizationController as o } from "@umbraco-cms/backoffice/localization-api";
import { UMB_NOTIFICATION_CONTEXT as u } from "@umbraco-cms/backoffice/notification";
import { UmbDetailRepositoryBase as p } from "@umbraco-cms/backoffice/repository";
import { tryExecute as i } from "@umbraco-cms/backoffice/resources";
import { S as r } from "./sdk.gen-DEbZmfQN.js";
import { U as n } from "./bundle.manifests-Djgyi0RL.js";
import { UMBCHECKOUT_SHIPPING_RATE_DETAIL_STORE_CONTEXT as c } from "./shipping-rate-detail.store-DxFU_GlC.js";
class h {
  #t;
  constructor(t) {
    this.#t = t;
  }
  async createScaffold(t = {}) {
    return { data: {
      entityType: n,
      unique: crypto.randomUUID(),
      name: "",
      value: "",
      ...t
    } };
  }
  async read(t) {
    const { data: e, error: a } = await i(this.#t, r.getShippingRateById({ path: { id: t } }));
    return a || !e ? { error: a } : { data: { entityType: n, unique: e.id, name: e.name, value: e.value } };
  }
  async create(t) {
    const { error: e } = await i(
      this.#t,
      r.createShippingRate({ body: { id: t.unique, name: t.name, value: t.value } })
    );
    return e ? { error: e } : this.read(t.unique);
  }
  async update(t) {
    const { error: e } = await i(
      this.#t,
      r.updateShippingRate({ path: { id: t.unique }, body: { name: t.name, value: t.value } })
    );
    return e ? { error: e } : this.read(t.unique);
  }
  async delete(t) {
    return i(this.#t, r.deleteShippingRate({ path: { id: t } }));
  }
}
class T extends p {
  #t = new o(this);
  constructor(t) {
    super(t, h, c);
  }
  // Confirm saves like v13 did. Deletes use Umbraco's delete entity action, which already shows "Deleted".
  async create(t, e) {
    const a = await super.create(t, e);
    return a.error || await this.#e("created"), a;
  }
  async save(t) {
    const e = await super.save(t);
    return e.error || await this.#e("updated"), e;
  }
  async #e(t) {
    (await this.getContext(u))?.peek("positive", {
      data: {
        headline: this.#t.term(`umbcheckoutStripe_shipping_rate_notification_${t}_title`),
        message: this.#t.term(`umbcheckoutStripe_shipping_rate_notification_${t}_message`)
      }
    });
  }
}
export {
  T as UmbCheckoutShippingRateDetailRepository,
  T as api
};
//# sourceMappingURL=shipping-rate-detail.repository-CDCDo68-.js.map
