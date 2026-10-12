import { UmbEntityDetailWorkspaceContextBase as m } from "@umbraco-cms/backoffice/workspace";
import { h as p, g as n, i as u, f as l } from "./bundle.manifests-5bNtKFqe.js";
import { html as E, customElement as _ } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement as T } from "@umbraco-cms/backoffice/lit-element";
var S = Object.getOwnPropertyDescriptor, h = (s, e, t, i) => {
  for (var r = i > 1 ? void 0 : i ? S(e, t) : e, o = s.length - 1, c; o >= 0; o--)
    (c = s[o]) && (r = c(r) || r);
  return r;
};
let a = class extends T {
  render() {
    return E`<umb-workspace-editor headline=${this.localize.term("umbcheckout_section")}>
      <umbcheckout-made-by slot="footer-info"></umbcheckout-made-by>
    </umb-workspace-editor>`;
  }
};
a = h([
  _("umbcheckout-stripe-settings-workspace-editor")
], a);
class I extends m {
  constructor(e) {
    super(e, {
      workspaceAlias: u,
      entityType: n,
      detailRepositoryAlias: p
    }), this.model = this._data.createObservablePartOfCurrent((t) => t), this.routes.setRoutes([
      {
        path: "",
        component: a,
        setup: () => {
          this.load(l);
        }
      }
    ]);
  }
  setValue(e, t) {
    this._data.updateCurrent({ [e]: t });
  }
}
export {
  I as UmbCheckoutStripeSettingsWorkspaceContext,
  I as api
};
//# sourceMappingURL=stripe-settings-workspace.context-CVpY_uoy.js.map
