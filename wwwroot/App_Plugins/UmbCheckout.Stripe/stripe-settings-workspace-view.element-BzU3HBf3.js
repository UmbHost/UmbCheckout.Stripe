import { html as s, nothing as U, css as E, state as b, customElement as w } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement as $ } from "@umbraco-cms/backoffice/lit-element";
import { UMB_WORKSPACE_CONTEXT as P } from "@umbraco-cms/backoffice/workspace";
import { UMBCHECKOUT_LICENSE_CONTEXT as x } from "@umbcheckout/core";
var S = Object.defineProperty, T = Object.getOwnPropertyDescriptor, v = (t) => {
  throw TypeError(t);
}, d = (t, e, o, r) => {
  for (var i = r > 1 ? void 0 : r ? T(e, o) : e, n = t.length - 1, p; n >= 0; n--)
    (p = t[n]) && (i = (r ? p(e, o, i) : p(i)) || i);
  return r && i && S(e, o, i), i;
}, h = (t, e, o) => e.has(t) || v("Cannot " + o), g = (t, e, o) => (h(t, e, "read from private field"), e.get(t)), m = (t, e, o) => e.has(t) ? v("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(t) : e.set(t, o), z = (t, e, o, r) => (h(t, e, "write to private field"), e.set(t, o), o), u = (t, e, o) => (h(t, e, "access private method"), o), c, a, f, k, y, C;
const _ = [
  { alias: "useLiveApiDetails", editor: "Umb.PropertyEditorUi.Toggle", key: "use_live_api_details" },
  { alias: "collectPhoneNumber", editor: "Umb.PropertyEditorUi.Toggle", key: "collect_phone_number" },
  { alias: "allowPromotionalCodes", editor: "Umb.PropertyEditorUi.Toggle", key: "allow_promotional_codes" },
  { alias: "collectPromotionalEmailsConsent", editor: "Umb.PropertyEditorUi.Toggle", key: "collect_promotional_emails_consent" },
  { alias: "shippingAllowedCountries", editor: "Umb.PropertyEditorUi.TextBox", key: "allowed_shipping_countries" },
  { alias: "enableAbandonedCartRecovery", editor: "Umb.PropertyEditorUi.Toggle", key: "enable_abandoned_cart_recovery" },
  { alias: "allowPromotionalCodesOnRecoveredCarts", editor: "Umb.PropertyEditorUi.Toggle", key: "allow_promotional_codes_on_recovered_carts" }
];
let l = class extends $ {
  constructor() {
    super(), m(this, a), m(this, c), this._licensed = !1, this.consumeContext(P, (t) => {
      z(this, c, t), this.observe(g(this, c)?.model, (e) => this._model = e, "_model");
    }), this.consumeContext(x, (t) => {
      this.observe(t?.isLicensed, (e) => this._licensed = e === !0, "_licensed");
    });
  }
  render() {
    return this._model ? s`${u(this, a, y).call(this)} ${u(this, a, C).call(this, this._model)}` : s`<uui-loader></uui-loader>`;
  }
};
c = /* @__PURE__ */ new WeakMap();
a = /* @__PURE__ */ new WeakSet();
f = function(t) {
  return _.map((e) => ({ alias: e.alias, value: t[e.alias] }));
};
k = function(t) {
  const e = t.target;
  for (const { alias: o, value: r } of e.value) {
    const i = _.find((n) => n.alias === o);
    i && g(this, c)?.setValue(i.alias, i.editor.endsWith("TextBox") ? r ?? "" : r === !0);
  }
};
y = function() {
  return s`<uui-box headline=${this.localize.term("umbcheckout_help_support")}>
      <p class="muted">
        ${this.localize.term("umbcheckout_documentation_description")}
        <a href="https://docs.umbcheckout.net/" target="_blank" rel="noopener">https://docs.umbcheckout.net/</a><br />
        ${this.localize.term("umbcheckout_github_report_issues")}
        <a href="https://github.com/UmbHost/UmbCheckout/issues" target="_blank" rel="noopener">${this.localize.term("umbcheckout_github_repo")}</a><br />
        ${this._licensed ? s`${this.localize.term("umbcheckout_support_ticket")}
              <a href="https://www.umbpanel.io" target="_blank" rel="noopener">UmbPanel</a>` : U}
      </p>
      <umbcheckout-unlicensed-alert></umbcheckout-unlicensed-alert>
    </uui-box>`;
};
C = function(t) {
  return s`<uui-box headline=${this.localize.term("umbcheckout_configuration")}>
      <umb-property-dataset .value=${u(this, a, f).call(this, t)} @change=${u(this, a, k)}>
        ${_.map(
    (e) => s`<umb-property
            alias=${e.alias}
            label=${this.localize.term(`umbcheckoutStripe_${e.key}_label`)}
            description=${this.localize.term(`umbcheckoutStripe_${e.key}`)}
            property-editor-ui-alias=${e.editor}></umb-property>`
  )}
      </umb-property-dataset>
    </uui-box>`;
};
l.styles = [
  E`
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
    `
];
d([
  b()
], l.prototype, "_model", 2);
d([
  b()
], l.prototype, "_licensed", 2);
l = d([
  w("umbcheckout-stripe-settings-workspace-view")
], l);
const L = l;
export {
  l as UmbCheckoutStripeSettingsWorkspaceViewElement,
  L as default
};
//# sourceMappingURL=stripe-settings-workspace-view.element-BzU3HBf3.js.map
