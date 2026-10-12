import { customElement, html, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { UMB_WORKSPACE_CONTEXT } from "@umbraco-cms/backoffice/workspace";
import type { UmbCheckoutShippingRateWorkspaceContext } from "../shipping-rate-workspace.context.js";

@customElement("umbcheckout-shipping-rate-details-workspace-view")
export class UmbCheckoutShippingRateDetailsWorkspaceViewElement extends UmbLitElement {
  #context?: UmbCheckoutShippingRateWorkspaceContext;
  @state() private _name = "";
  @state() private _value = "";

  constructor() {
    super();
    this.consumeContext(UMB_WORKSPACE_CONTEXT, (context) => {
      this.#context = context as UmbCheckoutShippingRateWorkspaceContext;
      this.observe(this.#context?.name, (name) => (this._name = name ?? ""), "_name");
      this.observe(this.#context?.value, (value) => (this._value = value ?? ""), "_value");
    });
  }

  override render() {
    return html`<uui-box>
      <umb-property-layout label=${this.localize.term("umbcheckoutStripe_shipping_rate_name")} mandatory>
        <uui-input slot="editor" required .value=${this._name}
          @input=${(e: Event) => this.#context?.setName((e.target as HTMLInputElement).value)}></uui-input>
      </umb-property-layout>
      <umb-property-layout label=${this.localize.term("umbcheckoutStripe_shipping_rate_value")} description="shr_..." mandatory>
        <uui-input slot="editor" required .value=${this._value}
          @input=${(e: Event) => this.#context?.setValue((e.target as HTMLInputElement).value)}></uui-input>
      </umb-property-layout>
    </uui-box>`;
  }
}

export default UmbCheckoutShippingRateDetailsWorkspaceViewElement;

declare global {
  interface HTMLElementTagNameMap { "umbcheckout-shipping-rate-details-workspace-view": UmbCheckoutShippingRateDetailsWorkspaceViewElement; }
}
