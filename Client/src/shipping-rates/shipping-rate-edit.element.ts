import { css, customElement, html, nothing, property, repeat, state, unsafeHTML, when } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { UmbTextStyles } from "@umbraco-cms/backoffice/style";
import { tryExecute } from "@umbraco-cms/backoffice/resources";
import { UMB_NOTIFICATION_CONTEXT } from "@umbraco-cms/backoffice/notification";
import type { UUIButtonState, UUIInputElement } from "@umbraco-cms/backoffice/external/uui";
import {
  UmbCheckoutStripeService,
  getStripeShippingRatesDashboardUrl,
  type UmbCheckoutShippingRateRequestModel,
  type UmbCheckoutStripeShippingRateModel,
} from "../api/stripe.js";

@customElement("umbcheckout-stripe-shipping-rate-edit")
export class UmbCheckoutShippingRateEditElement extends UmbLitElement {
  /** The absolute path of the shipping rates workspace */
  @property({ attribute: false })
  workspacePath?: string;

  /** The key of the shipping rate being edited, undefined when creating */
  @property({ attribute: false })
  key?: string;

  @state()
  private _model: UmbCheckoutShippingRateRequestModel = { name: "", value: "" };

  @state()
  private _loaded = false;

  @state()
  private _stripeShippingRate?: UmbCheckoutStripeShippingRateModel;

  @state()
  private _stripeShippingRates?: Array<UmbCheckoutStripeShippingRateModel>;

  @state()
  private _stripeDashboardUrl = getStripeShippingRatesDashboardUrl(false);

  @state()
  private _saveButtonState?: UUIButtonState;

  @state()
  private _deleteButtonState?: UUIButtonState;

  @state()
  private _invalid: Array<keyof UmbCheckoutShippingRateRequestModel> = [];

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
    const [settings, stripeShippingRates] = await Promise.all([
      tryExecute(this, UmbCheckoutStripeService.getSettings()),
      tryExecute(this, UmbCheckoutStripeService.getStripeShippingRates()),
    ]);

    this._stripeDashboardUrl = getStripeShippingRatesDashboardUrl(settings.data?.useLiveApiDetails ?? false);
    this._stripeShippingRates = stripeShippingRates.data ?? [];

    if (this.key) {
      const { data } = await tryExecute(this, UmbCheckoutStripeService.getShippingRate(this.key));
      if (data) {
        this._model = { name: data.name, value: data.value };
        this.#loadStripeShippingRate(data.value);
      }
    }

