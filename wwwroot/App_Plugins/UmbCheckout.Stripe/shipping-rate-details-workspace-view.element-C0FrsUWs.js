import { html as l, nothing as C, css as U, state as b, customElement as P } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement as W } from "@umbraco-cms/backoffice/lit-element";
import { tryExecute as D } from "@umbraco-cms/backoffice/resources";
import { UMB_WORKSPACE_CONTEXT as M } from "@umbraco-cms/backoffice/workspace";
import { formatMoney as $ } from "@umbcheckout/core";
import { S as N } from "./sdk.gen-DEbZmfQN.js";
var O = Object.defineProperty, A = Object.getOwnPropertyDescriptor, y = (t) => {
  throw TypeError(t);
}, m = (t, e, i, c) => {
  for (var n = c > 1 ? void 0 : c ? A(e, i) : e, _ = t.length - 1, d; _ >= 0; _--)
    (d = t[_]) && (n = (c ? d(e, i, n) : d(n)) || n);
  return c && n && O(e, i, n), n;
}, f = (t, e, i) => e.has(t) || y("Cannot " + i), h = (t, e, i) => (f(t, e, "read from private field"), e.get(t)), g = (t, e, i) => e.has(t) ? y("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(t) : e.set(t, i), T = (t, e, i, c) => (f(t, e, "write to private field"), e.set(t, i), i), r = (t, e, i) => (f(t, e, "access private method"), i), o, s, z, x, k, v, u, S, E, w, R;
const a = "umbcheckoutStripe_";
let p = class extends W {
  constructor() {
    super(), g(this, s), g(this, o), this._name = "", this._value = "", this.consumeContext(M, (t) => {
      T(this, o, t), this.observe(h(this, o)?.name, (e) => this._name = e ?? "", "_name"), this.observe(h(this, o)?.value, (e) => this._value = e ?? "", "_value");
    }), r(this, s, z).call(this);
  }
  render() {
    return l`${r(this, s, S).call(this)} ${r(this, s, E).call(this)} ${r(this, s, w).call(this)} ${r(this, s, R).call(this)}`;
  }
};
o = /* @__PURE__ */ new WeakMap();
s = /* @__PURE__ */ new WeakSet();
z = async function() {
  const { data: t } = await D(this, N.getStripeShippingRates());
  this._stripeRates = t ?? [];
};
x = function(t) {
  for (const { alias: e, value: i } of t.target.value)
    e === "name" && h(this, o)?.setName(i ?? ""), e === "value" && h(this, o)?.setValue(i ?? "");
};
k = function(t) {
  h(this, o)?.setName(t.displayName), h(this, o)?.setValue(t.id);
};
v = function(t) {
  if (!t) return "";
  const e = t.unit.replace(/_/g, " ");
  return `${t.value} ${t.value === 1 ? e : `${e}s`}`;
};
u = function(t, e, i) {
  return l`<umb-property-layout
      label=${this.localize.term(a + t)}
      description=${i ? this.localize.term(a + i) : ""}>
      <div slot="editor">${e ?? ""}</div>
    </umb-property-layout>`;
};
S = function() {
  const t = (this._stripeRates?.[0]?.dashboardUrl ?? "https://dashboard.stripe.com/test/shipping-rates/").replace(/[^/]+$/, "");
  return l`<uui-box headline=${this.localize.term("umbcheckout_help_support")}>
      <p class="muted">
        <umb-icon name="icon-info"></umb-icon>
        ${this.localize.term(a + "shipping_help_before")}
        <a href=${t} target="_blank" rel="noopener">${this.localize.term(a + "shipping_help_link")}</a>
        ${this.localize.term(a + "shipping_help_after")}
      </p>
      <umbcheckout-unlicensed-alert></umbcheckout-unlicensed-alert>
    </uui-box>`;
};
E = function() {
  const t = [
    { alias: "name", value: this._name },
    { alias: "value", value: this._value }
  ];
  return l`<uui-box headline=${this.localize.term(a + "shipping_rate")}>
      <umb-property-dataset .value=${t} @change=${r(this, s, x)}>
        <umb-property
          alias="name"
          label=${this.localize.term(a + "shipping_rate_label")}
          description=${this.localize.term(a + "shipping_rate_description")}
          property-editor-ui-alias="Umb.PropertyEditorUi.TextBox"
          .validation=${{ mandatory: !0 }}></umb-property>
        <umb-property
          alias="value"
          label=${this.localize.term(a + "shipping_rate_id_label")}
          description=${this.localize.term(a + "shipping_rate_id_description")}
          property-editor-ui-alias="Umb.PropertyEditorUi.TextBox"
          .validation=${{ mandatory: !0 }}></umb-property>
      </umb-property-dataset>
    </uui-box>`;
};
w = function() {
  const t = this._stripeRates?.find((e) => e.id === this._value);
  return t ? l`<uui-box headline=${this.localize.term(a + "shipping_rate_details")}>
      <p class="muted">${this.localize.term(a + "shipping_rate_details_description")}</p>
      ${r(this, s, u).call(this, "shipping_rate_detail_name", t.displayName)}
      ${r(this, s, u).call(this, "shipping_rate_detail_amount", $(this.localize, t.amount))}
      ${r(this, s, u).call(this, "shipping_rate_detail_minimum", r(this, s, v).call(this, t.deliveryEstimateMinimum))}
      ${r(this, s, u).call(this, "shipping_rate_detail_maximum", r(this, s, v).call(this, t.deliveryEstimateMaximum))}
      ${r(this, s, u).call(this, "shipping_rate_detail_edit", l`<uui-button look="outline" target="_blank" href=${t.dashboardUrl}
          label=${this.localize.term(a + "shipping_rate_detail_edit_button")}></uui-button>`)}
    </uui-box>` : C;
};
R = function() {
  const t = [
    { name: this.localize.term(a + "shipping_rate_detail_name"), alias: "name" },
    { name: this.localize.term(a + "shipping_rate_id_label"), alias: "id" },
    { name: this.localize.term(a + "shipping_rate_detail_amount"), alias: "amount" }
  ], e = (this._stripeRates ?? []).map((i) => ({
    id: i.id,
    icon: "icon-truck",
    data: [
      {
        columnAlias: "name",
        value: l`<uui-button look="link" compact label=${i.displayName} @click=${() => r(this, s, k).call(this, i)}></uui-button>`
      },
      { columnAlias: "id", value: i.id },
      { columnAlias: "amount", value: $(this.localize, i.amount) }
    ]
  }));
  return l`<uui-box headline=${this.localize.term(a + "shipping_rates")}>
      <p class="muted">${this.localize.term(a + "shipping_rates_description")}</p>
      ${this._stripeRates === void 0 ? l`<uui-loader></uui-loader>` : l`<umb-table .config=${{ allowSelection: !1 }} .columns=${t} .items=${e}></umb-table>`}
    </uui-box>`;
};
p.styles = [
  U`
      :host {
        display: grid;
        gap: var(--uui-size-layout-1);
        padding: var(--uui-size-layout-1);
      }
      .muted {
        color: var(--uui-color-text-alt);
        margin-top: 0;
      }
      .muted umb-icon {
        display: inline-block;
        vertical-align: text-bottom;
        margin-right: var(--uui-size-space-1);
      }
    `
];
m([
  b()
], p.prototype, "_name", 2);
m([
  b()
], p.prototype, "_value", 2);
m([
  b()
], p.prototype, "_stripeRates", 2);
p = m([
  P("umbcheckout-shipping-rate-details-workspace-view")
], p);
const X = p;
export {
  p as UmbCheckoutShippingRateDetailsWorkspaceViewElement,
  X as default
};
//# sourceMappingURL=shipping-rate-details-workspace-view.element-C0FrsUWs.js.map
