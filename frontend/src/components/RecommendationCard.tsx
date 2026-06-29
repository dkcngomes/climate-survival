"use client";

import { useState, useEffect } from "react";
import { ItemRecommendation } from "@/types";
import { useLocalization } from "@/i18n/LocalizationContext";
import { getAffiliateSearchUrl } from "@/config/affiliate";
import { fetchProductImage } from "@/services/productImages";

interface Props {
  item: ItemRecommendation;
  index: number;
  countryCode?: string;
}

const riskColors: Record<string, { bg: string; text: string; dot: string }> = {
  Critical: { bg: "bg-red-50 border-red-200", text: "text-red-700", dot: "bg-red-500" },
  High: { bg: "bg-orange-50 border-orange-200", text: "text-orange-700", dot: "bg-orange-500" },
  Medium: { bg: "bg-yellow-50 border-yellow-200", text: "text-yellow-700", dot: "bg-yellow-500" },
  Low: { bg: "bg-green-50 border-green-200", text: "text-green-700", dot: "bg-green-500" },
};

const categoryIcons: Record<string, string> = {
  Grains: "🌾",
  "Canned & Preserved": "🥫",
  "Oils & Fats": "🫒",
  Protein: "🥩",
  Food: "🍚",
  Beverages: "🧃",
  Essentials: "🔋",
  Agriculture: "🌱",
  Health: "💊",
  Home: "🔧",
};

/** Renders a product image with emoji fallback */
function ProductImage({ itemName }: { itemName: string }) {
  const [imageUrl, setImageUrl] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(false);

  useEffect(() => {
    let cancelled = false;
    setLoading(true);
    setError(false);

    fetchProductImage(itemName).then((url) => {
      if (!cancelled) {
        setImageUrl(url);
        setLoading(false);
      }
    });
  }, [itemName]);

  if (loading) {
    return (
      <div className="w-full h-32 bg-gray-200 animate-pulse rounded-xl flex items-center justify-center">
        <span className="text-gray-400 text-xs">Loading...</span>
      </div>
    );
  }

  if (imageUrl && !error) {
    return (
      <div className="w-full h-32 rounded-xl overflow-hidden bg-white">
        <img
          src={imageUrl}
          alt={itemName}
          className="w-full h-full object-contain p-2"
          onError={() => setError(true)}
          loading="lazy"
        />
      </div>
    );
  }

  return null; // No image — caller falls back
}

export default function RecommendationCard({ item, index, countryCode }: Props) {
  const { t } = useLocalization();
  const colors = riskColors[item.riskLevel] || riskColors.Medium;
  const icon = categoryIcons[item.category] || "📦";
  const isSriLanka = countryCode === "LK";

  return (
    <div className={`rounded-2xl border-2 p-5 ${colors.bg} transition-all hover:shadow-md`}>
      {/* Product image */}
      <div className="mb-3">
        <ProductImage itemName={item.itemName} />
      </div>

      <div className="flex items-start justify-between mb-3">
        <div className="flex items-center gap-3">
          <span className="text-3xl">{icon}</span>
          <div>
            <div className="flex items-center gap-2">
              <span className="text-xs text-gray-600 font-mono">#{index + 1}</span>
              <span className={`px-2 py-0.5 rounded-full text-xs font-semibold ${colors.text} bg-white/80`}>
                {item.riskLevel}
              </span>
            </div>
            <h3 className="text-lg font-bold text-gray-900 mt-1">{item.itemName}</h3>
            <p className="text-xs text-gray-700">{item.category}</p>
          </div>
        </div>
      </div>

      <p className="text-sm text-gray-900 mb-3">{item.reason}</p>

      <div className="bg-white/80 rounded-xl p-3 space-y-2">
        <div className="flex items-start gap-2">
          <span className="text-blue-700 font-bold text-sm">💡</span>
          <p className="text-sm text-gray-900">
            <span className="font-semibold">{t("stockUp.action")}:</span> {item.suggestedAction}
          </p>
        </div>
        <div className="flex items-start gap-2">
          <span className="text-green-700 font-bold text-sm">📦</span>
          <p className="text-sm text-gray-900">
            <span className="font-semibold">{t("stockUp.storage")}:</span> {item.storageTip}
          </p>
        </div>
      </div>

      {/* Affiliate link — only shown for Sri Lanka (Daraz.lk) */}
      {isSriLanka && (
        <a
          href={getAffiliateSearchUrl(item.itemName)}
          target="_blank"
          rel="noopener noreferrer"
          className="mt-3 inline-flex items-center gap-2 bg-orange-500 hover:bg-orange-600 text-white px-4 py-2 rounded-xl text-sm font-semibold transition-colors"
        >
          🛒 Buy on Daraz
        </a>
      )}
    </div>
  );
}
