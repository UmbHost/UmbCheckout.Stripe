import { manifests as settingsManifests } from "./settings/manifests.js";
import { manifests as shippingRatesManifests } from "./shipping-rates/manifests.js";
import { manifests as localizationManifests } from "./localization/manifests.js";

// Collates all the manifests of the UmbCheckout Stripe backoffice, this bundle is loaded from the package manifest
export const manifests: Array<UmbExtensionManifest> = [
  ...settingsManifests,
  ...shippingRatesManifests,
  ...localizationManifests,
];
