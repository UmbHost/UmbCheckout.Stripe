import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { UmbContextToken } from "@umbraco-cms/backoffice/context-api";
import { UmbDetailStoreBase } from "@umbraco-cms/backoffice/store";
import type { UmbCheckoutStripeSettingsModel } from "../types.js";

export class UmbCheckoutStripeSettingsStore extends UmbDetailStoreBase<UmbCheckoutStripeSettingsModel> {
  constructor(host: UmbControllerHost) {
    super(host, UMBCHECKOUT_STRIPE_SETTINGS_STORE_CONTEXT.toString());
  }
}

export const UMBCHECKOUT_STRIPE_SETTINGS_STORE_CONTEXT = new UmbContextToken<UmbCheckoutStripeSettingsStore>("UmbCheckoutStripeSettingsStore");

export { UmbCheckoutStripeSettingsStore as api };
