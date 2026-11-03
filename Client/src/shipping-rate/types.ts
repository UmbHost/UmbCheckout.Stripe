import type { UmbEntityModel } from "@umbraco-cms/backoffice/entity";

export interface UmbCheckoutShippingRateDetailModel extends UmbEntityModel {
  unique: string;
  name: string;
  value: string;
}

export interface UmbCheckoutShippingRateItemModel {
  unique: string;
  entityType: string;
  name: string;
}
