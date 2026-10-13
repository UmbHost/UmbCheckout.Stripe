import { html as c, state as _, customElement as p } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement as v } from "@umbraco-cms/backoffice/lit-element";
import { UMB_COLLECTION_CONTEXT as f } from "@umbraco-cms/backoffice/collection";
import { a as C } from "./bundle.manifests-ClTqBx9r.js";
var b = Object.defineProperty, d = Object.getOwnPropertyDescriptor, m = (e) => {
  throw TypeError(e);
}, u = (e, t, a, o) => {
  for (var i = o > 1 ? void 0 : o ? d(t, a) : t, n = e.length - 1, l; n >= 0; n--)
    (l = e[n]) && (i = (o ? l(t, a, i) : l(i)) || i);
  return o && i && b(t, a, i), i;
}, E = (e, t, a) => t.has(e) || m("Cannot " + a), w = (e, t, a) => t.has(e) ? m("Cannot add the same private member more than once") : t instanceof WeakSet ? t.add(e) : t.set(e, a), S = (e, t, a) => (E(e, t, "access private method"), a), r, h;
let s = class extends v {
  constructor() {
    super(), w(this, r), this._items = [], this._columns = [
      { name: this.localize.term("umbcheckoutStripe_shipping_rate_name"), alias: "name" },
      { name: this.localize.term("umbcheckoutStripe_shipping_rate_value"), alias: "value" }
    ], this.consumeContext(f, (e) => {
      this.observe(e?.items, (t) => this._items = (t ?? []).map((a) => S(this, r, h).call(this, a)), "_items");
    });
  }
  render() {
    return c`<umb-table .config=${{ allowSelection: !1 }} .columns=${this._columns} .items=${this._items}></umb-table>`;
  }
};
r = /* @__PURE__ */ new WeakSet();
h = function(e) {
  const t = `${C}/edit/${e.unique}`;
  return {
    id: e.unique,
    icon: "icon-truck",
    data: [
      { columnAlias: "name", value: c`<a href=${t}>${e.name}</a>` },
      { columnAlias: "value", value: e.value }
    ]
  };
};
u([
  _()
], s.prototype, "_items", 2);
s = u([
  p("umbcheckout-shipping-rate-table-collection-view")
], s);
const U = s;
export {
  s as UmbCheckoutShippingRateTableCollectionViewElement,
  U as default
};
//# sourceMappingURL=shipping-rate-table-collection-view.element-paaAtNDt.js.map
