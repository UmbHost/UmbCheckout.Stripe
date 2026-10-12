import { html as c, state as p, customElement as _ } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement as v } from "@umbraco-cms/backoffice/lit-element";
import { UMB_COLLECTION_CONTEXT as f } from "@umbraco-cms/backoffice/collection";
var b = Object.defineProperty, C = Object.getOwnPropertyDescriptor, m = (e) => {
  throw TypeError(e);
}, u = (e, t, i, s) => {
  for (var a = s > 1 ? void 0 : s ? C(t, i) : t, o = e.length - 1, l; o >= 0; o--)
    (l = e[o]) && (a = (s ? l(t, i, a) : l(a)) || a);
  return s && a && b(t, i, a), a;
}, d = (e, t, i) => t.has(e) || m("Cannot " + i), w = (e, t, i) => t.has(e) ? m("Cannot add the same private member more than once") : t instanceof WeakSet ? t.add(e) : t.set(e, i), g = (e, t, i) => (d(e, t, "access private method"), i), r, h;
let n = class extends v {
  constructor() {
    super(), w(this, r), this._items = [], this._columns = [
      { name: this.localize.term("umbcheckoutStripe_shipping_rate_name"), alias: "name" },
      { name: this.localize.term("umbcheckoutStripe_shipping_rate_value"), alias: "value" }
    ], this.consumeContext(f, (e) => {
      this.observe(e?.items, (t) => this._items = (t ?? []).map((i) => g(this, r, h).call(this, i)), "_items");
    });
  }
  render() {
    return c`<umb-table .config=${{ allowSelection: !1 }} .columns=${this._columns} .items=${this._items}></umb-table>`;
  }
};
r = /* @__PURE__ */ new WeakSet();
h = function(e) {
  const t = `section/settings/workspace/umbcheckout-shipping-rate/edit/${e.unique}`;
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
  p()
], n.prototype, "_items", 2);
n = u([
  _("umbcheckout-shipping-rate-table-collection-view")
], n);
const O = n;
export {
  n as UmbCheckoutShippingRateTableCollectionViewElement,
  O as default
};
//# sourceMappingURL=shipping-rate-table-collection-view.element-Cs34SgI_.js.map
