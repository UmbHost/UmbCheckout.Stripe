import { manifests as detail } from "./repository/detail/manifests.js";
import { manifests as item } from "./repository/item/manifests.js";
import { manifests as collection } from "./collection/manifests.js";
import { manifests as workspace } from "./workspace/manifests.js";
import { manifests as entityActions } from "./entity-actions/manifests.js";
import { manifests as menuItem } from "./menu-item/manifests.js";

export const manifests: Array<UmbExtensionManifest> = [...detail, ...item, ...collection, ...workspace, ...entityActions, ...menuItem];
