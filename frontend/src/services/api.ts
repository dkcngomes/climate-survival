// In production (Netlify), NEXT_PUBLIC_API_URL is not set → use relative URLs
// Netlify proxies /api/* → Render backend automatically.
// In local dev, set NEXT_PUBLIC_API_URL=http://localhost:8080 in .env.local
const API_BASE = process.env.NEXT_PUBLIC_API_URL || "";

import type {
  RecommendationResponse,
  CropRecommendationResponse,
  CountryInfo,
  CountryLocale,
  IpCountryResult,
} from "@/types";

// ─── Helpers for resilience against HF Spaces cold starts ───

/** Fetch with AbortController timeout (default 25s). */
async function fetchWithTimeout(url: string, options: RequestInit = {}, timeoutMs = 25000) {
  const controller = new AbortController();
  const id = setTimeout(() => controller.abort(), timeoutMs);
  try {
    const res = await fetch(url, { ...options, signal: controller.signal });
    return res;
  } finally {
    clearTimeout(id);
  }
}

/**
 * Fetch with one automatic retry on timeout (AbortError) or 504 Gateway Timeout.
 * HF Spaces free tier goes to sleep after inactivity; the retry gives it time to
 * wake up while the first attempt already triggered the cold start.
 */
async function fetchWithRetry(url: string, options: RequestInit = {}) {
  for (let attempt = 0; attempt <= 1; attempt++) {
    try {
      const res = await fetchWithTimeout(url, options);
      if (res.status === 504 && attempt === 0) {
        // Gateway Timeout — likely HF cold start → wait and retry
        await new Promise(r => setTimeout(r, 2000));
        continue;
      }
      return res;
    } catch (err) {
      if (attempt === 0 && err instanceof DOMException && err.name === "AbortError") {
        // Timeout — likely cold start → wait and retry
        await new Promise(r => setTimeout(r, 2000));
        continue;
      }
      throw err;
    }
  }
  throw new Error("Service temporarily unavailable. Please try again.");
}

// ─── API functions ───

export async function fetchRecommendations(lat: number, lng: number) {
  const res = await fetchWithRetry(`${API_BASE}/api/recommendations?lat=${lat}&lng=${lng}`);
  if (!res.ok) throw new Error(`Recommendations failed: ${res.statusText}`);
  return res.json() as Promise<RecommendationResponse>;
}

export async function fetchCropRecommendations(lat: number, lng: number, countryCode?: string) {
  let url = `${API_BASE}/api/crops?lat=${lat}&lng=${lng}`;
  if (countryCode) url += `&countryCode=${countryCode}`;
  const res = await fetchWithRetry(url);
  if (!res.ok) {
    const err = await res.json().catch(() => ({ error: "Crop request failed" }));
    throw new Error(err.error || `HTTP ${res.status}`);
  }
  return res.json();
}

export async function fetchCountries() {
  const res = await fetch(`${API_BASE}/api/locations/countries`);
  if (!res.ok) throw new Error(`Countries failed: ${res.statusText}`);
  return res.json() as Promise<CountryInfo[]>;
}

export async function detectIpCountry() {
  try {
    const res = await fetch(`${API_BASE}/api/locations/ip-country`);
    if (!res.ok) return null;
    return res.json() as Promise<IpCountryResult>;
  } catch {
    return null;
  }
}

export async function fetchLanguages() {
  const res = await fetch(`${API_BASE}/api/locations/languages`);
  if (!res.ok) throw new Error(`Languages failed: ${res.statusText}`);
  return res.json() as Promise<{ code: string; name: string }[]>;
}

export async function fetchCountryLocale(countryCode: string) {
  const res = await fetch(`${API_BASE}/api/locations/locale/${countryCode}`);
  if (!res.ok) throw new Error(`Locale failed: ${res.statusText}`);
  return res.json() as Promise<CountryLocale>;
}

export interface ContactPayload {
  name: string;
  email: string;
  subject: string;
  message: string;
}

export async function submitContact(payload: ContactPayload) {
  const res = await fetch(`${API_BASE}/api/contact`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(payload),
  });
  if (!res.ok) {
    const err = await res.json().catch(() => ({ error: "Submission failed" }));
    throw new Error(err.error || `HTTP ${res.status}`);
  }
  return res.json() as Promise<{ success: boolean; messageId: string }>;
}

// ─── Sri Lanka Prices (CBSL) ───

export interface PriceEntry {
  market: string;
  yesterday: number | null;
  today: number | null;
  change: number | null;
}

export interface CommodityPrice {
  commodity: string;
  category: string;
  unit: string;
  prices: PriceEntry[];
}

export interface SriLankaPricesResponse {
  source: string;
  reportDate: string;
  lastUpdated: string;
  totalItems: number;
  commodities: CommodityPrice[];
}

export async function fetchSriLankaPrices() {
  const res = await fetch(`${API_BASE}/api/prices/sri-lanka`);
  if (!res.ok) throw new Error(`Prices failed: ${res.statusText}`);
  return res.json() as Promise<SriLankaPricesResponse>;
}

// ─── SMS Alerts ───

export async function fetchCarriers() {
  const res = await fetchWithRetry(`${API_BASE}/api/alerts/carriers`);
  if (!res.ok) throw new Error(`Carriers failed: ${res.statusText}`);
  return res.json() as Promise<import("@/types").CarrierEntry[]>;
}

export async function subscribeToAlerts(req: import("@/types").SubscribeRequest) {
  const res = await fetchWithRetry(`${API_BASE}/api/alerts/subscribe`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(req),
  });
  if (!res.ok) {
    const err = await res.json().catch(() => ({ error: "Subscribe failed" }));
    throw new Error(err.error || `HTTP ${res.status}`);
  }
  return res.json() as Promise<import("@/types").SmsSubscription>;
}

export async function unsubscribeFromAlerts(subscriptionId: string) {
  const res = await fetchWithRetry(`${API_BASE}/api/alerts/unsubscribe`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ subscriptionId }),
  });
  if (!res.ok) {
    const err = await res.json().catch(() => ({ error: "Unsubscribe failed" }));
    throw new Error(err.error || `HTTP ${res.status}`);
  }
  return res.json() as Promise<{ success: boolean }>;
}

export async function sendTestSms(phoneNumber: string, carrierCode: string) {
  const res = await fetchWithRetry(`${API_BASE}/api/alerts/test`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ phoneNumber, carrierCode }),
  });
  if (!res.ok) {
    const err = await res.json().catch(() => ({ error: "Test SMS failed" }));
    throw new Error(err.error || `HTTP ${res.status}`);
  }
  return res.json() as Promise<{ success: boolean; message: string }>;
}
