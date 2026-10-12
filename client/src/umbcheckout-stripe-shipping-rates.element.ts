import { LitElement, html, css, nothing } from 'lit';
import { customElement, state } from 'lit/decorators.js';
import { getShippingRates, type ShippingRate } from './umbcheckout-stripe-resources.js';

@customElement('umbcheckout-stripe-shipping-rates')
export class UmbCheckoutStripeShippingRates extends LitElement {
  static styles = css`
    :host { display: block; padding: var(--uui-size-layout-1); }
    uui-box { margin-bottom: var(--uui-size-layout-1); }
    uui-table { width: 100%; }
    uui-table-row { cursor: pointer; }
    uui-table-row:hover { background: var(--uui-color-surface-emphasis); }
    .header-actions { display: flex; justify-content: flex-end; padding: var(--uui-size-space-4) 0; }
  `;

  @state() private _rates: ShippingRate[] = [];

  async connectedCallback() {
    super.connectedCallback();
    try { this._rates = await getShippingRates(); } catch (e) { console.error(e); }
  }

  private _navigateToRate(key: string) {
    window.history.pushState({}, '', `/umbraco/section/settings/workspace/umbcheckout-stripe-shipping-rate/${key}`);
    window.dispatchEvent(new PopStateEvent('popstate'));
  }

  private _navigateToCreate() {
    window.history.pushState({}, '', '/umbraco/section/settings/workspace/umbcheckout-stripe-shipping-rate/create');
    window.dispatchEvent(new PopStateEvent('popstate'));
  }

  render() {
    return html`
      <umb-body-layout header-transparent>
        <div slot="header"><h3>Stripe Shipping Rates</h3></div>
        <uui-box headline="Help &amp; Support">
          <p>You will need to create your Shipping Rates within your Stripe account before you can add them below.</p>
        </uui-box>
        <div class="header-actions">
          <uui-button look="outline" label="Create Shipping Rate" @click=${this._navigateToCreate}>Create Shipping Rate</uui-button>
        </div>
        ${this._rates.length > 0
          ? html`
            <uui-table>
              <uui-table-head>
                <uui-table-head-cell>Name</uui-table-head-cell>
                <uui-table-head-cell>Stripe ID</uui-table-head-cell>
              </uui-table-head>
              ${this._rates.map(rate => html`
                <uui-table-row @click=${() => this._navigateToRate(rate.key)}>
                  <uui-table-cell>${rate.name}</uui-table-cell>
                  <uui-table-cell>${rate.value}</uui-table-cell>
                </uui-table-row>
              `)}
            </uui-table>`
          : html`<p>No shipping rates configured.</p>`}
      </umb-body-layout>
    `;
  }
}
declare global { interface HTMLElementTagNameMap { 'umbcheckout-stripe-shipping-rates': UmbCheckoutStripeShippingRates; } }
