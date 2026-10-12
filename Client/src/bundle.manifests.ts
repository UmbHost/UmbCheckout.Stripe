import { manifests as entrypoints } from "./entrypoints/manifest.js";
import { manifests as localization } from "./localization/manifest.js";
import { manifests as shippingRate } from "./shipping-rate/manifests.js";

export const manifests: Array<UmbExtensionManifest> = [...entrypoints, ...localization, ...shippingRate];
