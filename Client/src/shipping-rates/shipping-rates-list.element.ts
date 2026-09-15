import { css, customElement, html, property, repeat, state, unsafeHTML } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { UmbTextStyles } from "@umbraco-cms/backoffice/style";
import { tryExecute } from "@umbraco-cms/backoffice/resources";
import {
  UmbCheckoutStripeService,
  getStripeShippingRatesDashboardUrl,
  type UmbCheckoutShippingRateModel,
} from "../api/stripe.js";

@customElement("umbcheckout-stripe-shipping-rates-list")
export class UmbCheckoutShippingRatesListElement extends UmbLitElement {
  /** The absolute path of the shipping rates workspace */
  @property({ attribute: false })
  workspacePath?: string;

  @state()
  private _shippingRates?: Array<UmbCheckoutShippingRateModel>;

  @state()
  private _stripeDashboardUrl = getStripeShippingRatesDashboardUrl(false);

  override connectedCallback() {
    super.connectedCallback();
    this.#load();
  }

  async #load() {
    const [shippingRates, settings] = await Promise.all([
      tryExecute(this, UmbCheckoutStripeService.getShippingRates()),
      tryExecute(this, UmbCheckoutStripeService.getSettings()),
    ]);

    this._shippingRates = shippingRates.data ?? [];
    this._stripeDashboardUrl = getStripeShippingRatesDashboardUrl(settings.data?.useLiveApiDetails ?? false);
  }

  override render() {
    return html`
      <umb-body-layout headline=${this.localize.term("umbcheckoutstripe_shipping_rates")}>
        <div class="content">
          <uui-box headline=${this.localize.term("umbcheckout_help_support")}>
            <p>
              <uui-icon name="icon-info"></uui-icon>
              ${unsafeHTML(this.localize.term("umbcheckoutstripe_shipping_help").replace("[[STRIPE_URL]]", this._stripeDashboardUrl))}
            </p>
            <!-- TODO: UmbHost.Licensing - re-enable the unlicensed warning once a Umbraco 17 compatible version of UmbHost.Licensing is available -->
          </uui-box>

          <div>
            <uui-button
              look="outline"
              label=${this.localize.term("umbcheckoutstripe_create_shipping_rate")}
              href=${`${this.workspacePath}/create`}></uui-button>
          </div>

          <uui-box headline=${this.localize.term("umbcheckout_shipping_rates")}>
            ${this._shippingRates ? this.#renderTable(this._shippingRates) : html`<uui-loader></uui-loader>`}
          </uui-box>
        </div>
      </umb-body-layout>
    `;
  }

  #renderTable(shippingRates: Array<UmbCheckoutShippingRateModel>) {
    return html`
      <uui-table>
        <uui-table-head>
          <uui-table-head-cell>${this.localize.term("general_name")}</uui-table-head-cell>
          <uui-table-head-cell>${this.localize.term("general_value")}</uui-table-head-cell>
        </uui-table-head>
        ${repeat(
          shippingRates,
          (shippingRate) => shippingRate.key,
          (shippingRate) => html`
            <uui-table-row>
              <uui-table-cell>
                <a href=${`${this.workspacePath}/edit/${shippingRate.key}`}>${shippingRate.name}</a>
              </uui-table-cell>
              <uui-table-cell>${shippingRate.value}</uui-table-cell>
            </uui-table-row>
          `,
        )}
      </uui-table>
    `;
  }

  static override styles = [
    UmbTextStyles,
    css`
      :host {
        display: block;
        width: 100%;
        height: 100%;
      }

      .content {
        display: flex;
        flex-direction: column;
        gap: var(--uui-size-layout-1);
      }
    `,
  ];
}

declare global {
  interface HTMLElementTagNameMap {
    "umbcheckout-stripe-shipping-rates-list": UmbCheckoutShippingRatesListElement;
  }
}
