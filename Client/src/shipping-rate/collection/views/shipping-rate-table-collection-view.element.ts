import { customElement, html, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { UMB_COLLECTION_CONTEXT } from "@umbraco-cms/backoffice/collection";
import type { UmbTableColumn, UmbTableItem } from "@umbraco-cms/backoffice/components";
import type { UmbCheckoutShippingRateDetailModel } from "../../types.js";

@customElement("umbcheckout-shipping-rate-table-collection-view")
export class UmbCheckoutShippingRateTableCollectionViewElement extends UmbLitElement {
  @state() private _items: Array<UmbTableItem> = [];

  private _columns: Array<UmbTableColumn> = [
    { name: this.localize.term("umbcheckoutStripe_shipping_rate_name"), alias: "name" },
    { name: this.localize.term("umbcheckoutStripe_shipping_rate_value"), alias: "value" },
  ];

  constructor() {
    super();
    this.consumeContext(UMB_COLLECTION_CONTEXT, (context) => {
      this.observe(context?.items, (items) => (this._items = (items ?? []).map((i) => this.#toRow(i as UmbCheckoutShippingRateDetailModel))), "_items");
    });
  }

  #toRow(rate: UmbCheckoutShippingRateDetailModel): UmbTableItem {
    const href = `section/settings/workspace/umbcheckout-shipping-rate/edit/${rate.unique}`;
    return {
      id: rate.unique,
      icon: "icon-truck",
      data: [
        { columnAlias: "name", value: html`<a href=${href}>${rate.name}</a>` },
        { columnAlias: "value", value: rate.value },
      ],
    };
  }

  override render() {
    return html`<umb-table .config=${{ allowSelection: false }} .columns=${this._columns} .items=${this._items}></umb-table>`;
  }
}

export default UmbCheckoutShippingRateTableCollectionViewElement;

declare global {
  interface HTMLElementTagNameMap { "umbcheckout-shipping-rate-table-collection-view": UmbCheckoutShippingRateTableCollectionViewElement; }
}
