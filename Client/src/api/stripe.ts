import { umbHttpClient } from "@umbraco-cms/backoffice/http-client";

const basePath = "/umbraco/umbcheckout/api/v1/stripe";

const security = () => [{ scheme: "bearer" as const, type: "http" as const }];

const jsonHeaders = { "Content-Type": "application/json" };

export interface UmbCheckoutStripeSettingsModel {
  useLiveApiDetails: boolean;
  collectPhoneNumber: boolean;
  collectPromotionalEmailsConsent: boolean;
  allowPromotionalCodes: boolean;
  enableAbandonedCartRecovery: boolean;
  allowPromotionalCodesOnRecoveredCarts: boolean;
  shippingAllowedCountries: string;
}

export interface UmbCheckoutShippingRateModel {
  id: number;
  key: string;
  name: string;
  value: string;
}

export interface UmbCheckoutShippingRateRequestModel {
  name: string;
  value: string;
}

export interface UmbCheckoutStripeShippingRateModel {
  id: string;
  displayName: string;
  active: boolean;
  amount?: number | null;
  currency?: string | null;
  deliveryEstimateMinimumValue?: number | null;
  deliveryEstimateMinimumUnit?: string | null;
  deliveryEstimateMaximumValue?: number | null;
  deliveryEstimateMaximumUnit?: string | null;
}

export class UmbCheckoutStripeService {
  static getSettings() {
    return umbHttpClient.get<{ 200: UmbCheckoutStripeSettingsModel }>({
      security: security(),
      url: `${basePath}/settings`,
    });
  }

  static updateSettings(model: UmbCheckoutStripeSettingsModel) {
    return umbHttpClient.put<{ 200: UmbCheckoutStripeSettingsModel }>({
      security: security(),
      url: `${basePath}/settings`,
      body: model,
      headers: jsonHeaders,
    });
  }

  static getShippingRates() {
    return umbHttpClient.get<{ 200: Array<UmbCheckoutShippingRateModel> }>({
      security: security(),
      url: `${basePath}/shipping-rates`,
    });
  }

  static getShippingRate(key: string) {
    return umbHttpClient.get<{ 200: UmbCheckoutShippingRateModel }>({
      security: security(),
      url: `${basePath}/shipping-rates/${encodeURIComponent(key)}`,
    });
  }

  static createShippingRate(model: UmbCheckoutShippingRateRequestModel) {
    return umbHttpClient.post<{ 200: UmbCheckoutShippingRateModel }>({
      security: security(),
      url: `${basePath}/shipping-rates`,
      body: model,
      headers: jsonHeaders,
    });
  }

  static updateShippingRate(key: string, model: UmbCheckoutShippingRateRequestModel) {
    return umbHttpClient.put<{ 200: UmbCheckoutShippingRateModel }>({
      security: security(),
      url: `${basePath}/shipping-rates/${encodeURIComponent(key)}`,
      body: model,
      headers: jsonHeaders,
    });
  }

  static deleteShippingRate(key: string) {
    return umbHttpClient.delete<{ 200: unknown }>({
      security: security(),
      url: `${basePath}/shipping-rates/${encodeURIComponent(key)}`,
    });
  }

  static getStripeShippingRates() {
    return umbHttpClient.get<{ 200: Array<UmbCheckoutStripeShippingRateModel> }>({
      security: security(),
      url: `${basePath}/stripe-shipping-rates`,
    });
  }

  static getStripeShippingRate(id: string) {
    return umbHttpClient.get<{ 200: UmbCheckoutStripeShippingRateModel }>({
      security: security(),
      url: `${basePath}/stripe-shipping-rates/${encodeURIComponent(id)}`,
    });
  }
}

/** Gets the Stripe dashboard shipping rates URL for either live or test mode */
export function getStripeShippingRatesDashboardUrl(useLiveApiDetails: boolean) {
  return useLiveApiDetails
    ? "https://dashboard.stripe.com/shipping-rates/"
    : "https://dashboard.stripe.com/test/shipping-rates/";
}
