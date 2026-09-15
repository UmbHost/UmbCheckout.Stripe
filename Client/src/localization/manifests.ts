export const manifests: Array<UmbExtensionManifest> = [
  {
    type: "localization",
    alias: "UmbCheckout.Stripe.Localization.En",
    name: "UmbCheckout Stripe English Localization",
    weight: -100,
    js: () => import("./en.js"),
    meta: {
      culture: "en",
    },
  },
];
