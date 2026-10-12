import type { UmbEntityModel } from "@umbraco-cms/backoffice/entity";

export interface UmbCheckoutStripeSettingsModel extends UmbEntityModel {
  useLiveApiDetails: boolean;
  collectPhoneNumber: boolean;
  allowPromotionalCodes: boolean;
  collectPromotionalEmailsConsent: boolean;
  shippingAllowedCountries: string;
  enableAbandonedCartRecovery: boolean;
  allowPromotionalCodesOnRecoveredCarts: boolean;
}
