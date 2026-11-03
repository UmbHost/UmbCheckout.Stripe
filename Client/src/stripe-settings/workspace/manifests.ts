import { UmbSubmitWorkspaceAction } from "@umbraco-cms/backoffice/workspace";
import { UMBCHECKOUT_STRIPE_SETTINGS_ENTITY_TYPE, UMBCHECKOUT_STRIPE_SETTINGS_WORKSPACE_ALIAS } from "../entity.js";

export const manifests: Array<UmbExtensionManifest> = [
  {
    type: "workspace",
    kind: "routable",
    alias: UMBCHECKOUT_STRIPE_SETTINGS_WORKSPACE_ALIAS,
    name: "UmbCheckout Stripe Settings Workspace",
    api: () => import("./stripe-settings-workspace.context.js"),
    meta: { entityType: UMBCHECKOUT_STRIPE_SETTINGS_ENTITY_TYPE },
  },
  {
    type: "workspaceView",
    alias: "UmbCheckout.WorkspaceView.StripeSettings",
    name: "UmbCheckout Stripe Settings View",
    element: () => import("./views/stripe-settings-workspace-view.element.js"),
    weight: 100,
    meta: { label: "#umbcheckoutStripe_settings", pathname: "settings", icon: "icon-settings" },
    conditions: [{ alias: "Umb.Condition.WorkspaceAlias", match: UMBCHECKOUT_STRIPE_SETTINGS_WORKSPACE_ALIAS }],
  },
  {
    type: "workspaceAction",
    kind: "default",
    alias: "UmbCheckout.WorkspaceAction.StripeSettings.Save",
    name: "Save UmbCheckout Stripe Settings",
    api: UmbSubmitWorkspaceAction,
    meta: { look: "primary", color: "positive", label: "#buttons_save" },
    conditions: [{ alias: "Umb.Condition.WorkspaceAlias", match: UMBCHECKOUT_STRIPE_SETTINGS_WORKSPACE_ALIAS }],
  },
];
