import { customElement, html, property, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import type { ManifestMenuItem, UmbMenuItemElement } from "@umbraco-cms/backoffice/menu";
import { umbExtensionsRegistry } from "@umbraco-cms/backoffice/extension-registry";
import { UMB_SECTION_CONTEXT } from "@umbraco-cms/backoffice/section";
import { UMBCHECKOUT_STRIPE_MENU_ALIAS } from "@umbcheckout/core";

/**
 * v13's expandable "Stripe" node under Settings → UmbCheckout.
 * 17.0 has no expandable menu-item kind, so this renders its children like <umb-menu> does: every menuItem whose
 * meta.menus contains UmbCheckout.Menu.Stripe. Like v13's tree, it opens itself when a child's workspace is showing.
 */
@customElement("umbcheckout-stripe-menu-item")
export class UmbCheckoutStripeMenuItemElement extends UmbLitElement implements UmbMenuItemElement {
  @property({ attribute: false }) manifest?: ManifestMenuItem;

  @state() private _expanded = false;

  #sectionPath?: string;
  #childEntityTypes: Array<string> = [];
  #onNavigation = () => this.#expandForCurrentPath();

  constructor() {
    super();
    this.consumeContext(UMB_SECTION_CONTEXT, (context) => {
      this.observe(context?.pathname, (pathname) => {
        this.#sectionPath = pathname ? `/section/${pathname}/workspace/` : undefined;
        this.#expandForCurrentPath();
      }, "_sectionPath");
    });
    this.observe(
      umbExtensionsRegistry.byTypeAndFilter("menuItem", (item) => item.meta.menus.includes(UMBCHECKOUT_STRIPE_MENU_ALIAS)),
      (children) => {
        this.#childEntityTypes = children.map((child) => child.meta.entityType).filter((type): type is string => !!type);
        this.#expandForCurrentPath();
      },
      "_children",
    );
  }

  override connectedCallback() {
    super.connectedCallback();
    window.addEventListener("navigationend", this.#onNavigation);
  }

  override disconnectedCallback() {
    super.disconnectedCallback();
    window.removeEventListener("navigationend", this.#onNavigation);
  }

  /** Opens (never closes) when the current URL is a child's workspace, so a manual collapse is respected elsewhere. */
  #expandForCurrentPath() {
    if (!this.#sectionPath) return;
    const path = window.location.pathname.endsWith("/") ? window.location.pathname : `${window.location.pathname}/`;
    if (this.#childEntityTypes.some((type) => path.includes(`${this.#sectionPath}${type}/`))) {
      this._expanded = true;
    }
  }

  override render() {
    const label = this.localize.string(this.manifest?.meta.label ?? "#umbcheckoutStripe_stripe");
    return html`<uui-menu-item
      has-children
      label=${label}
      .showChildren=${this._expanded}
      @show-children=${() => (this._expanded = true)}
      @hide-children=${() => (this._expanded = false)}>
      <umb-icon slot="icon" name=${this.manifest?.meta.icon ?? "icon-credit-card"}></umb-icon>
      <umb-extension-slot
        type="menuItem"
        default-element="umb-menu-item-default"
        .filter=${(item: ManifestMenuItem) => item.meta.menus.includes(UMBCHECKOUT_STRIPE_MENU_ALIAS)}></umb-extension-slot>
    </uui-menu-item>`;
  }
}

export default UmbCheckoutStripeMenuItemElement;

declare global {
  interface HTMLElementTagNameMap {
    "umbcheckout-stripe-menu-item": UmbCheckoutStripeMenuItemElement;
  }
}
