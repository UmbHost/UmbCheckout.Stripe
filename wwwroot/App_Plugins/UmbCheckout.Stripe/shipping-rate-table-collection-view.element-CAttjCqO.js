import { html as r, state as p, customElement as _ } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement as v } from "@umbraco-cms/backoffice/lit-element";
import { UMB_COLLECTION_CONTEXT as f } from "@umbraco-cms/backoffice/collection";
var b = Object.defineProperty, C = Object.getOwnPropertyDescriptor, m = (e) => {
  throw TypeError(e);
}, u = (e, t, i, o) => {
  for (var a = o > 1 ? void 0 : o ? C(t, i) : t, n = e.length - 1, l; n >= 0; n--)
    (l = e[n]) && (a = (o ? l(t, i, a) : l(a)) || a);
  return o && a && b(t, i, a), a;
}, d = (e, t, i) => t.has(e) || m("Cannot " + i), w = (e, t, i) => t.has(e) ? m("Cannot add the same private member more than once") : t instanceof WeakSet ? t.add(e) : t.set(e, i), E = (e, t, i) => (d(e, t, "access private method"), i), c, h;
let s = class extends v {
  constructor() {
    super(), w(this, c), this._items = [], this._columns = [
      { name: this.localize.term("umbcheckoutStripe_shipping_rate_name"), alias: "name" },
      { name: this.localize.term("umbcheckoutStripe_shipping_rate_value"), alias: "value" }
    ], this.consumeContext(f, (e) => {
      this.observe(e?.items, (t) => this._items = (t ?? []).map((i) => E(this, c, h).call(this, i)), "_items");
    });
  }
  render() {
    return r`<umb-table .config=${{ allowSelection: !1 }} .columns=${this._columns} .items=${this._items}></umb-table>`;
  }
};
c = /* @__PURE__ */ new WeakSet();
h = function(e) {
  const t = `section/umbcheckout/workspace/umbcheckout-shipping-rate/edit/${e.unique}`;
  return {
    id: e.unique,
    icon: "icon-truck",
    data: [
      { columnAlias: "name", value: r`<a href=${t}>${e.name}</a>` },
      { columnAlias: "value", value: e.value }
    ]
  };
};
u([
  p()
], s.prototype, "_items", 2);
s = u([
  _("umbcheckout-shipping-rate-table-collection-view")
], s);
const O = s;
export {
  s as UmbCheckoutShippingRateTableCollectionViewElement,
  O as default
};
//# sourceMappingURL=shipping-rate-table-collection-view.element-CAttjCqO.js.map
