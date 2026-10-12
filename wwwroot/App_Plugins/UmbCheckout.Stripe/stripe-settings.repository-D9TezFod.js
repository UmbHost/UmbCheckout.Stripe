import { UmbLocalizationController as s } from "@umbraco-cms/backoffice/localization-api";
import { UMB_NOTIFICATION_CONTEXT as n } from "@umbraco-cms/backoffice/notification";
import { UmbDetailRepositoryBase as l } from "@umbraco-cms/backoffice/repository";
import { tryExecute as r } from "@umbraco-cms/backoffice/resources";
import { a } from "./sdk.gen-CWaSkarI.js";
import { f as c, g as u } from "./bundle.manifests-5bNtKFqe.js";
import { UMBCHECKOUT_STRIPE_SETTINGS_STORE_CONTEXT as m } from "./stripe-settings.store-BE9uC4MY.js";
class p {
  #e;
  constructor(e) {
    this.#e = e;
  }
  async createScaffold() {
    return this.read();
  }
  async read() {
    const { data: e, error: t } = await r(this.#e, a.getStripeSettings());
    return t || !e ? { error: t } : { data: {
      entityType: u,
      unique: c,
      useLiveApiDetails: e.useLiveApiDetails,
      collectPhoneNumber: e.collectPhoneNumber,
      allowPromotionalCodes: e.allowPromotionalCodes,
      collectPromotionalEmailsConsent: e.collectPromotionalEmailsConsent,
      shippingAllowedCountries: e.shippingAllowedCountries ?? "",
      enableAbandonedCartRecovery: e.enableAbandonedCartRecovery,
      allowPromotionalCodesOnRecoveredCarts: e.allowPromotionalCodesOnRecoveredCarts
    } };
  }
  async create(e) {
    return this.update(e);
  }
  async update(e) {
    const { error: t } = await r(
      this.#e,
      a.updateStripeSettings({
        body: {
          useLiveApiDetails: e.useLiveApiDetails,
          collectPhoneNumber: e.collectPhoneNumber,
          allowPromotionalCodes: e.allowPromotionalCodes,
          collectPromotionalEmailsConsent: e.collectPromotionalEmailsConsent,
          shippingAllowedCountries: e.shippingAllowedCountries,
          enableAbandonedCartRecovery: e.enableAbandonedCartRecovery,
          allowPromotionalCodesOnRecoveredCarts: e.allowPromotionalCodesOnRecoveredCarts
        }
      }),
      { disableNotifications: !0 }
    );
    return t ? { error: t } : this.read();
  }
  async delete() {
    return { error: new Error("The Stripe settings cannot be deleted.") };
  }
}
class E extends l {
  #e = new s(this);
  constructor(e) {
    super(e, p, m);
  }
  // The settings are a single row: create and save both store them, and both report the outcome with v13's toasts.
  async create(e, t) {
    const o = await super.create(e, t);
    return await this.#t(!o.error), o;
  }
  async save(e) {
    const t = await super.save(e);
    return await this.#t(!t.error), t;
  }
  async #t(e) {
    const t = e ? "umbcheckoutStripe_settings_saved" : "umbcheckoutStripe_settings_save_failed";
    (await this.getContext(n))?.peek(e ? "positive" : "danger", {
      data: { headline: this.#e.term(`${t}_title`), message: this.#e.term(`${t}_message`) }
    });
  }
}
export {
  E as UmbCheckoutStripeSettingsRepository,
  E as api
};
//# sourceMappingURL=stripe-settings.repository-D9TezFod.js.map
