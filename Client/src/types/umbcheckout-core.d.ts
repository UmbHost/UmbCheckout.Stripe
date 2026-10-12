// Types for the @umbcheckout/core module, provided at runtime by the UmbCheckout package's importmap.
// Keep in sync with UmbCheckout.Backoffice/Client/src/exports.ts.
declare module "@umbcheckout/core" {
  import type { UmbElement } from "@umbraco-cms/backoffice/element-api";
  import type { UmbContextMinimal, UmbContextToken } from "@umbraco-cms/backoffice/context-api";
  import type { Observable } from "@umbraco-cms/backoffice/observable-api";
  import type { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
  import type { CSSResult } from "@umbraco-cms/backoffice/external/lit";

  export interface UmbCheckoutApiClient {
    setConfig(config: { auth?: unknown; baseUrl?: string; credentials?: RequestCredentials }): unknown;
    interceptors: { response: { use(fn: (response: Response) => Response | Promise<Response>): unknown } };
  }
  export function configureUmbCheckoutClient(host: UmbElement, client: UmbCheckoutApiClient): void;
  export function whenUmbCheckoutClientReady(client: UmbCheckoutApiClient): Promise<void>;

  export const UMBCHECKOUT_SECTION_ALIAS: "umbCheckout";
  export const UMBCHECKOUT_SECTION_PATHNAME: "umbcheckout";
  export const UMBCHECKOUT_SETTINGS_MENU_ALIAS: "UmbCheckout.Menu.Settings";
  export const UMBCHECKOUT_STORE_MENU_ALIAS: "UmbCheckout.Menu.Store";
  export const UMBCHECKOUT_STRIPE_MENU_ALIAS: "UmbCheckout.Menu.Stripe";

  export type UmbCheckoutLicenseAddon = { name: string; status: string; nextDueDate: string };
  export type UmbCheckoutLicenseStatus = {
    status: string;
    isValid: boolean;
    isDevelopmentLicense: boolean;
    regDate: string;
    expiryDateTime: string;
    validDomains: string;
    validPaths: string;
    addons: Array<UmbCheckoutLicenseAddon>;
  };
  export type UmbCheckoutLicenseCheckResult = { accepted: boolean; retryAfterSeconds?: number | null };
  export interface UmbCheckoutLicenseContext extends UmbContextMinimal {
    readonly status: Observable<UmbCheckoutLicenseStatus | undefined>;
    readonly isLicensed: Observable<boolean>;
    requestCheck(): Promise<UmbCheckoutLicenseCheckResult | undefined>;
  }
  export const UMBCHECKOUT_LICENSE_CONTEXT: UmbContextToken<UmbCheckoutLicenseContext>;

  /** <umbcheckout-unlicensed-alert>: v13's red unlicensed alert; renders nothing when licensed. */
  export class UmbCheckoutUnlicensedAlertElement extends UmbLitElement {}
  /** <umbcheckout-made-by>: v13's "Made with ♥ by UmbHost" link; renders nothing when licensed. */
  export class UmbCheckoutMadeByElement extends UmbLitElement {}
  export const UmbCheckoutAlertStyles: CSSResult;
}
