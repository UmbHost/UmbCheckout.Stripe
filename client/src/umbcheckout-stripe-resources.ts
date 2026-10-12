const API_BASE = '/umbraco/management/api/v1/umbcheckout';

export interface ConfigProperty {
  alias: string;
  label: string;
  description: string;
  value: any;
  view: string;
  validation: { mandatory: boolean; mandatoryMessage?: string };
  config?: Record<string, any>;
}

export interface ShippingRate {
  key: string;
  name: string;
  value: string;
}

export interface ShippingRateResponse {
  key: string;
  properties: ConfigProperty[];
}

export interface StripeShippingRateDetail {
  id: string;
  displayName: string;
  fixedAmount: { amount: number; currency: string };
  deliveryEstimate: {
    minimum: { unit: string; value: number } | null;
    maximum: { unit: string; value: number } | null;
  } | null;
}

async function fetchJson<T>(url: string, options?: RequestInit): Promise<T> {
  const response = await fetch(url, {
    ...options,
    headers: { 'Content-Type': 'application/json', ...options?.headers },
  });
  if (!response.ok) throw new Error(`HTTP ${response.status}`);
  return response.json();
}

export async function getStripeSettings(): Promise<ConfigProperty[]> {
  return fetchJson(`${API_BASE}/stripe-settings`);
}

export async function updateStripeSettings(values: Record<string, any>): Promise<void> {
  await fetch(`${API_BASE}/stripe-settings`, {
    method: 'PATCH',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(values),
  });
}

export async function getShippingRates(): Promise<ShippingRate[]> {
  return fetchJson(`${API_BASE}/stripe-shipping-rates/list`);
}

export async function getStripeShippingRates(): Promise<StripeShippingRateDetail[]> {
  return fetchJson(`${API_BASE}/stripe-shipping-rates/stripe`);
}

export async function getShippingRate(key: string): Promise<ShippingRateResponse> {
  return fetchJson(`${API_BASE}/stripe-shipping-rates/get?key=${key}`);
}

export async function getStripeShippingRate(id: string): Promise<StripeShippingRateDetail> {
  return fetchJson(`${API_BASE}/stripe-shipping-rates/stripe-detail?id=${id}`);
}

export async function createShippingRate(values: Record<string, any>): Promise<void> {
  await fetch(`${API_BASE}/stripe-shipping-rates/create`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(values),
  });
}

export async function updateShippingRate(values: Record<string, any>, key: string): Promise<void> {
  await fetch(`${API_BASE}/stripe-shipping-rates/update?key=${key}`, {
    method: 'PATCH',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(values),
  });
}

export async function deleteShippingRate(key: string): Promise<void> {
  await fetch(`${API_BASE}/stripe-shipping-rates/delete?key=${key}`, {
    method: 'DELETE',
  });
}
