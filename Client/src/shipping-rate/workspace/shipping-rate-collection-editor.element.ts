import { css, customElement, html, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { tryExecute } from "@umbraco-cms/backoffice/resources";
import { ShippingRatesService } from "../../api/sdk.gen.js";
import { UMBCHECKOUT_SHIPPING_RATE_COLLECTION_ALIAS } from "../entity.js";
import "../shared/shipping-rates-help.element.js";

/**
 * The Shipping Rates list (v13's StripeShippingRates.html): Help & Support above the list. The "" route of the
 * Shipping Rates workspace; a body layout rather than the workspace editor, which would render the workspace views.
 */
@customElement("umbcheckout-shipping-rate-collection-editor")
export class UmbCheckoutShippingRateCollectionEditorElement extends UmbLitElement {
  @state() private _dashboardUrl?: string;

  constructor() {
    super();
    this.#loadDashboardUrl();
  }

  async #loadDashboardUrl() {
    const { data } = await tryExecute(this, ShippingRatesService.getStripeShippingRates(), { disableNotifications: true });
    this._dashboardUrl = data?.[0]?.dashboardUrl;
  }

  override render() {
    return html`<umb-body-layout main-no-padding headline=${this.localize.term("umbcheckoutStripe_shipping_rates")}>
      <div class="help"><umbcheckout-shipping-rates-help .dashboardUrl=${this._dashboardUrl}></umbcheckout-shipping-rates-help></div>
      <umb-collection alias=${UMBCHECKOUT_SHIPPING_RATE_COLLECTION_ALIAS}></umb-collection>
      <div class="help"><umbcheckout-made-by></umbcheckout-made-by></div>
    </umb-body-layout>`;
  }

  static override styles = [
    css`
      .help {
        padding: var(--uui-size-layout-1) var(--uui-size-layout-1) 0;
      }
    `,
  ];
}

export default UmbCheckoutShippingRateCollectionEditorElement;

declare global {
  interface HTMLElementTagNameMap {
    "umbcheckout-shipping-rate-collection-editor": UmbCheckoutShippingRateCollectionEditorElement;
  }
}
