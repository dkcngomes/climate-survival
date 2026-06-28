"use client";

import React, { createContext, useContext, useState, useCallback, useEffect, ReactNode } from "react";

// ── Types ──
export interface LocaleInfo {
  languageCode: string;
  languageName: string;
  currencyCode: string;
  currencySymbol: string;
  locale: string;
}

interface LocalizationContextType {
  language: string;
  setLanguage: (lang: string) => void;
  locale: LocaleInfo;
  setLocaleFromCountry: (countryCode: string) => void;
  t: (key: string, vars?: Record<string, string | number>) => string;
  formatCurrency: (amount: number) => string;
  availableLanguages: { code: string; name: string }[];
}

// ── Available languages ──
const AVAILABLE_LANGUAGES = [
  { code: "en", name: "English" },
  { code: "es", name: "Español" },
  { code: "fr", name: "Français" },
  { code: "pt", name: "Português" },
  { code: "de", name: "Deutsch" },
  { code: "zh", name: "中文" },
  { code: "ja", name: "日本語" },
  { code: "ko", name: "한국어" },
  { code: "ar", name: "العربية" },
  { code: "ru", name: "Русский" },
  { code: "hi", name: "हिन्दी" },
  { code: "id", name: "Bahasa Indonesia" },
  { code: "si", name: "සිංහල" },
];

// ── Currency map by country (subset, backend has full map) ──
const COUNTRY_CURRENCY: Record<string, { code: string; symbol: string; locale: string }> = {
  US: { code: "USD", symbol: "$", locale: "en-US" },
  GB: { code: "GBP", symbol: "£", locale: "en-GB" },
  EU: { code: "EUR", symbol: "€", locale: "en-EU" },
  LK: { code: "LKR", symbol: "Rs", locale: "si-LK" },
  IN: { code: "INR", symbol: "₹", locale: "hi-IN" },
  JP: { code: "JPY", symbol: "¥", locale: "ja-JP" },
  CN: { code: "CNY", symbol: "¥", locale: "zh-CN" },
  BR: { code: "BRL", symbol: "R$", locale: "pt-BR" },
  AU: { code: "AUD", symbol: "A$", locale: "en-AU" },
  CA: { code: "CAD", symbol: "CA$", locale: "en-CA" },
  MX: { code: "MXN", symbol: "MX$", locale: "es-MX" },
  DE: { code: "EUR", symbol: "€", locale: "de-DE" },
  FR: { code: "EUR", symbol: "€", locale: "fr-FR" },
  ES: { code: "EUR", symbol: "€", locale: "es-ES" },
  IT: { code: "EUR", symbol: "€", locale: "it-IT" },
  PT: { code: "EUR", symbol: "€", locale: "pt-PT" },
  NL: { code: "EUR", symbol: "€", locale: "nl-NL" },
  KR: { code: "KRW", symbol: "₩", locale: "ko-KR" },
  ID: { code: "IDR", symbol: "Rp", locale: "id-ID" },
  TH: { code: "THB", symbol: "฿", locale: "th-TH" },
  SG: { code: "SGD", symbol: "S$", locale: "en-SG" },
  HK: { code: "HKD", symbol: "HK$", locale: "zh-HK" },
  TW: { code: "TWD", symbol: "NT$", locale: "zh-TW" },
  AE: { code: "AED", symbol: "د.إ", locale: "ar-AE" },
  SA: { code: "SAR", symbol: "﷼", locale: "ar-SA" },
  RU: { code: "RUB", symbol: "₽", locale: "ru-RU" },
  TR: { code: "TRY", symbol: "₺", locale: "tr-TR" },
  ZA: { code: "ZAR", symbol: "R", locale: "af-ZA" },
  NG: { code: "NGN", symbol: "₦", locale: "en-NG" },
  EG: { code: "EGP", symbol: "E£", locale: "ar-EG" },
};

// ── Language → country code mapping for default currency per language ──
const LANG_DEFAULT_COUNTRY: Record<string, string> = {
  en: "US", es: "ES", fr: "FR", pt: "PT", de: "DE",
  zh: "CN", ja: "JP", ko: "KR", ar: "SA", ru: "RU",
  hi: "IN", id: "ID", nl: "NL", it: "IT", sv: "SE",
  nb: "NO", da: "DK", fi: "FI", pl: "PL", tr: "TR",
  uk: "UA", ms: "MY", th: "TH", vi: "VN", tl: "PH",
  he: "IL", bn: "BD", ur: "PK", ne: "NP", si: "LK",
  am: "ET", sw: "KE", af: "ZA",
};

