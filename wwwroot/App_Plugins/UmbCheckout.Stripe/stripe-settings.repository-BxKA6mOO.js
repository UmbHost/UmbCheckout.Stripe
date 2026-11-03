import { UmbLocalizationController as s } from "@umbraco-cms/backoffice/localization-api";
import { UMB_NOTIFICATION_CONTEXT as n } from "@umbraco-cms/backoffice/notification";
import { UmbDetailRepositoryBase as l } from "@umbraco-cms/backoffice/repository";
import { tryExecute as i, UmbApiError as c } from "@umbraco-cms/backoffice/resources";
import { a as r } from "./sdk.gen-CWaSkarI.js";
import { f as m, g as p } from "./bundle.manifests-ClTqBx9r.js";
import { UMBCHECKOUT_STRIPE_SETTINGS_STORE_CONTEXT as u } from "./stripe-settings.store-BE9uC4MY.js";
class d {
  #e;
  constructor(e) {
    this.#e = e;
  }
  async createScaffold() {
    return this.read();
  }
  async read() {
    const { data: e, error: t } = await i(this.#e, r.getStripeSettings());
    return t || !e ? { error: t } : { data: {
      entityType: p,
      unique: m,
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
    const { error: t } = await i(
      this.#e,
      r.updateStripeSettings({
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
const C = "urn:umbcheckout:cancelled";
class _ extends l {
  #e = new s(this);
  constructor(e) {
    super(e, d, u);
  }
  // The settings are a single row: create and save both store them, and both report the outcome with v13's toasts.
  async create(e, t) {
    const o = await super.create(e, t);
    return await this.#t(o.error), o;
  }
  async save(e) {
    const t = await super.save(e);
    return await this.#t(t.error), t;
  }
  async #t(e) {
    const t = e ? "umbcheckoutStripe_settings_save_failed" : "umbcheckoutStripe_settings_saved", o = c.isUmbApiError(e) && e.problemDetails?.type === C ? e.problemDetails.detail ?? e.problemDetails.title : void 0;
    (await this.getContext(n))?.peek(e ? "danger" : "positive", {
      data: { headline: this.#e.term(`${t}_title`), message: o ?? this.#e.term(`${t}_message`) }
    });
  }
}
export {
  _ as UmbCheckoutStripeSettingsRepository,
  _ as api
};
//# sourceMappingURL=stripe-settings.repository-BxKA6mOO.js.map
