import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { UmbLocalizationController } from "@umbraco-cms/backoffice/localization-api";
import { UMB_NOTIFICATION_CONTEXT } from "@umbraco-cms/backoffice/notification";
import { UmbDetailRepositoryBase } from "@umbraco-cms/backoffice/repository";
import { UmbApiError } from "@umbraco-cms/backoffice/resources";
import type { UmbCheckoutStripeSettingsModel } from "../types.js";
import { UmbCheckoutStripeSettingsServerDataSource } from "./stripe-settings.server.data-source.js";
import { UMBCHECKOUT_STRIPE_SETTINGS_STORE_CONTEXT } from "./stripe-settings.store.js";

/** The ProblemDetails type the server gives a save a notification handler cancelled (UmbCheckoutProblemDetails.CancelledType). */
const CANCELLED_TYPE = "urn:umbcheckout:cancelled";

export class UmbCheckoutStripeSettingsRepository extends UmbDetailRepositoryBase<UmbCheckoutStripeSettingsModel> {
  #localize = new UmbLocalizationController(this);

  constructor(host: UmbControllerHost) {
    super(host, UmbCheckoutStripeSettingsServerDataSource, UMBCHECKOUT_STRIPE_SETTINGS_STORE_CONTEXT);
  }

  // The settings are a single row: create and save both store them, and both report the outcome with v13's toasts.
  override async create(model: UmbCheckoutStripeSettingsModel, parentUnique: string | null) {
    const result = await super.create(model, parentUnique);
    await this.#notify(result.error);
    return result;
  }

  override async save(model: UmbCheckoutStripeSettingsModel) {
    const result = await super.save(model);
    await this.#notify(result.error);
    return result;
  }

  async #notify(error: unknown) {
    const key = error ? "umbcheckoutStripe_settings_save_failed" : "umbcheckoutStripe_settings_saved";
    // A site's notification handler refused the save: its reason replaces v13's generic "not saved" message.
    const cancelled = UmbApiError.isUmbApiError(error) && error.problemDetails?.type === CANCELLED_TYPE ? error.problemDetails.title : undefined;
    const notifications = await this.getContext(UMB_NOTIFICATION_CONTEXT);
    notifications?.peek(error ? "danger" : "positive", {
      data: { headline: this.#localize.term(`${key}_title`), message: cancelled ?? this.#localize.term(`${key}_message`) },
    });
  }
}

export { UmbCheckoutStripeSettingsRepository as api };
