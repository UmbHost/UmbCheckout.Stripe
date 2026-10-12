import { html as m, state as _, customElement as c } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement as d } from "@umbraco-cms/backoffice/lit-element";
import { UMB_WORKSPACE_CONTEXT as f } from "@umbraco-cms/backoffice/workspace";
var y = Object.defineProperty, b = Object.getOwnPropertyDescriptor, h = (e) => {
  throw TypeError(e);
}, l = (e, t, r, s) => {
  for (var a = s > 1 ? void 0 : s ? b(t, r) : t, p = e.length - 1, n; p >= 0; p--)
    (n = e[p]) && (a = (s ? n(t, r, a) : n(a)) || a);
  return s && a && y(t, r, a), a;
}, v = (e, t, r) => t.has(e) || h("Cannot " + r), o = (e, t, r) => (v(e, t, "read from private field"), t.get(e)), g = (e, t, r) => t.has(e) ? h("Cannot add the same private member more than once") : t instanceof WeakSet ? t.add(e) : t.set(e, r), w = (e, t, r, s) => (v(e, t, "write to private field"), t.set(e, r), r), i;
let u = class extends d {
  constructor() {
    super(), g(this, i), this._name = "", this._value = "", this.consumeContext(f, (e) => {
      w(this, i, e), this.observe(o(this, i)?.name, (t) => this._name = t ?? "", "_name"), this.observe(o(this, i)?.value, (t) => this._value = t ?? "", "_value");
    });
  }
  render() {
    return m`<uui-box>
      <umb-property-layout label=${this.localize.term("umbcheckoutStripe_shipping_rate_name")} mandatory>
        <uui-input slot="editor" required .value=${this._name}
          @input=${(e) => o(this, i)?.setName(e.target.value)}></uui-input>
      </umb-property-layout>
      <umb-property-layout label=${this.localize.term("umbcheckoutStripe_shipping_rate_value")} description="shr_..." mandatory>
        <uui-input slot="editor" required .value=${this._value}
          @input=${(e) => o(this, i)?.setValue(e.target.value)}></uui-input>
      </umb-property-layout>
    </uui-box>`;
  }
};
i = /* @__PURE__ */ new WeakMap();
l([
  _()
], u.prototype, "_name", 2);
l([
  _()
], u.prototype, "_value", 2);
u = l([
  c("umbcheckout-shipping-rate-details-workspace-view")
], u);
const k = u;
export {
  u as UmbCheckoutShippingRateDetailsWorkspaceViewElement,
  k as default
};
//# sourceMappingURL=shipping-rate-details-workspace-view.element-DOxQQdrM.js.map
