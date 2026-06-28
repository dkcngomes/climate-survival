"use client";

import { useLocalization } from "./LocalizationContext";
import { useState, useRef, useEffect } from "react";

export default function LanguageSwitcher() {
  const { language, setLanguage, availableLanguages, t } = useLocalization();
  const [open, setOpen] = useState(false);
  const ref = useRef<HTMLDivElement>(null);

  // Close on outside click
  useEffect(() => {
    const handler = (e: MouseEvent) => {
      if (ref.current && !ref.current.contains(e.target as Node)) {
        setOpen(false);
      }
    };
    document.addEventListener("mousedown", handler);
    return () => document.removeEventListener("mousedown", handler);
  }, []);

  const current = availableLanguages.find((l) => l.code === language) || availableLanguages[0];

  return (
    <div className="relative" ref={ref}>
      <button
        onClick={() => setOpen(!open)}
        className="flex items-center gap-1.5 px-3 py-1.5 text-sm border border-gray-300 rounded-lg bg-white hover:bg-gray-50 text-gray-900 transition-colors"
      >
        <span className="text-base leading-none">
          {language === "en" ? "🇬🇧" :
           language === "es" ? "🇪🇸" :
           language === "fr" ? "🇫🇷" :
           language === "pt" ? "🇵🇹" :
           language === "de" ? "🇩🇪" :
           language === "zh" ? "🇨🇳" :
           language === "ja" ? "🇯🇵" :
           language === "ko" ? "🇰🇷" :
           language === "ar" ? "🇸🇦" :
           language === "ru" ? "🇷🇺" :
           language === "hi" ? "🇮🇳" :
           language === "id" ? "🇮🇩" :
           language === "si" ? "🇱🇰" : "🌐"}
        </span>
        <span>{current.name}</span>
        <svg className={`w-3 h-3 transition-transform ${open ? "rotate-180" : ""}`} fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
        </svg>
      </button>

      {open && (
        <div className="absolute right-0 mt-1 w-52 bg-white border border-gray-200 rounded-xl shadow-lg z-50 max-h-72 overflow-y-auto">
          {availableLanguages.map((lang) => (
            <button
              key={lang.code}
              onClick={() => {
                setLanguage(lang.code);
                setOpen(false);
              }}
              className={`w-full text-left px-4 py-2.5 text-sm hover:bg-gray-50 transition-colors flex items-center gap-2 ${
                language === lang.code ? "bg-blue-50 text-blue-700 font-semibold" : "text-gray-900"
              }`}
            >
              <span className="text-base">
                {lang.code === "en" ? "🇬🇧" :
                 lang.code === "es" ? "🇪🇸" :
                 lang.code === "fr" ? "🇫🇷" :
                 lang.code === "pt" ? "🇵🇹" :
                 lang.code === "de" ? "🇩🇪" :
                 lang.code === "zh" ? "🇨🇳" :
                 lang.code === "ja" ? "🇯🇵" :
                 lang.code === "ko" ? "🇰🇷" :
                 lang.code === "ar" ? "🇸🇦" :
                 lang.code === "ru" ? "🇷🇺" :
                 lang.code === "hi" ? "🇮🇳" :
                 lang.code === "id" ? "🇮🇩" :
                 lang.code === "si" ? "🇱🇰" : "🌐"}
              </span>
              {lang.name}
            </button>
          ))}
        </div>
      )}
    </div>
  );
}
