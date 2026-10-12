import { customElement, html, property } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import type { ManifestMenuItem, UmbMenuItemElement } from "@umbraco-cms/backoffice/menu";
import { UMBCHECKOUT_STRIPE_MENU_ALIAS } from "@umbcheckout/core";

/**
 * v13's expandable "Stripe" node under Settings → UmbCheckout.
 * 17.0 has no expandable menu-item kind, so this renders its children like <umb-menu> does: every menuItem whose
 * meta.menus contains UmbCheckout.Menu.Stripe, slotted into an <umb-menu-item-layout> with has-children.
 */
@customElement("umbcheckout-stripe-menu-item")
export class UmbCheckoutStripeMenuItemElement extends UmbLitElement implements UmbMenuItemElement {
  @property({ attribute: false }) manifest?: ManifestMenuItem;

  override render() {
    return html`<umb-menu-item-layout
      has-children
      icon-name=${this.manifest?.meta.icon ?? "icon-credit-card"}
      label=${this.localize.string(this.manifest?.meta.label ?? "#umbcheckoutStripe_stripe")}>
      <umb-extension-slot
        type="menuItem"
        default-element="umb-menu-item-default"
        .filter=${(item: ManifestMenuItem) => item.meta.menus.includes(UMBCHECKOUT_STRIPE_MENU_ALIAS)}></umb-extension-slot>
    </umb-menu-item-layout>`;
  }
}

export default UmbCheckoutStripeMenuItemElement;

declare global {
  interface HTMLElementTagNameMap {
    "umbcheckout-stripe-menu-item": UmbCheckoutStripeMenuItemElement;
  }
}
