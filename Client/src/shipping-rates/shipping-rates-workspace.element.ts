import { css, customElement, html } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import type { UmbRoute } from "@umbraco-cms/backoffice/router";
import { UmbCheckoutShippingRatesListElement } from "./shipping-rates-list.element.js";
import { UmbCheckoutShippingRateEditElement } from "./shipping-rate-edit.element.js";

/**
 * Workspace for the Stripe shipping rates, routes between the list and the create / edit views:
 * - workspace/umbcheckout-stripe-shipping-rates
 * - workspace/umbcheckout-stripe-shipping-rates/create
 * - workspace/umbcheckout-stripe-shipping-rates/edit/:key
 */
@customElement("umbcheckout-stripe-shipping-rates-workspace")
export class UmbCheckoutShippingRatesWorkspaceElement extends UmbLitElement {
  #routes: Array<UmbRoute> = [
    {
      path: "create",
      component: UmbCheckoutShippingRateEditElement,
      setup: (component, info) => {
        (component as UmbCheckoutShippingRateEditElement).workspacePath = info.slot.constructAbsolutePath("");
      },
    },
    {
      path: "edit/:key",
      component: UmbCheckoutShippingRateEditElement,
      setup: (component, info) => {
        const element = component as UmbCheckoutShippingRateEditElement;
        element.workspacePath = info.slot.constructAbsolutePath("");
        element.key = info.match.params.key;
      },
    },
    {
      path: "",
      component: UmbCheckoutShippingRatesListElement,
      setup: (component, info) => {
        (component as UmbCheckoutShippingRatesListElement).workspacePath = info.slot.constructAbsolutePath("");
      },
    },
  ];

  override render() {
    return html`<umb-router-slot .routes=${this.#routes}></umb-router-slot>`;
  }

  static override styles = [
    css`
      :host {
        display: block;
        width: 100%;
        height: 100%;
      }

      umb-router-slot {
        height: 100%;
      }
    `,
  ];
}

export default UmbCheckoutShippingRatesWorkspaceElement;

declare global {
  interface HTMLElementTagNameMap {
    "umbcheckout-stripe-shipping-rates-workspace": UmbCheckoutShippingRatesWorkspaceElement;
  }
}
