import { css, customElement, html, property } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";

const KEY = "umbcheckoutStripe_";
const TEST_DASHBOARD = "https://dashboard.stripe.com/test/shipping-rates/";

/**
 * v13's Help & Support box on the shipping-rate pages: where to create shipping rates in Stripe, plus the
 * unlicensed warning. `dashboardUrl` is any rate's dashboard link (it carries the account's mode); test mode otherwise.
 */
@customElement("umbcheckout-shipping-rates-help")
export class UmbCheckoutShippingRatesHelpElement extends UmbLitElement {
  @property({ attribute: false }) dashboardUrl?: string;

  override render() {
    const dashboard = (this.dashboardUrl ?? TEST_DASHBOARD).replace(/[^/]+$/, "");
    return html`<uui-box headline=${this.localize.term("umbcheckout_help_support")}>
      <p class="muted">
        <umb-icon name="icon-info"></umb-icon>
        ${this.localize.term(KEY + "shipping_help_before")}
        <a href=${dashboard} target="_blank" rel="noopener">${this.localize.term(KEY + "shipping_help_link")}</a>
        ${this.localize.term(KEY + "shipping_help_after")}
      </p>
      <umbcheckout-unlicensed-alert></umbcheckout-unlicensed-alert>
    </uui-box>`;
  }

  static override styles = [
    css`
      .muted {
        color: var(--uui-color-text-alt);
        margin-top: 0;
      }
      .muted umb-icon {
        display: inline-block;
        vertical-align: text-bottom;
        margin-right: var(--uui-size-space-1);
      }
    `,
  ];
}

declare global {
  interface HTMLElementTagNameMap {
    "umbcheckout-shipping-rates-help": UmbCheckoutShippingRatesHelpElement;
  }
}
