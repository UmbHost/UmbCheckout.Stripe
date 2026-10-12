import { UMB_WORKSPACE_CONTEXT as E, UmbEntityDetailWorkspaceContextBase as y, UmbWorkspaceIsNewRedirectController as f, UmbWorkspaceIsNewRedirectControllerAlias as P } from "@umbraco-cms/backoffice/workspace";
import { a as S, b as U, c as T, U as O, d as w, e as A } from "./bundle.manifests-Djgyi0RL.js";
import { html as u, css as _, state as h, customElement as m } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement as d } from "@umbraco-cms/backoffice/lit-element";
import { tryExecute as I } from "@umbraco-cms/backoffice/resources";
import { S as R } from "./sdk.gen-DEbZmfQN.js";
import "./shipping-rates-help.element-BHyeEPx2.js";
var N = Object.defineProperty, g = Object.getOwnPropertyDescriptor, l = (t, e, a, r) => {
  for (var s = r > 1 ? void 0 : r ? g(e, a) : e, i = t.length - 1, o; i >= 0; i--)
    (o = t[i]) && (s = (r ? o(e, a, s) : o(s)) || s);
  return r && s && N(e, a, s), s;
};
let n = class extends d {
  constructor() {
    super(), this._name = "", this._isNew = !1, this.consumeContext(E, (t) => {
      const e = t;
      this.observe(e?.name, (a) => this._name = a ?? "", "_name"), this.observe(e?.isNew, (a) => this._isNew = a === !0, "_isNew");
    });
  }
  render() {
    const t = this._isNew || !this._name ? this.localize.term("umbcheckoutStripe_create_shipping_rate") : this._name;
    return u`<umb-entity-detail-workspace-editor .backPath=${S}>
      <h3 slot="header" class="headline">${t}</h3>
      <umbcheckout-made-by slot="footer-info"></umbcheckout-made-by>
    </umb-entity-detail-workspace-editor>`;
  }
};
n.styles = [
  _`
      .headline {
        margin: 0 0 0 var(--uui-size-space-3);
        font-size: var(--uui-type-h5-size);
        font-weight: 700;
        align-self: center;
      }
    `
];
l([
  h()
], n.prototype, "_name", 2);
l([
  h()
], n.prototype, "_isNew", 2);
n = l([
  m("umbcheckout-shipping-rate-workspace-editor")
], n);
var k = Object.defineProperty, H = Object.getOwnPropertyDescriptor, b = (t) => {
  throw TypeError(t);
}, C = (t, e, a, r) => {
  for (var s = r > 1 ? void 0 : r ? H(e, a) : e, i = t.length - 1, o; i >= 0; i--)
    (o = t[i]) && (s = (r ? o(e, a, s) : o(s)) || s);
  return r && s && k(e, a, s), s;
}, W = (t, e, a) => e.has(t) || b("Cannot " + a), B = (t, e, a) => e.has(t) ? b("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(t) : e.set(t, a), K = (t, e, a) => (W(t, e, "access private method"), a), p, v;
let c = class extends d {
  constructor() {
    super(), B(this, p), K(this, p, v).call(this);
  }
  render() {
    return u`<umb-body-layout main-no-padding headline=${this.localize.term("umbcheckoutStripe_shipping_rates")}>
      <div class="help"><umbcheckout-shipping-rates-help .dashboardUrl=${this._dashboardUrl}></umbcheckout-shipping-rates-help></div>
      <umb-collection alias=${U}></umb-collection>
      <div class="help"><umbcheckout-made-by></umbcheckout-made-by></div>
    </umb-body-layout>`;
  }
};
p = /* @__PURE__ */ new WeakSet();
v = async function() {
  const { data: t } = await I(this, R.getStripeShippingRates(), { disableNotifications: !0 });
  this._dashboardUrl = t?.[0]?.dashboardUrl;
};
c.styles = [
  _`
      .help {
        padding: var(--uui-size-layout-1) var(--uui-size-layout-1) 0;
      }
    `
];
C([
  h()
], c.prototype, "_dashboardUrl", 2);
c = C([
  m("umbcheckout-shipping-rate-collection-editor")
], c);
class Y extends y {
  constructor(e) {
    super(e, {
      workspaceAlias: w,
      entityType: O,
      detailRepositoryAlias: T
    }), this.name = this._data.createObservablePartOfCurrent((a) => a?.name), this.value = this._data.createObservablePartOfCurrent((a) => a?.value), this.routes.setRoutes([
      {
        path: "create",
        component: n,
        setup: async () => {
          await this.createScaffold({ parent: { entityType: A, unique: null } }), new f(
            this,
            this,
            this.getHostElement().shadowRoot.querySelector("umb-router-slot")
          );
        }
      },
      {
        path: "edit/:unique",
        component: n,
        setup: (a, r) => {
          this.removeUmbControllerByAlias(P), this.load(r.match.params.unique);
        }
      },
      // The Shipping Rates list.
      { path: "", component: c }
    ]);
  }
  setName(e) {
    this._data.updateCurrent({ name: e });
  }
  setValue(e) {
    this._data.updateCurrent({ value: e });
  }
}
export {
  Y as UmbCheckoutShippingRateWorkspaceContext,
  Y as api
};
//# sourceMappingURL=shipping-rate-workspace.context-CzAxVJqs.js.map
