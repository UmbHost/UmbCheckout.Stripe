import { UMB_WORKSPACE_CONTEXT as _, UmbEntityDetailWorkspaceContextBase as b, UmbWorkspaceIsNewRedirectController as d, UmbWorkspaceIsNewRedirectControllerAlias as C } from "@umbraco-cms/backoffice/workspace";
import { a as E, b as P, c as T, U as O, d as y, e as I } from "./bundle.manifests-MLL5cmAF.js";
import { html as p, css as S, state as u, customElement as m } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement as h } from "@umbraco-cms/backoffice/lit-element";
var U = Object.defineProperty, f = Object.getOwnPropertyDescriptor, l = (a, e, t, r) => {
  for (var s = r > 1 ? void 0 : r ? f(e, t) : e, o = a.length - 1, n; o >= 0; o--)
    (n = a[o]) && (s = (r ? n(e, t, s) : n(s)) || s);
  return r && s && U(e, t, s), s;
};
let i = class extends h {
  constructor() {
    super(), this._name = "", this._isNew = !1, this.consumeContext(_, (a) => {
      const e = a;
      this.observe(e?.name, (t) => this._name = t ?? "", "_name"), this.observe(e?.isNew, (t) => this._isNew = t === !0, "_isNew");
    });
  }
  render() {
    const a = this._isNew || !this._name ? this.localize.term("umbcheckoutStripe_create_shipping_rate") : this._name;
    return p`<umb-entity-detail-workspace-editor .backPath=${E}>
      <h3 slot="header" class="headline">${a}</h3>
      <umbcheckout-made-by slot="footer-info"></umbcheckout-made-by>
    </umb-entity-detail-workspace-editor>`;
  }
};
i.styles = [
  S`
      .headline {
        margin: 0 0 0 var(--uui-size-space-3);
        font-size: var(--uui-type-h5-size);
        font-weight: 700;
        align-self: center;
      }
    `
];
l([
  u()
], i.prototype, "_name", 2);
l([
  u()
], i.prototype, "_isNew", 2);
i = l([
  m("umbcheckout-shipping-rate-workspace-editor")
], i);
var w = Object.getOwnPropertyDescriptor, A = (a, e, t, r) => {
  for (var s = r > 1 ? void 0 : r ? w(e, t) : e, o = a.length - 1, n; o >= 0; o--)
    (n = a[o]) && (s = n(s) || s);
  return s;
};
let c = class extends h {
  render() {
    return p`<umb-body-layout main-no-padding headline=${this.localize.term("umbcheckoutStripe_shipping_rates")}>
      <umb-collection alias=${P}></umb-collection>
    </umb-body-layout>`;
  }
};
c = A([
  m("umbcheckout-shipping-rate-collection-editor")
], c);
class k extends b {
  constructor(e) {
    super(e, {
      workspaceAlias: y,
      entityType: O,
      detailRepositoryAlias: T
    }), this.name = this._data.createObservablePartOfCurrent((t) => t?.name), this.value = this._data.createObservablePartOfCurrent((t) => t?.value), this.routes.setRoutes([
      {
        path: "create",
        component: i,
        setup: async () => {
          await this.createScaffold({ parent: { entityType: I, unique: null } }), new d(
            this,
            this,
            this.getHostElement().shadowRoot.querySelector("umb-router-slot")
          );
        }
      },
      {
        path: "edit/:unique",
        component: i,
        setup: (t, r) => {
          this.removeUmbControllerByAlias(C), this.load(r.match.params.unique);
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
  k as UmbCheckoutShippingRateWorkspaceContext,
  k as api
};
//# sourceMappingURL=shipping-rate-workspace.context-dPJ1dEQq.js.map
