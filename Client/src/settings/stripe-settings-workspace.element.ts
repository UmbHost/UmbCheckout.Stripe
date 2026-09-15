import { css, customElement, html, state, unsafeHTML } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { UmbTextStyles } from "@umbraco-cms/backoffice/style";
import { tryExecute } from "@umbraco-cms/backoffice/resources";
import { UMB_NOTIFICATION_CONTEXT } from "@umbraco-cms/backoffice/notification";
import type { UUIBooleanInputEvent, UUIButtonState, UUIInputElement } from "@umbraco-cms/backoffice/external/uui";
import { UmbCheckoutStripeService, type UmbCheckoutStripeSettingsModel } from "../api/stripe.js";

@customElement("umbcheckout-stripe-settings-workspace")
export class UmbCheckoutStripeSettingsWorkspaceElement extends UmbLitElement {
  @state()
  private _settings?: UmbCheckoutStripeSettingsModel;

  @state()
  private _saveButtonState?: UUIButtonState;

  #notificationContext?: typeof UMB_NOTIFICATION_CONTEXT.TYPE;

  constructor() {
    super();

    this.consumeContext(UMB_NOTIFICATION_CONTEXT, (context) => {
      this.#notificationContext = context;
    });
  }

  override connectedCallback() {
    super.connectedCallback();
    this.#load();
  }

  async #load() {
    const { data } = await tryExecute(this, UmbCheckoutStripeService.getSettings());
    if (data) {
      this._settings = data;
    }
  }

  #update<K extends keyof UmbCheckoutStripeSettingsModel>(key: K, value: UmbCheckoutStripeSettingsModel[K]) {
    if (!this._settings) return;
    this._settings = { ...this._settings, [key]: value };
  }

  async #onSave() {
    if (!this._settings) return;

    this._saveButtonState = "waiting";

    const { data, error } = await tryExecute(this, UmbCheckoutStripeService.updateSettings(this._settings), {
      disableNotifications: true,
    });

    if (error || !data) {
      this._saveButtonState = "failed";
      this.#notificationContext?.peek("danger", {
        data: {
          headline: this.localize.term("umbcheckoutstripe_settings_save_failed_title"),
          message: this.localize.term("umbcheckoutstripe_settings_save_failed_message"),
        },
      });
      return;
    }

    this._settings = data;
    this._saveButtonState = "success";
    this.#notificationContext?.peek("positive", {
      data: {
        headline: this.localize.term("umbcheckoutstripe_settings_saved_title"),
        message: this.localize.term("umbcheckoutstripe_settings_saved_message"),
      },
    });
  }

  override render() {
    return html`
      <umb-body-layout headline=${this.localize.term("umbcheckoutstripe_stripe_settings")}>
        <div class="content">
          <uui-box headline=${this.localize.term("umbcheckout_help_support")}>
            <p>
              ${unsafeHTML(this.localize.term("umbcheckout_documentation_description"))}<br />
              ${unsafeHTML(this.localize.term("umbcheckout_github_report_issues"))}
            </p>
            <!-- TODO: UmbHost.Licensing - re-enable the support ticket link and unlicensed warning once a Umbraco 17 compatible version of UmbHost.Licensing is available -->
          </uui-box>
          ${this._settings ? this.#renderSettings(this._settings) : html`<uui-loader></uui-loader>`}
        </div>
        <uui-button
          slot="actions"
          look="primary"
          color="positive"
          label=${this.localize.term("buttons_save")}
          .state=${this._saveButtonState}
          ?disabled=${!this._settings || this._saveButtonState === "waiting"}
          @click=${this.#onSave}></uui-button>
      </umb-body-layout>
    `;
  }

  #renderSettings(settings: UmbCheckoutStripeSettingsModel) {
    return html`
      <uui-box headline=${this.localize.term("umbcheckout_configuration")}>
        <umb-property-layout
          label=${this.localize.term("umbcheckout_use_live_api_details_label")}
          description=${this.localize.term("umbcheckout_use_live_api_details")}>
          <uui-toggle
            slot="editor"
            label=${this.localize.term("umbcheckout_use_live_api_details_label")}
            .checked=${settings.useLiveApiDetails}
            @change=${(e: UUIBooleanInputEvent) => this.#update("useLiveApiDetails", e.target.checked)}></uui-toggle>
        </umb-property-layout>

        <umb-property-layout
          label=${this.localize.term("umbcheckoutstripe_collect_phone_number_label")}
          description=${this.localize.term("umbcheckoutstripe_collect_phone_number")}>
          <uui-toggle
            slot="editor"
            label=${this.localize.term("umbcheckoutstripe_collect_phone_number_label")}
            .checked=${settings.collectPhoneNumber}
            @change=${(e: UUIBooleanInputEvent) => this.#update("collectPhoneNumber", e.target.checked)}></uui-toggle>
        </umb-property-layout>

        ${this.#renderToggle(settings, "allowPromotionalCodes", "allow_promotional_codes")}
        ${this.#renderToggle(settings, "collectPromotionalEmailsConsent", "collect_promotional_emails_consent")}

        <umb-property-layout
          label=${this.localize.term("umbcheckoutstripe_allowed_shipping_countries_label")}
          description=${this.localize.term("umbcheckoutstripe_allowed_shipping_countries")}>
          <uui-input
            slot="editor"
            label=${this.localize.term("umbcheckoutstripe_allowed_shipping_countries_label")}
            placeholder="GB,US"
            .value=${settings.shippingAllowedCountries ?? ""}
            @input=${(e: Event) =>
              this.#update("shippingAllowedCountries", (e.target as UUIInputElement).value as string)}></uui-input>
        </umb-property-layout>

        ${this.#renderToggle(settings, "enableAbandonedCartRecovery", "enable_abandoned_cart_recovery")}
        ${this.#renderToggle(settings, "allowPromotionalCodesOnRecoveredCarts", "allow_promotional_codes_on_recovered_carts")}
      </uui-box>
    `;
  }

  #renderToggle(
    settings: UmbCheckoutStripeSettingsModel,
    key: "collectPromotionalEmailsConsent" | "allowPromotionalCodes" | "enableAbandonedCartRecovery" | "allowPromotionalCodesOnRecoveredCarts",
    localizationKey: string,
  ) {
    const label = this.localize.term(`umbcheckoutstripe_${localizationKey}_label`);
    return html`
      <umb-property-layout label=${label} description=${this.localize.term(`umbcheckoutstripe_${localizationKey}`)}>
        <uui-toggle
          slot="editor"
          label=${label}
          .checked=${settings[key]}
          @change=${(e: UUIBooleanInputEvent) => this.#update(key, e.target.checked)}></uui-toggle>
      </umb-property-layout>
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

export default UmbCheckoutStripeSettingsWorkspaceElement;

declare global {
  interface HTMLElementTagNameMap {
    "umbcheckout-stripe-settings-workspace": UmbCheckoutStripeSettingsWorkspaceElement;
  }
}
