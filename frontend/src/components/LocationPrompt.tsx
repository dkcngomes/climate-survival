"use client";

import { useState, useEffect } from "react";
import { LocationState, CountryInfo } from "@/types";
import { fetchCountries, detectIpCountry } from "@/services/api";
import { useLocalization } from "@/i18n/LocalizationContext";

interface Props {
  onLocationReady: (lat: number, lng: number, countryCode?: string) => void;
  location: LocationState;
}

export default function LocationPrompt({ onLocationReady, location }: Props) {
  const { t } = useLocalization();
  const [countries, setCountries] = useState<CountryInfo[]>([]);
  const [selectedCode, setSelectedCode] = useState("");
  const [detecting, setDetecting] = useState(true);
  const [loading, setLoading] = useState(false);
  const [useGeo, setUseGeo] = useState(false);

  // Load countries on mount
  useEffect(() => {
    (async () => {
      try {
        const data = await fetchCountries();
        setCountries(data);
      } catch {
        // Fallback: minimal list
        setCountries([
          { code: "LK", name: "Sri Lanka", latitude: 7.873054, longitude: 80.771797, region: "Asia" },
          { code: "US", name: "United States", latitude: 37.09024, longitude: -95.712891, region: "Americas" },
          { code: "GB", name: "United Kingdom", latitude: 55.378051, longitude: -3.435973, region: "Europe" },
          { code: "IN", name: "India", latitude: 20.593684, longitude: 78.96288, region: "Asia" },
          { code: "AU", name: "Australia", latitude: -25.274398, longitude: 133.775136, region: "Oceania" },
        ]);
      }
    })();
  }, []);

  // Detect country from IP
  useEffect(() => {
    (async () => {
      try {
        const result = await detectIpCountry();
        if (result?.countryCode) {
          setSelectedCode(result.countryCode);
        }
      } catch {
        // Silently fail — user can pick manually
      } finally {
        setDetecting(false);
      }
    })();
  }, []);

  const handleCountryChange = (code: string) => {
    setSelectedCode(code);
  };

  const handleSubmitCountry = () => {
    if (!selectedCode) return;
    const country = countries.find((c) => c.code === selectedCode);
    if (!country) return;
    onLocationReady(country.latitude, country.longitude, country.code);
  };

  const handleGeoDetect = () => {
    if (!navigator.geolocation) return;

    setLoading(true);
    navigator.geolocation.getCurrentPosition(
      async (pos) => {
        setLoading(false);
        setUseGeo(true);
        // Try to detect country from IP too
        try {
          const { detectIpCountry } = await import("@/services/api");
          const ipResult = await detectIpCountry();
          const countryCode = ipResult?.countryCode || undefined;
          onLocationReady(pos.coords.latitude, pos.coords.longitude, countryCode);
        } catch {
          onLocationReady(pos.coords.latitude, pos.coords.longitude);
        }
      },
      () => {
        setLoading(false);
      },
      { enableHighAccuracy: true, timeout: 10000 }
    );
  };

  // Group by region for the dropdown
  const grouped = countries.reduce<Record<string, CountryInfo[]>>((acc, c) => {
    (acc[c.region] = acc[c.region] || []).push(c);
    return acc;
  }, {});

  const regionOrder = ["Americas", "Europe", "Asia", "Africa", "Oceania"];

  return (
    <div className="bg-white rounded-2xl shadow-lg p-8 max-w-lg mx-auto">
      <div className="text-center mb-6">
        <div className="text-5xl mb-4">🌍</div>
        <h2 className="text-2xl font-bold text-gray-900">🌍 {t("location.title")}</h2>
        <p className="text-gray-800 mt-2">
          {t("location.description")}
        </p>
      </div>

      {/* Loading countries / detecting IP */}
      {detecting && (
        <div className="text-center text-sm text-gray-600 mb-4">
          <span className="inline-block animate-pulse">📍 {t("location.detecting")}</span>
        </div>
      )}

      {/* Country dropdown */}
      <div className="mb-4">
        <label className="block text-sm font-medium text-gray-800 mb-2">
          {t("location.country")}
        </label>
        <select
          value={selectedCode}
          onChange={(e) => handleCountryChange(e.target.value)}
          className="w-full px-4 py-3 border border-gray-300 rounded-xl bg-white text-gray-900 focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none appearance-none cursor-pointer"
        >
          <option value="">{t("location.selectCountry")}</option>
          {regionOrder.map((region) => {
            const regionCountries = grouped[region];
            if (!regionCountries?.length) return null;
            return (
              <optgroup key={region} label={region}>
                {regionCountries
                  .sort((a, b) => a.name.localeCompare(b.name))
                  .map((c) => (
                    <option key={c.code} value={c.code}>
                      {c.name}
                    </option>
                  ))}
              </optgroup>
            );
          })}
        </select>
      </div>

      {/* Submit button */}
      <button
        onClick={handleSubmitCountry}
        disabled={!selectedCode}
        className="w-full py-3 px-6 bg-blue-600 hover:bg-blue-700 disabled:bg-gray-300 disabled:text-gray-500 text-white font-semibold rounded-xl transition-colors mb-3"
      >
        📍 {t("location.getRecommendations")}
      </button>

      {/* Divider */}
      <div className="flex items-center gap-3 my-4">
        <div className="flex-1 border-t border-gray-200" />
        <span className="text-xs text-gray-600">{t("location.or")}</span>
        <div className="flex-1 border-t border-gray-200" />
      </div>

      {/* Browser geolocation */}
      <button
        onClick={handleGeoDetect}
        disabled={loading}
        className="w-full py-3 px-6 bg-gray-100 hover:bg-gray-200 disabled:bg-gray-100 text-gray-800 font-semibold rounded-xl transition-colors flex items-center justify-center gap-2"
      >
        {loading ? (
          <>
            <svg className="animate-spin h-5 w-5" viewBox="0 0 24 24">
              <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" fill="none" />
              <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z" />
            </svg>
            {t("loading")}
          </>
        ) : (
          <>
            📡 {t("location.useMyLocation")}
          </>
        )}
      </button>

      {useGeo && (
        <p className="text-xs text-green-600 text-center mt-2">
          ✅ {t("location.usingLocation")}
        </p>
      )}
    </div>
  );
}
