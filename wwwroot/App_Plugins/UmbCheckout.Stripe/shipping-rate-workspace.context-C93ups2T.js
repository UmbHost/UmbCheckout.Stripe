import { UmbEntityDetailWorkspaceContextBase as p, UmbWorkspaceIsNewRedirectController as c, UmbWorkspaceIsNewRedirectControllerAlias as u } from "@umbraco-cms/backoffice/workspace";
import { a as l, U as m, b as h, c as _ } from "./bundle.manifests-Cs23f-QK.js";
import { html as d, customElement as C } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement as E } from "@umbraco-cms/backoffice/lit-element";
var b = Object.getOwnPropertyDescriptor, T = (o, e, t, r) => {
  for (var a = r > 1 ? void 0 : r ? b(e, t) : e, i = o.length - 1, n; i >= 0; i--)
    (n = o[i]) && (a = n(a) || a);
  return a;
};
let s = class extends E {
  render() {
    return d`<umb-entity-detail-workspace-editor back-path="section/settings/workspace/umbcheckout-shipping-rate-root"></umb-entity-detail-workspace-editor>`;
  }
};
s = T([
  C("umbcheckout-shipping-rate-workspace-editor")
], s);
class R extends p {
  constructor(e) {
    super(e, {
      workspaceAlias: h,
      entityType: m,
      detailRepositoryAlias: l
    }), this.name = this._data.createObservablePartOfCurrent((t) => t?.name), this.value = this._data.createObservablePartOfCurrent((t) => t?.value), this.routes.setRoutes([
      {
        path: "create",
        component: s,
        setup: async () => {
          await this.createScaffold({ parent: { entityType: _, unique: null } }), new c(
            this,
            this,
            this.getHostElement().shadowRoot.querySelector("umb-router-slot")
          );
        }
      },
      {
        path: "edit/:unique",
        component: s,
        setup: (t, r) => {
          this.removeUmbControllerByAlias(u), this.load(r.match.params.unique);
        }
      }
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
  R as UmbCheckoutShippingRateWorkspaceContext,
  R as api
};
//# sourceMappingURL=shipping-rate-workspace.context-C93ups2T.js.map
