import { c as t } from "./client.gen-C2rnZBGa.js";
class a {
  static getShippingRates(e) {
    return (e?.client ?? t).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/management/api/v1/umbcheckout/stripe-shipping-rates",
      ...e
    });
  }
  static createShippingRate(e) {
    return (e?.client ?? t).post({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/management/api/v1/umbcheckout/stripe-shipping-rates",
      ...e,
      headers: {
        "Content-Type": "application/json",
        ...e?.headers
      }
    });
  }
  static deleteShippingRate(e) {
    return (e.client ?? t).delete({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/management/api/v1/umbcheckout/stripe-shipping-rates/{id}",
      ...e
    });
  }
  static getShippingRateById(e) {
    return (e.client ?? t).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/management/api/v1/umbcheckout/stripe-shipping-rates/{id}",
      ...e
    });
  }
  static updateShippingRate(e) {
    return (e.client ?? t).put({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/management/api/v1/umbcheckout/stripe-shipping-rates/{id}",
      ...e,
      headers: {
        "Content-Type": "application/json",
        ...e.headers
      }
    });
  }
  static getStripeShippingRates(e) {
    return (e?.client ?? t).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/management/api/v1/umbcheckout/stripe-shipping-rates/stripe",
      ...e
    });
  }
  static getStripeShippingRateById(e) {
    return (e.client ?? t).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/management/api/v1/umbcheckout/stripe-shipping-rates/stripe/{stripeId}",
      ...e
    });
  }
}
export {
  a as S
};
//# sourceMappingURL=sdk.gen-DEbZmfQN.js.map
