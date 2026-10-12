import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { UmbLocalizationController } from "@umbraco-cms/backoffice/localization-api";
import { UMB_NOTIFICATION_CONTEXT } from "@umbraco-cms/backoffice/notification";
import { UmbDetailRepositoryBase } from "@umbraco-cms/backoffice/repository";
import type { UmbCheckoutShippingRateDetailModel } from "../../types.js";
import { UmbCheckoutShippingRateDetailServerDataSource } from "./shipping-rate-detail.server.data-source.js";
import { UMBCHECKOUT_SHIPPING_RATE_DETAIL_STORE_CONTEXT } from "./shipping-rate-detail.store.js";

export class UmbCheckoutShippingRateDetailRepository extends UmbDetailRepositoryBase<UmbCheckoutShippingRateDetailModel> {
  #localize = new UmbLocalizationController(this);

  constructor(host: UmbControllerHost) {
    super(host, UmbCheckoutShippingRateDetailServerDataSource, UMBCHECKOUT_SHIPPING_RATE_DETAIL_STORE_CONTEXT);
  }

  // Confirm saves like v13 did. Deletes use Umbraco's delete entity action, which already shows "Deleted".
  override async create(model: UmbCheckoutShippingRateDetailModel, parentUnique: string | null) {
    const result = await super.create(model, parentUnique);
    if (!result.error) await this.#notify("created");
    return result;
  }

  override async save(model: UmbCheckoutShippingRateDetailModel) {
    const result = await super.save(model);
    if (!result.error) await this.#notify("updated");
    return result;
  }

  async #notify(action: "created" | "updated") {
    const notifications = await this.getContext(UMB_NOTIFICATION_CONTEXT);
    notifications?.peek("positive", {
      data: {
        headline: this.#localize.term(`umbcheckoutStripe_shipping_rate_notification_${action}_title`),
        message: this.#localize.term(`umbcheckoutStripe_shipping_rate_notification_${action}_message`),
      },
    });
  }
}

export { UmbCheckoutShippingRateDetailRepository as api };
