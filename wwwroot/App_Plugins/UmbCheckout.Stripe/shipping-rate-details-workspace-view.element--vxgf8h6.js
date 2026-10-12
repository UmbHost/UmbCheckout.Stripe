import { html as l, nothing as U, css as C, state as f, customElement as P } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement as W } from "@umbraco-cms/backoffice/lit-element";
import { tryExecute as D } from "@umbraco-cms/backoffice/resources";
import { UMB_WORKSPACE_CONTEXT as M } from "@umbraco-cms/backoffice/workspace";
import { formatMoney as $ } from "@umbcheckout/core";
import { S as N } from "./sdk.gen-DEbZmfQN.js";
import "./shipping-rates-help.element-BHyeEPx2.js";
var O = Object.defineProperty, A = Object.getOwnPropertyDescriptor, y = (t) => {
  throw TypeError(t);
}, m = (t, e, i, c) => {
  for (var n = c > 1 ? void 0 : c ? A(e, i) : e, _ = t.length - 1, d; _ >= 0; _--)
    (d = t[_]) && (n = (c ? d(e, i, n) : d(n)) || n);
  return c && n && O(e, i, n), n;
}, b = (t, e, i) => e.has(t) || y("Cannot " + i), h = (t, e, i) => (b(t, e, "read from private field"), e.get(t)), g = (t, e, i) => e.has(t) ? y("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(t) : e.set(t, i), T = (t, e, i, c) => (b(t, e, "write to private field"), e.set(t, i), i), r = (t, e, i) => (b(t, e, "access private method"), i), o, a, z, x, S, v, u, k, E, w, R;
const s = "umbcheckoutStripe_";
let p = class extends W {
  constructor() {
    super(), g(this, a), g(this, o), this._name = "", this._value = "", this.consumeContext(M, (t) => {
      T(this, o, t), this.observe(h(this, o)?.name, (e) => this._name = e ?? "", "_name"), this.observe(h(this, o)?.value, (e) => this._value = e ?? "", "_value");
    }), r(this, a, z).call(this);
  }
  render() {
    return l`${r(this, a, k).call(this)} ${r(this, a, E).call(this)} ${r(this, a, w).call(this)} ${r(this, a, R).call(this)}`;
  }
};
o = /* @__PURE__ */ new WeakMap();
a = /* @__PURE__ */ new WeakSet();
z = async function() {
  const { data: t } = await D(this, N.getStripeShippingRates());
  this._stripeRates = t ?? [];
};
x = function(t) {
  for (const { alias: e, value: i } of t.target.value)
    e === "name" && h(this, o)?.setName(i ?? ""), e === "value" && h(this, o)?.setValue(i ?? "");
};
S = function(t) {
  h(this, o)?.setName(t.displayName), h(this, o)?.setValue(t.id);
};
v = function(t) {
  if (!t) return "";
  const e = t.unit.replace(/_/g, " ");
  return `${t.value} ${t.value === 1 ? e : `${e}s`}`;
};
u = function(t, e, i) {
  return l`<umb-property-layout
      label=${this.localize.term(s + t)}
      description=${i ? this.localize.term(s + i) : ""}>
      <div slot="editor">${e ?? ""}</div>
    </umb-property-layout>`;
};
k = function() {
  return l`<umbcheckout-shipping-rates-help .dashboardUrl=${this._stripeRates?.[0]?.dashboardUrl}></umbcheckout-shipping-rates-help>`;
};
E = function() {
  const t = [
    { alias: "name", value: this._name },
    { alias: "value", value: this._value }
  ];
  return l`<uui-box headline=${this.localize.term(s + "shipping_rate")}>
      <umb-property-dataset .value=${t} @change=${r(this, a, x)}>
        <umb-property
          alias="name"
          label=${this.localize.term(s + "shipping_rate_label")}
          description=${this.localize.term(s + "shipping_rate_description")}
          property-editor-ui-alias="Umb.PropertyEditorUi.TextBox"
          .validation=${{ mandatory: !0 }}></umb-property>
        <umb-property
          alias="value"
          label=${this.localize.term(s + "shipping_rate_id_label")}
          description=${this.localize.term(s + "shipping_rate_id_description")}
          property-editor-ui-alias="Umb.PropertyEditorUi.TextBox"
          .validation=${{ mandatory: !0 }}></umb-property>
      </umb-property-dataset>
    </uui-box>`;
};
w = function() {
  const t = this._stripeRates?.find((e) => e.id === this._value);
  return t ? l`<uui-box headline=${this.localize.term(s + "shipping_rate_details")}>
      <p class="muted">${this.localize.term(s + "shipping_rate_details_description")}</p>
      ${r(this, a, u).call(this, "shipping_rate_detail_name", t.displayName)}
      ${r(this, a, u).call(this, "shipping_rate_detail_amount", $(this.localize, t.amount))}
      ${r(this, a, u).call(this, "shipping_rate_detail_minimum", r(this, a, v).call(this, t.deliveryEstimateMinimum))}
      ${r(this, a, u).call(this, "shipping_rate_detail_maximum", r(this, a, v).call(this, t.deliveryEstimateMaximum))}
      ${r(this, a, u).call(this, "shipping_rate_detail_edit", l`<uui-button look="outline" target="_blank" href=${t.dashboardUrl}
          label=${this.localize.term(s + "shipping_rate_detail_edit_button")}></uui-button>`)}
    </uui-box>` : U;
};
R = function() {
  const t = [
    { name: this.localize.term(s + "shipping_rate_detail_name"), alias: "name" },
    { name: this.localize.term(s + "shipping_rate_id_label"), alias: "id" },
    { name: this.localize.term(s + "shipping_rate_detail_amount"), alias: "amount" }
  ], e = (this._stripeRates ?? []).map((i) => ({
    id: i.id,
    icon: "icon-truck",
    data: [
      {
        columnAlias: "name",
        value: l`<uui-button look="link" compact label=${i.displayName} @click=${() => r(this, a, S).call(this, i)}></uui-button>`
      },
      { columnAlias: "id", value: i.id },
      { columnAlias: "amount", value: $(this.localize, i.amount) }
    ]
  }));
  return l`<uui-box headline=${this.localize.term(s + "shipping_rates")}>
      <p class="muted">${this.localize.term(s + "shipping_rates_description")}</p>
      ${this._stripeRates === void 0 ? l`<uui-loader></uui-loader>` : l`<umb-table .config=${{ allowSelection: !1 }} .columns=${t} .items=${e}></umb-table>`}
    </uui-box>`;
};
p.styles = [
  C`
      :host {
        display: grid;
        gap: var(--uui-size-layout-1);
        padding: var(--uui-size-layout-1);
      }
      .muted {
        color: var(--uui-color-text-alt);
        margin-top: 0;
      }
    `
];
m([
  f()
], p.prototype, "_name", 2);
m([
  f()
], p.prototype, "_value", 2);
m([
  f()
], p.prototype, "_stripeRates", 2);
p = m([
  P("umbcheckout-shipping-rate-details-workspace-view")
], p);
const Y = p;
export {
  p as UmbCheckoutShippingRateDetailsWorkspaceViewElement,
  Y as default
};
//# sourceMappingURL=shipping-rate-details-workspace-view.element--vxgf8h6.js.map
