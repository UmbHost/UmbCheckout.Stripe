import { createUmbCheckoutDeleteAction } from "@umbcheckout/core";

/** Deletes a shipping rate and confirms it with v13's toast. */
export const api = createUmbCheckoutDeleteAction(
  "umbcheckoutStripe_shipping_rate_notification_deleted_title",
  "umbcheckoutStripe_shipping_rate_notification_deleted_message",
);
