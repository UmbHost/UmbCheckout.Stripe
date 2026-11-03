import { customElement, html } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";

/** v13 header "UmbCheckout"; the footer shows the "Made by UmbHost" link when unlicensed. */
@customElement("umbcheckout-stripe-settings-workspace-editor")
export class UmbCheckoutStripeSettingsWorkspaceEditorElement extends UmbLitElement {
  override render() {
    return html`<umb-workspace-editor headline=${this.localize.term("umbcheckout_section")}>
      <umbcheckout-made-by slot="footer-info"></umbcheckout-made-by>
    </umb-workspace-editor>`;
  }
}

export default UmbCheckoutStripeSettingsWorkspaceEditorElement;

declare global {
  interface HTMLElementTagNameMap {
    "umbcheckout-stripe-settings-workspace-editor": UmbCheckoutStripeSettingsWorkspaceEditorElement;
  }
}
