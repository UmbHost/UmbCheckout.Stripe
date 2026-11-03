import { css, customElement, html, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { UMB_WORKSPACE_CONTEXT } from "@umbraco-cms/backoffice/workspace";
import { UMBCHECKOUT_SHIPPING_RATES_PATH } from "../entity.js";
import type { UmbCheckoutShippingRateWorkspaceContext } from "./shipping-rate-workspace.context.js";

/** v13's header showed the shipping rate's name; a new rate shows "Create Shipping Rate". */
@customElement("umbcheckout-shipping-rate-workspace-editor")
export class UmbCheckoutShippingRateWorkspaceEditorElement extends UmbLitElement {
  @state() private _name = "";
  @state() private _isNew = false;

  constructor() {
    super();
    this.consumeContext(UMB_WORKSPACE_CONTEXT, (context) => {
      const workspace = context as UmbCheckoutShippingRateWorkspaceContext | undefined;
      this.observe(workspace?.name, (name) => (this._name = name ?? ""), "_name");
      this.observe(workspace?.isNew, (isNew) => (this._isNew = isNew === true), "_isNew");
    });
  }

  override render() {
    const title = this._isNew || !this._name ? this.localize.term("umbcheckoutStripe_create_shipping_rate") : this._name;
    return html`<umb-entity-detail-workspace-editor .backPath=${UMBCHECKOUT_SHIPPING_RATES_PATH}>
      <h3 slot="header" class="headline">${title}</h3>
      <umbcheckout-made-by slot="footer-info"></umbcheckout-made-by>
    </umb-entity-detail-workspace-editor>`;
  }

  static override styles = [
    css`
      .headline {
        margin: 0 0 0 var(--uui-size-space-3);
        font-size: var(--uui-type-h5-size);
        font-weight: 700;
        align-self: center;
      }
    `,
  ];
}

export default UmbCheckoutShippingRateWorkspaceEditorElement;

declare global {
  interface HTMLElementTagNameMap {
    "umbcheckout-shipping-rate-workspace-editor": UmbCheckoutShippingRateWorkspaceEditorElement;
  }
}
