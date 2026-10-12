import { html as E, property as x, state as y, customElement as k } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement as M } from "@umbraco-cms/backoffice/lit-element";
import { umbExtensionsRegistry as S } from "@umbraco-cms/backoffice/extension-registry";
import { UMB_SECTION_CONTEXT as $ } from "@umbraco-cms/backoffice/section";
import { UMBCHECKOUT_STRIPE_MENU_ALIAS as w } from "@umbcheckout/core";
var T = Object.defineProperty, I = Object.getOwnPropertyDescriptor, b = (t) => {
  throw TypeError(t);
}, f = (t, e, i, a) => {
  for (var n = a > 1 ? void 0 : a ? I(e, i) : e, p = t.length - 1, u; p >= 0; p--)
    (u = t[p]) && (n = (a ? u(e, i, n) : u(n)) || n);
  return a && n && T(e, i, n), n;
}, v = (t, e, i) => e.has(t) || b("Cannot " + i), r = (t, e, i) => (v(t, e, "read from private field"), e.get(t)), h = (t, e, i) => e.has(t) ? b("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(t) : e.set(t, i), C = (t, e, i, a) => (v(t, e, "write to private field"), e.set(t, i), i), _ = (t, e, i) => (v(t, e, "access private method"), i), o, d, l, s, m;
let c = class extends M {
  constructor() {
    super(), h(this, s), this._expanded = !1, h(this, o), h(this, d, []), h(this, l, () => _(this, s, m).call(this)), this.consumeContext($, (t) => {
      this.observe(t?.pathname, (e) => {
        C(this, o, e ? `/section/${e}/workspace/` : void 0), _(this, s, m).call(this);
      }, "_sectionPath");
    }), this.observe(
      S.byTypeAndFilter("menuItem", (t) => t.meta.menus.includes(w)),
      (t) => {
        C(this, d, t.map((e) => e.meta.entityType).filter((e) => !!e)), _(this, s, m).call(this);
      },
      "_children"
    );
  }
  connectedCallback() {
    super.connectedCallback(), window.addEventListener("navigationend", r(this, l));
  }
  disconnectedCallback() {
    super.disconnectedCallback(), window.removeEventListener("navigationend", r(this, l));
  }
  render() {
    const t = this.localize.string(this.manifest?.meta.label ?? "#umbcheckoutStripe_stripe");
    return E`<uui-menu-item
      has-children
      label=${t}
      .showChildren=${this._expanded}
      @show-children=${() => this._expanded = !0}
      @hide-children=${() => this._expanded = !1}>
      <umb-icon slot="icon" name=${this.manifest?.meta.icon ?? "icon-credit-card"}></umb-icon>
      <umb-extension-slot
        type="menuItem"
        default-element="umb-menu-item-default"
        .filter=${(e) => e.meta.menus.includes(w)}></umb-extension-slot>
    </uui-menu-item>`;
  }
};
o = /* @__PURE__ */ new WeakMap();
d = /* @__PURE__ */ new WeakMap();
l = /* @__PURE__ */ new WeakMap();
s = /* @__PURE__ */ new WeakSet();
m = function() {
  if (!r(this, o)) return;
  const t = window.location.pathname.endsWith("/") ? window.location.pathname : `${window.location.pathname}/`;
  r(this, d).some((e) => t.includes(`${r(this, o)}${e}/`)) && (this._expanded = !0);
};
f([
  x({ attribute: !1 })
], c.prototype, "manifest", 2);
f([
  y()
], c.prototype, "_expanded", 2);
c = f([
  k("umbcheckout-stripe-menu-item")
], c);
const A = c;
export {
  c as UmbCheckoutStripeMenuItemElement,
  A as default
};
//# sourceMappingURL=stripe-menu-item.element-DgZ51gdV.js.map
