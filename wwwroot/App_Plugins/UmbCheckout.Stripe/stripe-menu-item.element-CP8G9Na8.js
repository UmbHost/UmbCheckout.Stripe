import { html as s, property as a, customElement as c } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement as p } from "@umbraco-cms/backoffice/lit-element";
import { UMBCHECKOUT_STRIPE_MENU_ALIAS as f } from "@umbcheckout/core";
var b = Object.defineProperty, h = Object.getOwnPropertyDescriptor, o = (t, m, i, r) => {
  for (var e = r > 1 ? void 0 : r ? h(m, i) : m, u = t.length - 1, l; u >= 0; u--)
    (l = t[u]) && (e = (r ? l(m, i, e) : l(e)) || e);
  return r && e && b(m, i, e), e;
};
let n = class extends p {
  render() {
    return s`<umb-menu-item-layout
      has-children
      icon-name=${this.manifest?.meta.icon ?? "icon-credit-card"}
      label=${this.localize.string(this.manifest?.meta.label ?? "#umbcheckoutStripe_stripe")}>
      <umb-extension-slot
        type="menuItem"
        default-element="umb-menu-item-default"
        .filter=${(t) => t.meta.menus.includes(f)}></umb-extension-slot>
    </umb-menu-item-layout>`;
  }
};
o([
  a({ attribute: !1 })
], n.prototype, "manifest", 2);
n = o([
  c("umbcheckout-stripe-menu-item")
], n);
const v = n;
export {
  n as UmbCheckoutStripeMenuItemElement,
  v as default
};
//# sourceMappingURL=stripe-menu-item.element-CP8G9Na8.js.map
