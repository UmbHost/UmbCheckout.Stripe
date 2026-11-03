import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { UmbEntityDetailWorkspaceContextBase } from "@umbraco-cms/backoffice/workspace";
import type { UmbCheckoutStripeSettingsRepository } from "../repository/stripe-settings.repository.js";
import type { UmbCheckoutStripeSettingsModel } from "../types.js";
import {
  UMBCHECKOUT_STRIPE_SETTINGS_ENTITY_TYPE,
  UMBCHECKOUT_STRIPE_SETTINGS_REPOSITORY_ALIAS,
  UMBCHECKOUT_STRIPE_SETTINGS_UNIQUE,
  UMBCHECKOUT_STRIPE_SETTINGS_WORKSPACE_ALIAS,
} from "../entity.js";
import { UmbCheckoutStripeSettingsWorkspaceEditorElement } from "./stripe-settings-workspace-editor.element.js";

export class UmbCheckoutStripeSettingsWorkspaceContext extends UmbEntityDetailWorkspaceContextBase<
  UmbCheckoutStripeSettingsModel,
  UmbCheckoutStripeSettingsRepository
> {
  readonly model = this._data.createObservablePartOfCurrent((data) => data);

  constructor(host: UmbControllerHost) {
    super(host, {
      workspaceAlias: UMBCHECKOUT_STRIPE_SETTINGS_WORKSPACE_ALIAS,
      entityType: UMBCHECKOUT_STRIPE_SETTINGS_ENTITY_TYPE,
      detailRepositoryAlias: UMBCHECKOUT_STRIPE_SETTINGS_REPOSITORY_ALIAS,
    });

    this.routes.setRoutes([
      {
        path: "",
        component: UmbCheckoutStripeSettingsWorkspaceEditorElement,
        setup: () => {
          this.load(UMBCHECKOUT_STRIPE_SETTINGS_UNIQUE);
        },
      },
    ]);
  }

  setValue<K extends keyof UmbCheckoutStripeSettingsModel>(alias: K, value: UmbCheckoutStripeSettingsModel[K]) {
    this._data.updateCurrent({ [alias]: value } as Partial<UmbCheckoutStripeSettingsModel>);
  }
}

export { UmbCheckoutStripeSettingsWorkspaceContext as api };
