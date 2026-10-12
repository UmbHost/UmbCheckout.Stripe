import { customElement, html } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";

@customElement("umbcheckout-shipping-rate-workspace-editor")
export class UmbCheckoutShippingRateWorkspaceEditorElement extends UmbLitElement {
  override render() {
    return html`<umb-entity-detail-workspace-editor back-path="section/settings/workspace/umbcheckout-shipping-rate-root"></umb-entity-detail-workspace-editor>`;
  }
}

export default UmbCheckoutShippingRateWorkspaceEditorElement;

declare global {
  interface HTMLElementTagNameMap { "umbcheckout-shipping-rate-workspace-editor": UmbCheckoutShippingRateWorkspaceEditorElement; }
}
