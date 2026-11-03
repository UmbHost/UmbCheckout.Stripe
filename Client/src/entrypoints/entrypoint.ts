import type { UmbEntryPointOnInit, UmbEntryPointOnUnload } from "@umbraco-cms/backoffice/extension-api";
import { configureUmbCheckoutClient } from "@umbcheckout/core";
import { client } from "../api/client.gen.js";

export const onInit: UmbEntryPointOnInit = (host) => {
  configureUmbCheckoutClient(host, client);
};

// Required by the 17.0 UmbEntryPointModule type; nothing to tear down.
export const onUnload: UmbEntryPointOnUnload = () => {};
