import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import type { UmbDetailDataSource } from "@umbraco-cms/backoffice/repository";
import { tryExecute } from "@umbraco-cms/backoffice/resources";
import { StripeSettingsService } from "../../api/sdk.gen.js";
import { UMBCHECKOUT_STRIPE_SETTINGS_ENTITY_TYPE, UMBCHECKOUT_STRIPE_SETTINGS_UNIQUE } from "../entity.js";
import type { UmbCheckoutStripeSettingsModel } from "../types.js";

/** Singleton detail data source: read and update only. */
export class UmbCheckoutStripeSettingsServerDataSource implements UmbDetailDataSource<UmbCheckoutStripeSettingsModel> {
  #host: UmbControllerHost;

  constructor(host: UmbControllerHost) {
    this.#host = host;
  }

  async createScaffold() {
    return this.read();
  }

  async read() {
    const { data, error } = await tryExecute(this.#host, StripeSettingsService.getStripeSettings());
    if (error || !data) return { error };

    const model: UmbCheckoutStripeSettingsModel = {
      entityType: UMBCHECKOUT_STRIPE_SETTINGS_ENTITY_TYPE,
      unique: UMBCHECKOUT_STRIPE_SETTINGS_UNIQUE,
      useLiveApiDetails: data.useLiveApiDetails,
      collectPhoneNumber: data.collectPhoneNumber,
      allowPromotionalCodes: data.allowPromotionalCodes,
      collectPromotionalEmailsConsent: data.collectPromotionalEmailsConsent,
      shippingAllowedCountries: data.shippingAllowedCountries ?? "",
      enableAbandonedCartRecovery: data.enableAbandonedCartRecovery,
      allowPromotionalCodesOnRecoveredCarts: data.allowPromotionalCodesOnRecoveredCarts,
    };
    return { data: model };
  }

  async create(model: UmbCheckoutStripeSettingsModel) {
    return this.update(model);
  }

  async update(model: UmbCheckoutStripeSettingsModel) {
    // The repository shows v13's "failed to save" toast, so the generic one is turned off here.
    const { error } = await tryExecute(
      this.#host,
      StripeSettingsService.updateStripeSettings({
        body: {
          useLiveApiDetails: model.useLiveApiDetails,
          collectPhoneNumber: model.collectPhoneNumber,
          allowPromotionalCodes: model.allowPromotionalCodes,
          collectPromotionalEmailsConsent: model.collectPromotionalEmailsConsent,
          shippingAllowedCountries: model.shippingAllowedCountries,
          enableAbandonedCartRecovery: model.enableAbandonedCartRecovery,
          allowPromotionalCodesOnRecoveredCarts: model.allowPromotionalCodesOnRecoveredCarts,
        },
      }),
      { disableNotifications: true },
    );
    if (error) return { error };
    return this.read();
  }

  async delete() {
    return { error: new Error("The Stripe settings cannot be deleted.") };
  }
}
