import { css as n, property as u, customElement as h, html as m } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement as b } from "@umbraco-cms/backoffice/lit-element";
var _ = Object.defineProperty, d = Object.getOwnPropertyDescriptor, c = (t, r, l, i) => {
  for (var e = i > 1 ? void 0 : i ? d(r, l) : r, s = t.length - 1, p; s >= 0; s--)
    (p = t[s]) && (e = (i ? p(r, l, e) : p(e)) || e);
  return i && e && _(r, l, e), e;
};
const a = "umbcheckoutStripe_", f = "https://dashboard.stripe.com/test/shipping-rates/";
let o = class extends b {
  render() {
    const t = (this.dashboardUrl ?? f).replace(/[^/]+$/, "");
    return m`<uui-box headline=${this.localize.term("umbcheckout_help_support")}>
      <p class="muted">
        <umb-icon name="icon-info"></umb-icon>
        ${this.localize.term(a + "shipping_help_before")}
        <a href=${t} target="_blank" rel="noopener">${this.localize.term(a + "shipping_help_link")}</a>
        ${this.localize.term(a + "shipping_help_after")}
      </p>
      <umbcheckout-unlicensed-alert></umbcheckout-unlicensed-alert>
    </uui-box>`;
  }
};
o.styles = [
  n`
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
c([
  u({ attribute: !1 })
], o.prototype, "dashboardUrl", 2);
o = c([
  h("umbcheckout-shipping-rates-help")
], o);
//# sourceMappingURL=shipping-rates-help.element-BHyeEPx2.js.map
