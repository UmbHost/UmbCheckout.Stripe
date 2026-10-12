import { customElement, html } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { UMBCHECKOUT_SHIPPING_RATE_COLLECTION_ALIAS } from "../entity.js";

/**
 * The Shipping Rates list: the "" route of the Shipping Rates workspace. Uses a body layout rather than the workspace
 * editor, which would also render the shipping-rate workspace views.
 */
@customElement("umbcheckout-shipping-rate-collection-editor")
export class UmbCheckoutShippingRateCollectionEditorElement extends UmbLitElement {
  override render() {
    return html`<umb-body-layout main-no-padding headline=${this.localize.term("umbcheckoutStripe_shipping_rates")}>
      <umb-collection alias=${UMBCHECKOUT_SHIPPING_RATE_COLLECTION_ALIAS}></umb-collection>
    </umb-body-layout>`;
  }
}

export default UmbCheckoutShippingRateCollectionEditorElement;

declare global {
  interface HTMLElementTagNameMap {
    "umbcheckout-shipping-rate-collection-editor": UmbCheckoutShippingRateCollectionEditorElement;
  }
}
