import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import {
  UmbEntityDetailWorkspaceContextBase,
  UmbWorkspaceIsNewRedirectController,
  UmbWorkspaceIsNewRedirectControllerAlias,
} from "@umbraco-cms/backoffice/workspace";
import type { UmbRouterSlotElement } from "@umbraco-cms/backoffice/router";
import type { UmbCheckoutShippingRateDetailRepository } from "../repository/detail/shipping-rate-detail.repository.js";
import type { UmbCheckoutShippingRateDetailModel } from "../types.js";
import {
  UMBCHECKOUT_SHIPPING_RATE_DETAIL_REPOSITORY_ALIAS,
  UMBCHECKOUT_SHIPPING_RATE_ENTITY_TYPE,
  UMBCHECKOUT_SHIPPING_RATE_ROOT_ENTITY_TYPE,
  UMBCHECKOUT_SHIPPING_RATE_WORKSPACE_ALIAS,
} from "../entity.js";
import { UmbCheckoutShippingRateWorkspaceEditorElement } from "./shipping-rate-workspace-editor.element.js";

export class UmbCheckoutShippingRateWorkspaceContext extends UmbEntityDetailWorkspaceContextBase<
  UmbCheckoutShippingRateDetailModel,
  UmbCheckoutShippingRateDetailRepository
> {
  readonly name = this._data.createObservablePartOfCurrent((data) => data?.name);
  readonly value = this._data.createObservablePartOfCurrent((data) => data?.value);

  constructor(host: UmbControllerHost) {
    super(host, {
      workspaceAlias: UMBCHECKOUT_SHIPPING_RATE_WORKSPACE_ALIAS,
      entityType: UMBCHECKOUT_SHIPPING_RATE_ENTITY_TYPE,
      detailRepositoryAlias: UMBCHECKOUT_SHIPPING_RATE_DETAIL_REPOSITORY_ALIAS,
    });

    this.routes.setRoutes([
      {
        path: "create",
        component: UmbCheckoutShippingRateWorkspaceEditorElement,
        setup: async () => {
          await this.createScaffold({ parent: { entityType: UMBCHECKOUT_SHIPPING_RATE_ROOT_ENTITY_TYPE, unique: null } });
          new UmbWorkspaceIsNewRedirectController(
            this,
            this,
            this.getHostElement().shadowRoot!.querySelector("umb-router-slot") as UmbRouterSlotElement,
          );
        },
      },
      {
        path: "edit/:unique",
        component: UmbCheckoutShippingRateWorkspaceEditorElement,
        setup: (_component, info) => {
          this.removeUmbControllerByAlias(UmbWorkspaceIsNewRedirectControllerAlias);
          this.load(info.match.params.unique);
        },
      },
    ]);
  }

  setName(name: string) {
    this._data.updateCurrent({ name });
  }

  setValue(value: string) {
    this._data.updateCurrent({ value });
  }
}

export { UmbCheckoutShippingRateWorkspaceContext as api };
