import { LitElement, html, css } from 'lit';
import { customElement, state } from 'lit/decorators.js';
import { getStripeSettings, updateStripeSettings, type ConfigProperty } from './umbcheckout-stripe-resources.js';

@customElement('umbcheckout-stripe-settings')
export class UmbCheckoutStripeSettings extends LitElement {
  static styles = css`
    :host { display: block; padding: var(--uui-size-layout-1); }
    uui-box { margin-bottom: var(--uui-size-layout-1); }
    .property-row { padding: var(--uui-size-space-4) 0; border-bottom: 1px solid var(--uui-color-border); }
    .property-label { font-weight: bold; margin-bottom: var(--uui-size-space-2); }
    .property-description { color: var(--uui-color-text-alt); font-size: 0.85em; margin-bottom: var(--uui-size-space-2); }
    .footer { display: flex; justify-content: flex-end; padding: var(--uui-size-space-4) 0; }
  `;

  @state() private _properties: ConfigProperty[] = [];
  @state() private _saveState: 'init' | 'busy' | 'success' | 'error' = 'init';

  async connectedCallback() {
    super.connectedCallback();
    try { this._properties = await getStripeSettings(); } catch (e) { console.error(e); }
  }

  private async _save() {
    this._saveState = 'busy';
    try {
      const values: Record<string, any> = {};
      for (const p of this._properties) values[p.alias] = p.value;
      await updateStripeSettings(values);
      this._saveState = 'success';
      setTimeout(() => (this._saveState = 'init'), 2000);
    } catch {
      this._saveState = 'error';
      setTimeout(() => (this._saveState = 'init'), 2000);
    }
  }

  private _onChange(alias: string, value: any) {
    this._properties = this._properties.map(p => p.alias === alias ? { ...p, value } : p);
  }

  render() {
    return html`
      <umb-body-layout header-transparent>
        <div slot="header"><h3>Stripe Settings</h3></div>
        <uui-box headline="Help &amp; Support">
          <p>Documentation available at <a href="https://docs.umbcheckout.net/" target="_blank">https://docs.umbcheckout.net/</a></p>
          <p>Please report issues on <a target="_blank" href="https://github.com/UmbHost/UmbCheckout/issues">GitHub</a></p>
        </uui-box>
        <uui-box headline="Configuration">
          ${this._properties.map(p => html`
            <div class="property-row">
              <div class="property-label">${p.label}</div>
              <div class="property-description">${p.description}</div>
              ${p.view === 'boolean'
                ? html`<uui-toggle .checked=${p.value === true || p.value === 'true'} @change=${(e: Event) => this._onChange(p.alias, (e.target as any).checked)}></uui-toggle>`
                : html`<uui-input .value=${p.value ?? ''} @change=${(e: Event) => this._onChange(p.alias, (e.target as HTMLInputElement).value)}></uui-input>`}
            </div>
          `)}
        </uui-box>
        <div slot="footer">
          <div class="footer">
            <uui-button look="primary" color="positive" label="Save" .state=${this._saveState === 'busy' ? 'waiting' : this._saveState === 'success' ? 'success' : this._saveState === 'error' ? 'failed' : undefined} @click=${this._save}>Save</uui-button>
          </div>
        </div>
      </umb-body-layout>
    `;
  }
}
declare global { interface HTMLElementTagNameMap { 'umbcheckout-stripe-settings': UmbCheckoutStripeSettings; } }
