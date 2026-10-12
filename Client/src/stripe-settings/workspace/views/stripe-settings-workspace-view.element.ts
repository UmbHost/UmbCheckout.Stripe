import { css, customElement, html, nothing, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import type { UmbPropertyDatasetElement, UmbPropertyValueData } from "@umbraco-cms/backoffice/property";
import { UMB_WORKSPACE_CONTEXT } from "@umbraco-cms/backoffice/workspace";
import { UMBCHECKOUT_LICENSE_CONTEXT } from "@umbcheckout/core";
import type { UmbCheckoutStripeSettingsWorkspaceContext } from "../stripe-settings-workspace.context.js";
import type { UmbCheckoutStripeSettingsModel } from "../../types.js";

type Field = { alias: Exclude<keyof UmbCheckoutStripeSettingsModel, "entityType" | "unique">; editor: string; key: string };

// v13's order (develop branch); the label is `<key>_label`, the description `<key>`.
const FIELDS: Array<Field> = [
  { alias: "useLiveApiDetails", editor: "Umb.PropertyEditorUi.Toggle", key: "use_live_api_details" },
  { alias: "collectPhoneNumber", editor: "Umb.PropertyEditorUi.Toggle", key: "collect_phone_number" },
  { alias: "allowPromotionalCodes", editor: "Umb.PropertyEditorUi.Toggle", key: "allow_promotional_codes" },
  { alias: "collectPromotionalEmailsConsent", editor: "Umb.PropertyEditorUi.Toggle", key: "collect_promotional_emails_consent" },
  { alias: "shippingAllowedCountries", editor: "Umb.PropertyEditorUi.TextBox", key: "allowed_shipping_countries" },
  { alias: "enableAbandonedCartRecovery", editor: "Umb.PropertyEditorUi.Toggle", key: "enable_abandoned_cart_recovery" },
  { alias: "allowPromotionalCodesOnRecoveredCarts", editor: "Umb.PropertyEditorUi.Toggle", key: "allow_promotional_codes_on_recovered_carts" },
];

/** v13's Settings → UmbCheckout → Stripe → Settings page: Help & Support, then the Stripe configuration. */
@customElement("umbcheckout-stripe-settings-workspace-view")
export class UmbCheckoutStripeSettingsWorkspaceViewElement extends UmbLitElement {
  #context?: UmbCheckoutStripeSettingsWorkspaceContext;

  @state() private _model?: UmbCheckoutStripeSettingsModel;
  @state() private _licensed = false;

  constructor() {
    super();
    this.consumeContext(UMB_WORKSPACE_CONTEXT, (context) => {
      this.#context = context as UmbCheckoutStripeSettingsWorkspaceContext;
      this.observe(this.#context?.model, (model) => (this._model = model), "_model");
    });
    this.consumeContext(UMBCHECKOUT_LICENSE_CONTEXT, (context) => {
      this.observe(context?.isLicensed, (licensed) => (this._licensed = licensed === true), "_licensed");
    });
  }

  #toDataset(model: UmbCheckoutStripeSettingsModel): Array<UmbPropertyValueData> {
    return FIELDS.map((f) => ({ alias: f.alias, value: model[f.alias] }));
  }

  #onChange(event: Event) {
    const dataset = event.target as UmbPropertyDatasetElement;
    for (const { alias, value } of dataset.value) {
      const field = FIELDS.find((f) => f.alias === alias);
      if (!field) continue;
      this.#context?.setValue(field.alias, (field.editor.endsWith("TextBox") ? (value ?? "") : value === true) as never);
    }
  }

  #renderHelp() {
    return html`<uui-box headline=${this.localize.term("umbcheckout_help_support")}>
      <p class="muted">
        ${this.localize.term("umbcheckout_documentation_description")}
        <a href="https://docs.umbcheckout.net/" target="_blank" rel="noopener">https://docs.umbcheckout.net/</a><br />
        ${this.localize.term("umbcheckout_github_report_issues")}
        <a href="https://github.com/UmbHost/UmbCheckout/issues" target="_blank" rel="noopener">${this.localize.term("umbcheckout_github_repo")}</a><br />
        ${this._licensed
          ? html`${this.localize.term("umbcheckout_support_ticket")}
              <a href="https://www.umbpanel.io" target="_blank" rel="noopener">UmbPanel</a>`
          : nothing}
      </p>
      <umbcheckout-unlicensed-alert></umbcheckout-unlicensed-alert>
    </uui-box>`;
  }

  #renderConfiguration(model: UmbCheckoutStripeSettingsModel) {
    return html`<uui-box headline=${this.localize.term("umbcheckout_configuration")}>
      <umb-property-dataset .value=${this.#toDataset(model)} @change=${this.#onChange}>
        ${FIELDS.map(
          (f) => html`<umb-property
            alias=${f.alias}
            label=${this.localize.term(`umbcheckoutStripe_${f.key}_label`)}
            description=${this.localize.term(`umbcheckoutStripe_${f.key}`)}
            property-editor-ui-alias=${f.editor}></umb-property>`,
        )}
      </umb-property-dataset>
    </uui-box>`;
  }

  override render() {
    if (!this._model) return html`<uui-loader></uui-loader>`;
    return html`${this.#renderHelp()} ${this.#renderConfiguration(this._model)}`;
  }

  static override styles = [
    css`
      :host {
        display: flex;
        flex-direction: column;
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

export default UmbCheckoutStripeSettingsWorkspaceViewElement;

declare global {
  interface HTMLElementTagNameMap {
    "umbcheckout-stripe-settings-workspace-view": UmbCheckoutStripeSettingsWorkspaceViewElement;
  }
}
