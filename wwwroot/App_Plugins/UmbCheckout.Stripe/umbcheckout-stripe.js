const t = [
  {
    name: "UmbCheckout.Stripe.Entrypoint",
    alias: "UmbCheckout.Stripe.Entrypoint",
    type: "backofficeEntryPoint",
    js: () => import("./entrypoint-B7Nx1T05.js")
  }
], e = [
  {
    type: "localization",
    alias: "UmbCheckout.Stripe.Localization.En",
    name: "UmbCheckout Stripe English",
    js: () => import("./en-CU3-f1_k.js"),
    meta: { culture: "en" }
  }
], i = [...t, ...e];
export {
  i as manifests
};
//# sourceMappingURL=umbcheckout-stripe.js.map