// ── Dynamically load translations ──
const translationCache = new Map<string, Record<string, any>>();

async function loadTranslation(lang: string): Promise<Record<string, any>> {
  if (translationCache.has(lang)) return translationCache.get(lang)!;
  try {
    const mod = await import(`./translations/${lang}.json`);
    translationCache.set(lang, mod.default || mod);
    return translationCache.get(lang)!;
  } catch {
    // Fallback to English
    if (lang !== "en") return loadTranslation("en");
    return {};
  }
}

// ── Context ──
const LocalizationContext = createContext<LocalizationContextType | null>(null);

export function LocalizationProvider({ children }: { children: ReactNode }) {
  const [language, setLanguageState] = useState("en");
  const [translations, setTranslations] = useState<Record<string, any>>({});
  const [locale, setLocale] = useState<LocaleInfo>({
    languageCode: "en", languageName: "English",
    currencyCode: "USD", currencySymbol: "$", locale: "en-US",
  });
  const [loaded, setLoaded] = useState(false);

  // Load translations on language change
  useEffect(() => {
    loadTranslation(language).then((t) => {
      setTranslations(t);
      setLoaded(true);
    });
  }, [language]);

  // On mount, try to detect browser language
  useEffect(() => {
    const browserLang = navigator.language?.split("-")[0] || "en";
    if (AVAILABLE_LANGUAGES.some((l) => l.code === browserLang)) {
      setLanguageState(browserLang);
    } else {
      setLanguageState("en");
    }
    setLoaded(true);
  }, []);

  const setLanguage = useCallback((lang: string) => {
    setLanguageState(lang);
    // Auto-set currency based on language
    const countryCode = LANG_DEFAULT_COUNTRY[lang] || "US";
    const currency = COUNTRY_CURRENCY[countryCode] || COUNTRY_CURRENCY.US;
    setLocale({
      languageCode: lang,
      languageName: AVAILABLE_LANGUAGES.find((l) => l.code === lang)?.name || lang,
      currencyCode: currency.code,
      currencySymbol: currency.symbol,
      locale: currency.locale,
    });
  }, []);

  const setLocaleFromCountry = useCallback((countryCode: string) => {
    const currency = COUNTRY_CURRENCY[countryCode.toUpperCase()];
    if (currency) {
      setLocale((prev) => ({
        ...prev,
        currencyCode: currency.code,
        currencySymbol: currency.symbol,
        locale: currency.locale,
      }));
    }
  }, []);

  // Translation function: "stockUp.title" → get nested value, replace {{var}}
  const t = useCallback(
    (key: string, vars?: Record<string, string | number>): string => {
      const parts = key.split(".");
      let value: any = translations;
      for (const part of parts) {
        value = value?.[part];
      }
      if (typeof value !== "string") {
        // Try English fallback
        return key;
      }
      if (vars) {
        return value.replace(/{{(\w+)}}/g, (_, key) => {
          const v = vars[key];
          return v != null ? String(v) : `{{${key}}}`;
        });
      }
      return value;
    },
    [translations]
  );

  const formatCurrency = useCallback(
    (amount: number): string => {
      try {
        // Use Intl.NumberFormat for proper local currency formatting
        return new Intl.NumberFormat(locale.locale || "en-US", {
          style: "currency",
          currency: locale.currencyCode || "USD",
          minimumFractionDigits: 2,
          maximumFractionDigits: 2,
        }).format(amount);
      } catch {
        // Fallback
        return `${locale.currencySymbol}${amount.toFixed(2)}`;
      }
    },
    [locale]
  );

  if (!loaded) {
    return <div className="min-h-screen flex items-center justify-center text-gray-900">Loading...</div>;
  }

  return (
    <LocalizationContext.Provider
      value={{
        language,
        setLanguage,
        locale,
        setLocaleFromCountry,
        t,
        formatCurrency,
        availableLanguages: AVAILABLE_LANGUAGES,
      }}
    >
      {children}
    </LocalizationContext.Provider>
  );
}

export function useLocalization() {
  const ctx = useContext(LocalizationContext);
  if (!ctx) throw new Error("useLocalization must be used within LocalizationProvider");
  return ctx;
}
