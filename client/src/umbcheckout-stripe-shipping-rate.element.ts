import { LitElement, html, css, nothing } from 'lit';
import { customElement, state, property } from 'lit/decorators.js';
import {
  getShippingRate, getStripeShippingRates, getStripeShippingRate,
  createShippingRate, updateShippingRate, deleteShippingRate,
  type ConfigProperty, type StripeShippingRateDetail,
} from './umbcheckout-stripe-resources.js';

@customElement('umbcheckout-stripe-shipping-rate')
export class UmbCheckoutStripeShippingRate extends LitElement {
  static styles = css`
    :host { display: block; padding: var(--uui-size-layout-1); }
    uui-box { margin-bottom: var(--uui-size-layout-1); }
    .property-row { padding: var(--uui-size-space-4) 0; border-bottom: 1px solid var(--uui-color-border); }
    .property-label { font-weight: bold; margin-bottom: var(--uui-size-space-2); }
    .property-description { color: var(--uui-color-text-alt); font-size: 0.85em; margin-bottom: var(--uui-size-space-2); }
    .detail-row { display: flex; justify-content: space-between; padding: var(--uui-size-space-2) 0; }
    .detail-label { font-weight: bold; }
    uui-table { width: 100%; }
    uui-table-row { cursor: pointer; }
    uui-table-row:hover { background: var(--uui-color-surface-emphasis); }
    .footer { display: flex; justify-content: space-between; padding: var(--uui-size-space-4) 0; }
  `;

  @property() rateKey = '';

  @state() private _properties: ConfigProperty[] = [];
  @state() private _stripeRates: StripeShippingRateDetail[] = [];
  @state() private _selectedStripeRate: StripeShippingRateDetail | null = null;
  @state() private _isEdit = false;
  @state() private _saveState: 'init' | 'busy' | 'success' | 'error' = 'init';

  async connectedCallback() {
    super.connectedCallback();
    try {
      this._stripeRates = await getStripeShippingRates();
      if (this.rateKey && this.rateKey !== 'create') {
        this._isEdit = true;
        const data = await getShippingRate(this.rateKey);
        this._properties = data.properties;
        const valueP = this._properties.find(p => p.alias === 'value');
        if (valueP?.value) {
          this._selectedStripeRate = await getStripeShippingRate(valueP.value);
        }
      } else {
        this._properties = [
          { alias: 'name', label: 'Shipping Rate Name', description: 'The Shipping Rate Name', value: '', view: 'textbox', validation: { mandatory: true } },
          { alias: 'value', label: 'Shipping Rate ID', description: 'The Shipping Rate ID set in Stripe', value: '', view: 'textbox', validation: { mandatory: true } },
        ];
      }
    } catch (e) { console.error(e); }
  }

  private _onChange(alias: string, value: string) {
    this._properties = this._properties.map(p => p.alias === alias ? { ...p, value } : p);
  }

  private async _selectStripeRate(rate: StripeShippingRateDetail) {
    this._selectedStripeRate = rate;
    this._onChange('value', rate.id);
    this._onChange('name', rate.displayName);
  }

  private async _save() {
    this._saveState = 'busy';
    try {
      const values: Record<string, any> = {};
      for (const p of this._properties) values[p.alias] = p.value;
      if (this._isEdit) {
        await updateShippingRate(values, this.rateKey);
      } else {
        await createShippingRate(values);
      }
      this._saveState = 'success';
      setTimeout(() => (this._saveState = 'init'), 2000);
    } catch {
      this._saveState = 'error';
      setTimeout(() => (this._saveState = 'init'), 2000);
    }
  }

  private async _delete() {
    if (!confirm('Are you sure you want to delete this shipping rate?')) return;
    try {
      await deleteShippingRate(this.rateKey);
      window.history.back();
    } catch (e) { console.error(e); }
  }

  render() {
    return html`
      <umb-body-layout header-transparent>
        <div slot="header"><h3>${this._isEdit ? 'Edit' : 'Create'} Shipping Rate</h3></div>

        <uui-box headline="Configuration">
          ${this._properties.map(p => html`
            <div class="property-row">
              <div class="property-label">${p.label}</div>
              <div class="property-description">${p.description}</div>
              <uui-input .value=${p.value ?? ''} @change=${(e: Event) => this._onChange(p.alias, (e.target as HTMLInputElement).value)}></uui-input>
            </div>
          `)}
        </uui-box>

        ${this._selectedStripeRate ? html`
          <uui-box headline="Stripe Shipping Rate Details">
            <div class="detail-row"><span class="detail-label">Name</span><span>${this._selectedStripeRate.displayName}</span></div>
            <div class="detail-row"><span class="detail-label">Amount</span><span>${this._selectedStripeRate.fixedAmount?.amount} ${this._selectedStripeRate.fixedAmount?.currency}</span></div>
            ${this._selectedStripeRate.deliveryEstimate?.minimum ? html`<div class="detail-row"><span class="detail-label">Minimum Delivery</span><span>${this._selectedStripeRate.deliveryEstimate.minimum.value} ${this._selectedStripeRate.deliveryEstimate.minimum.unit}</span></div>` : nothing}
            ${this._selectedStripeRate.deliveryEstimate?.maximum ? html`<div class="detail-row"><span class="detail-label">Maximum Delivery</span><span>${this._selectedStripeRate.deliveryEstimate.maximum.value} ${this._selectedStripeRate.deliveryEstimate.maximum.unit}</span></div>` : nothing}
          </uui-box>
        ` : nothing}

        <uui-box headline="Available Stripe Shipping Rates">
          <p>Click on a Shipping Rate to update the form above</p>
          <uui-table>
            <uui-table-head>
              <uui-table-head-cell>Name</uui-table-head-cell>
              <uui-table-head-cell>ID</uui-table-head-cell>
            </uui-table-head>
            ${this._stripeRates.map(rate => html`
              <uui-table-row @click=${() => this._selectStripeRate(rate)}>
                <uui-table-cell>${rate.displayName}</uui-table-cell>
                <uui-table-cell>${rate.id}</uui-table-cell>
              </uui-table-row>
            `)}
          </uui-table>
        </uui-box>

        <div slot="footer">
          <div class="footer">
            ${this._isEdit ? html`<uui-button look="primary" color="danger" label="Delete" @click=${this._delete}>Delete</uui-button>` : html`<div></div>`}
            <uui-button look="primary" color="positive" label="Save" .state=${this._saveState === 'busy' ? 'waiting' : this._saveState === 'success' ? 'success' : this._saveState === 'error' ? 'failed' : undefined} @click=${this._save}>Save</uui-button>
          </div>
        </div>
      </umb-body-layout>
    `;
  }
}
declare global { interface HTMLElementTagNameMap { 'umbcheckout-stripe-shipping-rate': UmbCheckoutStripeShippingRate; } }