    this._loaded = true;
  }

  async #loadStripeShippingRate(id: string) {
    if (!id) {
      this._stripeShippingRate = undefined;
      return;
    }

    const { data } = await tryExecute(this, UmbCheckoutStripeService.getStripeShippingRate(id), {
      disableNotifications: true,
    });
    this._stripeShippingRate = data;
  }

  #onSelectStripeShippingRate(stripeShippingRate: UmbCheckoutStripeShippingRateModel) {
    this._model = { name: stripeShippingRate.displayName, value: stripeShippingRate.id };
    this._stripeShippingRate = stripeShippingRate;
  }

  #notify(color: "positive" | "danger", key: string) {
    this.#notificationContext?.peek(color, {
      data: {
        headline: this.localize.term(`umbcheckoutstripe_shipping_rate_notification_${key}_title`),
        message: this.localize.term(`umbcheckoutstripe_shipping_rate_notification_${key}_message`),
      },
    });
  }

  async #onSave() {
    const invalid: Array<keyof UmbCheckoutShippingRateRequestModel> = [];
    if (!this._model.name.trim()) invalid.push("name");
    if (!this._model.value.trim()) invalid.push("value");
    this._invalid = invalid;
    if (invalid.length) {
      this._saveButtonState = "failed";
      return;
    }

    this._saveButtonState = "waiting";

    if (this.key) {
      const { data, error } = await tryExecute(this, UmbCheckoutStripeService.updateShippingRate(this.key, this._model), {
        disableNotifications: true,
      });

      if (error || !data) {
        this._saveButtonState = "failed";
        this.#notify("danger", "update_failed");
        return;
      }

      this._model = { name: data.name, value: data.value };
      this._saveButtonState = "success";
      this.#notify("positive", "updated");
      return;
    }

    const { data, error } = await tryExecute(this, UmbCheckoutStripeService.createShippingRate(this._model), {
      disableNotifications: true,
    });

    if (error || !data) {
      this._saveButtonState = "failed";
      this.#notify("danger", "create_failed");
      return;
    }

    this._saveButtonState = "success";
    this.#notify("positive", "created");
    window.history.pushState({}, "", `${this.workspacePath}/edit/${data.key}`);
  }

  async #onDelete() {
    if (!this.key) return;

    this._deleteButtonState = "waiting";

    const { error } = await tryExecute(this, UmbCheckoutStripeService.deleteShippingRate(this.key), {
      disableNotifications: true,
    });

    if (error) {
      this._deleteButtonState = "failed";
      this.#notify("danger", "delete_failed");
      return;
    }

    this._deleteButtonState = "success";
    this.#notify("positive", "deleted");
    window.history.pushState({}, "", this.workspacePath ?? "");
  }

  override render() {
    return html`
      <umb-body-layout headline=${this.localize.term("umbcheckoutstripe_shipping_rate")}>
        <uui-button
          slot="header"
          compact
          href=${this.workspacePath ?? ""}
          label=${this.localize.term("general_back")}>
          <uui-icon name="icon-arrow-left"></uui-icon>
        </uui-button>

        ${this._loaded
          ? html`<div class="content">
              <uui-box headline=${this.localize.term("umbcheckout_help_support")}>
                <p>
                  <uui-icon name="icon-info"></uui-icon>
                  ${unsafeHTML(this.localize.term("umbcheckoutstripe_shipping_help").replace("[[STRIPE_URL]]", this._stripeDashboardUrl))}
                </p>
                <!-- TODO: UmbHost.Licensing - re-enable the unlicensed warning once a Umbraco 17 compatible version of UmbHost.Licensing is available -->
              </uui-box>
              ${this.#renderForm()} ${this.#renderStripeShippingRate()} ${this.#renderStripeShippingRates()}
            </div>`
          : html`<uui-loader></uui-loader>`}

        ${when(
          this.key,
          () => html`<uui-button
            slot="actions"
            look="primary"
            color="danger"
            label=${this.localize.term("general_delete")}
            .state=${this._deleteButtonState}
            ?disabled=${this._deleteButtonState === "waiting"}
            @click=${this.#onDelete}></uui-button>`,
        )}
        <uui-button
          slot="actions"
          look="primary"
          color="positive"
          label=${this.localize.term("buttons_save")}
          .state=${this._saveButtonState}
          ?disabled=${!this._loaded || this._saveButtonState === "waiting"}
          @click=${this.#onSave}></uui-button>
      </umb-body-layout>
    `;
  }

  #renderForm() {
    return html`
      <uui-box headline=${this.localize.term("umbcheckoutstripe_shipping_rate")}>
        <umb-property-layout
          label=${this.localize.term("umbcheckoutstripe_shipping_rate_label")}
          description=${this.localize.term("umbcheckoutstripe_shipping_rate_description")}
          mandatory
          ?invalid=${this._invalid.includes("name")}>
          <uui-input
            slot="editor"
            label=${this.localize.term("umbcheckoutstripe_shipping_rate_label")}
            .value=${this._model.name}
            @input=${(e: Event) => (this._model = { ...this._model, name: (e.target as UUIInputElement).value as string })}></uui-input>
        </umb-property-layout>

        <umb-property-layout
          label=${this.localize.term("umbcheckoutstripe_shipping_rate_id_label")}
          description=${this.localize.term("umbcheckoutstripe_shipping_rate_id_description")}
          mandatory
          ?invalid=${this._invalid.includes("value")}>
          <uui-input
            slot="editor"
            label=${this.localize.term("umbcheckoutstripe_shipping_rate_id_label")}
            .value=${this._model.value}
            @input=${(e: Event) => (this._model = { ...this._model, value: (e.target as UUIInputElement).value as string })}
            @change=${() => this.#loadStripeShippingRate(this._model.value)}></uui-input>
        </umb-property-layout>
      </uui-box>
    `;
  }

  #renderStripeShippingRate() {
    const rate = this._stripeShippingRate;
    if (!rate) return nothing;

    return html`
      <uui-box
        headline=${this.localize.term("umbcheckoutstripe_shipping_rate_details")}>
        <p class="description">${this.localize.term("umbcheckoutstripe_shipping_rate_details_description")}</p>
        <umb-property-layout label=${this.localize.term("umbcheckoutstripe_shipping_rate_detail_name")}>
          <div slot="editor">${rate.displayName}</div>
        </umb-property-layout>
        <umb-property-layout
          label=${this.localize.term("umbcheckoutstripe_shipping_rate_detail_amount")}
          description=${this.localize.term("umbcheckoutstripe_shipping_rate_detail_amount_description")}>
          <div slot="editor">${rate.amount ?? ""} ${rate.currency ?? ""}</div>
        </umb-property-layout>
        <umb-property-layout label=${this.localize.term("umbcheckoutstripe_shipping_rate_detail_minimum")}>
          <div slot="editor">${rate.deliveryEstimateMinimumValue ?? ""} ${rate.deliveryEstimateMinimumUnit ?? ""}</div>
        </umb-property-layout>
        <umb-property-layout label=${this.localize.term("umbcheckoutstripe_shipping_rate_detail_maximum")}>
          <div slot="editor">${rate.deliveryEstimateMaximumValue ?? ""} ${rate.deliveryEstimateMaximumUnit ?? ""}</div>
        </umb-property-layout>
        <umb-property-layout label=${this.localize.term("umbcheckoutstripe_shipping_rate_detail_edit")}>
          <uui-button
            slot="editor"
            look="outline"
            color="positive"
            target="_blank"
            href=${`${this._stripeDashboardUrl}${rate.id}`}
            label=${this.localize.term("umbcheckoutstripe_shipping_rate_detail_edit_button")}></uui-button>
        </umb-property-layout>
      </uui-box>
    `;
  }

  #renderStripeShippingRates() {
    return html`
      <uui-box headline=${this.localize.term("umbcheckoutstripe_shipping_rates")}>
        <p class="description">${this.localize.term("umbcheckoutstripe_shipping_rates_description")}</p>
        <uui-table>
          <uui-table-head>
            <uui-table-head-cell>${this.localize.term("general_name")}</uui-table-head-cell>
            <uui-table-head-cell>${this.localize.term("umbcheckoutstripe_shipping_id")}</uui-table-head-cell>
          </uui-table-head>
          ${repeat(
            this._stripeShippingRates ?? [],
            (rate) => rate.id,
            (rate) => html`
              <uui-table-row
                selectable
                ?selected=${rate.id === this._model.value}
                @click=${() => this.#onSelectStripeShippingRate(rate)}>
                <uui-table-cell>${rate.displayName}</uui-table-cell>
                <uui-table-cell>${rate.id}</uui-table-cell>
              </uui-table-row>
            `,
          )}
        </uui-table>
      </uui-box>
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

      .description {
        margin-top: 0;
        color: var(--uui-color-text-alt);
      }

      uui-table-row[selectable] {
        cursor: pointer;
      }
    `,
  ];
}

declare global {
  interface HTMLElementTagNameMap {
    "umbcheckout-stripe-shipping-rate-edit": UmbCheckoutShippingRateEditElement;
  }
}
