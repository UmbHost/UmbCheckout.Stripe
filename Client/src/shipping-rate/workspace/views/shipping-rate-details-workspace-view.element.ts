import { css, customElement, html, nothing, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import type { UmbPropertyDatasetElement, UmbPropertyValueData } from "@umbraco-cms/backoffice/property";
import { tryExecute } from "@umbraco-cms/backoffice/resources";
import { UMB_WORKSPACE_CONTEXT } from "@umbraco-cms/backoffice/workspace";
import type { UmbTableColumn, UmbTableItem } from "@umbraco-cms/backoffice/components";
import { formatMoney } from "@umbcheckout/core";
import { ShippingRatesService } from "../../../api/sdk.gen.js";
import type { DeliveryEstimateResponseModel, StripeShippingRateResponseModel } from "../../../api/types.gen.js";
import type { UmbCheckoutShippingRateWorkspaceContext } from "../shipping-rate-workspace.context.js";
import "../../shared/shipping-rates-help.element.js";

const KEY = "umbcheckoutStripe_";

/**
 * v13's shipping-rate page (StripeShippingRate.html): Help & Support, the Shipping Rate properties, the matching
 * rate's details as Stripe has them, and the account's Stripe shipping rates (click one to fill in the form).
 */
@customElement("umbcheckout-shipping-rate-details-workspace-view")
export class UmbCheckoutShippingRateDetailsWorkspaceViewElement extends UmbLitElement {
  #context?: UmbCheckoutShippingRateWorkspaceContext;
  @state() private _name = "";
  @state() private _value = "";
  @state() private _stripeRates?: Array<StripeShippingRateResponseModel>;

  constructor() {
    super();
    this.consumeContext(UMB_WORKSPACE_CONTEXT, (context) => {
      this.#context = context as UmbCheckoutShippingRateWorkspaceContext;
      this.observe(this.#context?.name, (name) => (this._name = name ?? ""), "_name");
      this.observe(this.#context?.value, (value) => (this._value = value ?? ""), "_value");
    });
    this.#loadStripeRates();
  }

  async #loadStripeRates() {
    const { data } = await tryExecute(this, ShippingRatesService.getStripeShippingRates());
    this._stripeRates = data ?? [];
  }

  #onChange(event: Event) {
    for (const { alias, value } of (event.target as UmbPropertyDatasetElement).value) {
      if (alias === "name") this.#context?.setName((value as string) ?? "");
      if (alias === "value") this.#context?.setValue((value as string) ?? "");
    }
  }

  #select(rate: StripeShippingRateResponseModel) {
    // v13: clicking a Stripe rate fills in the form above.
    this.#context?.setName(rate.displayName);
    this.#context?.setValue(rate.id);
  }

  #estimate(estimate: DeliveryEstimateResponseModel | null | undefined) {
    if (!estimate) return "";
    const unit = estimate.unit.replace(/_/g, " ");
    return `${estimate.value} ${estimate.value === 1 ? unit : `${unit}s`}`;
  }

  #row(labelKey: string, value: unknown, descriptionKey?: string) {
    return html`<umb-property-layout
      label=${this.localize.term(KEY + labelKey)}
      description=${descriptionKey ? this.localize.term(KEY + descriptionKey) : ""}>
      <div slot="editor">${value ?? ""}</div>
    </umb-property-layout>`;
  }

  #renderHelp() {
    return html`<umbcheckout-shipping-rates-help .dashboardUrl=${this._stripeRates?.[0]?.dashboardUrl}></umbcheckout-shipping-rates-help>`;
  }

  #renderProperties() {
    const dataset: Array<UmbPropertyValueData> = [
      { alias: "name", value: this._name },
      { alias: "value", value: this._value },
    ];
    return html`<uui-box headline=${this.localize.term(KEY + "shipping_rate")}>
      <umb-property-dataset .value=${dataset} @change=${this.#onChange}>
        <umb-property
          alias="name"
          label=${this.localize.term(KEY + "shipping_rate_label")}
          description=${this.localize.term(KEY + "shipping_rate_description")}
          property-editor-ui-alias="Umb.PropertyEditorUi.TextBox"
          .validation=${{ mandatory: true }}></umb-property>
        <umb-property
          alias="value"
          label=${this.localize.term(KEY + "shipping_rate_id_label")}
          description=${this.localize.term(KEY + "shipping_rate_id_description")}
          property-editor-ui-alias="Umb.PropertyEditorUi.TextBox"
          .validation=${{ mandatory: true }}></umb-property>
      </umb-property-dataset>
    </uui-box>`;
  }

  #renderStripeDetails() {
    const rate = this._stripeRates?.find((r) => r.id === this._value);
    if (!rate) return nothing;
    return html`<uui-box headline=${this.localize.term(KEY + "shipping_rate_details")}>
      <p class="muted">${this.localize.term(KEY + "shipping_rate_details_description")}</p>
      ${this.#row("shipping_rate_detail_name", rate.displayName)}
      ${this.#row("shipping_rate_detail_amount", formatMoney(this.localize, rate.amount))}
      ${this.#row("shipping_rate_detail_minimum", this.#estimate(rate.deliveryEstimateMinimum))}
      ${this.#row("shipping_rate_detail_maximum", this.#estimate(rate.deliveryEstimateMaximum))}
      ${this.#row(
        "shipping_rate_detail_edit",
        html`<uui-button look="outline" target="_blank" href=${rate.dashboardUrl}
          label=${this.localize.term(KEY + "shipping_rate_detail_edit_button")}></uui-button>`,
      )}
    </uui-box>`;
  }

  #renderStripeRates() {
    const columns: Array<UmbTableColumn> = [
      { name: this.localize.term(KEY + "shipping_rate_detail_name"), alias: "name" },
      { name: this.localize.term(KEY + "shipping_rate_id_label"), alias: "id" },
      { name: this.localize.term(KEY + "shipping_rate_detail_amount"), alias: "amount" },
    ];
    const items: Array<UmbTableItem> = (this._stripeRates ?? []).map((rate) => ({
      id: rate.id,
      icon: "icon-truck",
      data: [
        {
          columnAlias: "name",
          value: html`<uui-button look="link" compact label=${rate.displayName} @click=${() => this.#select(rate)}></uui-button>`,
        },
        { columnAlias: "id", value: rate.id },
        { columnAlias: "amount", value: formatMoney(this.localize, rate.amount) },
      ],
    }));
    return html`<uui-box headline=${this.localize.term(KEY + "shipping_rates")}>
      <p class="muted">${this.localize.term(KEY + "shipping_rates_description")}</p>
      ${this._stripeRates === undefined
        ? html`<uui-loader></uui-loader>`
        : html`<umb-table .config=${{ allowSelection: false }} .columns=${columns} .items=${items}></umb-table>`}
    </uui-box>`;
  }

  override render() {
    return html`${this.#renderHelp()} ${this.#renderProperties()} ${this.#renderStripeDetails()} ${this.#renderStripeRates()}`;
  }

  static override styles = [
    css`
      :host {
        display: grid;
        gap: var(--uui-size-layout-1);
        padding: var(--uui-size-layout-1);
      }
      .muted {
        color: var(--uui-color-text-alt);
        margin-top: 0;
      }
    `,
  ];
}

export default UmbCheckoutShippingRateDetailsWorkspaceViewElement;

declare global {
  interface HTMLElementTagNameMap {
    "umbcheckout-shipping-rate-details-workspace-view": UmbCheckoutShippingRateDetailsWorkspaceViewElement;
  }
}
