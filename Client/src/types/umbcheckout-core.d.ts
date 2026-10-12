// Types for the @umbcheckout/core module, provided at runtime by the UmbCheckout package's importmap.
// Keep in sync with UmbCheckout.Backoffice/Client/src/exports.ts.
declare module "@umbcheckout/core" {
  import type { UmbElement } from "@umbraco-cms/backoffice/element-api";
  import type { UmbContextToken } from "@umbraco-cms/backoffice/context-api";
  import type { Observable } from "@umbraco-cms/backoffice/observable-api";

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

  export type UmbCheckoutLicenseStatus = { status: string; isValid: boolean; isDevelopmentLicense: boolean; expiryDateTime: string };
  export interface UmbCheckoutLicenseContext {
    readonly status: Observable<UmbCheckoutLicenseStatus | undefined>;
    readonly isLicensed: Observable<boolean>;
    requestCheck(): Promise<void>;
  }
  export const UMBCHECKOUT_LICENSE_CONTEXT: UmbContextToken<UmbCheckoutLicenseContext>;
}
