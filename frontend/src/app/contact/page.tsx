"use client";

import { useState } from "react";
import Link from "next/link";
import { LocalizationProvider, useLocalization } from "@/i18n/LocalizationContext";
import LanguageSwitcher from "@/i18n/LanguageSwitcher";
import { submitContact, ContactPayload } from "@/services/api";

function ContactContent() {
  const { t } = useLocalization();
  const [form, setForm] = useState<ContactPayload>({
    name: "",
    email: "",
    subject: "",
    message: "",
  });
  const [submitting, setSubmitting] = useState(false);
  const [success, setSuccess] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
    setForm((prev) => ({ ...prev, [e.target.name]: e.target.value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitting(true);
    setError(null);
    setSuccess(false);
    try {
      await submitContact(form);
      setSuccess(true);
      setForm({ name: "", email: "", subject: "", message: "" });
    } catch (err) {
      setError(err instanceof Error ? err.message : t("contact.errorGeneric"));
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-emerald-50 via-white to-blue-50 flex flex-col">
      {/* Header */}
      <header className="sticky top-0 z-30 bg-white/80 backdrop-blur-sm border-b border-gray-200">
        <div className="max-w-5xl mx-auto px-4 h-14 flex items-center justify-between">
          <Link href="/" className="flex items-center gap-3 hover:opacity-80 transition-opacity">
            <span className="text-2xl">🌍</span>
            <div>
              <h1 className="text-sm font-bold text-gray-900">{t("app.title")}</h1>
            </div>
          </Link>
          <div className="flex items-center gap-3">
            <LanguageSwitcher />
            <Link
              href="/"
              className="text-xs px-3 py-1.5 bg-gray-100 hover:bg-gray-200 rounded-lg text-gray-800 transition-colors border border-gray-200"
            >
              🏠 {t("contact.backToHome")}
            </Link>
          </div>
        </div>
      </header>

      {/* Contact Form */}
      <main className="flex-1 max-w-2xl mx-auto px-4 py-12 w-full">
        <div className="text-center mb-10">
          <h2 className="text-3xl font-extrabold text-gray-900 mb-3">
            📬 {t("contact.title")}
          </h2>
          <p className="text-gray-700">
            {t("contact.description")}
          </p>
        </div>

        {success ? (
          <div className="bg-green-50 border border-green-200 rounded-2xl p-8 text-center">
            <p className="text-5xl mb-4">✅</p>
            <h3 className="text-xl font-bold text-green-800 mb-2">{t("contact.thankYou")}</h3>
            <p className="text-green-700 mb-6">{t("contact.successMessage")}</p>
            <Link
              href="/"
              className="inline-block px-6 py-2.5 bg-green-600 hover:bg-green-700 text-white font-semibold rounded-xl transition-colors"
            >
              {t("contact.backToHome")}
            </Link>
          </div>
        ) : (
          <form onSubmit={handleSubmit} className="space-y-5">
            {/* Name */}
            <div>
              <label className="block text-sm font-semibold text-gray-800 mb-1.5">
                {t("contact.name")} <span className="text-red-500">*</span>
              </label>
              <input
                type="text"
                name="name"
                value={form.name}
                onChange={handleChange}
                required
                maxLength={200}
                placeholder={t("contact.namePlaceholder")}
                className="w-full px-4 py-2.5 border border-gray-300 rounded-xl text-gray-900 focus:ring-2 focus:ring-emerald-400 focus:border-emerald-400 outline-none transition-shadow text-sm"
              />
            </div>

            {/* Email */}
            <div>
              <label className="block text-sm font-semibold text-gray-800 mb-1.5">
                {t("contact.email")} <span className="text-red-500">*</span>
              </label>
              <input
                type="email"
                name="email"
                value={form.email}
                onChange={handleChange}
                required
                maxLength={320}
                placeholder={t("contact.emailPlaceholder")}
                className="w-full px-4 py-2.5 border border-gray-300 rounded-xl text-gray-900 focus:ring-2 focus:ring-emerald-400 focus:border-emerald-400 outline-none transition-shadow text-sm"
              />
            </div>

            {/* Subject */}
            <div>
              <label className="block text-sm font-semibold text-gray-800 mb-1.5">
                {t("contact.subject")} <span className="text-red-500">*</span>
              </label>
              <select
                name="subject"
                value={form.subject}
                onChange={handleChange}
                required
                className="w-full px-4 py-2.5 border border-gray-300 rounded-xl text-gray-900 focus:ring-2 focus:ring-emerald-400 focus:border-emerald-400 outline-none transition-shadow text-sm bg-white"
              >
                <option value="">{t("contact.subjectPlaceholder")}</option>
                <option value="Bug Report">{t("contact.subjectBug")}</option>
                <option value="Feature Request">{t("contact.subjectFeature")}</option>
                <option value="Data Question">{t("contact.subjectData")}</option>
                <option value="Partnership">{t("contact.subjectPartner")}</option>
                <option value="Other">{t("contact.subjectOther")}</option>
              </select>
            </div>

            {/* Message */}
            <div>
              <label className="block text-sm font-semibold text-gray-800 mb-1.5">
                {t("contact.message")} <span className="text-red-500">*</span>
              </label>
              <textarea
                name="message"
                value={form.message}
                onChange={handleChange}
                required
                maxLength={5000}
                rows={6}
                placeholder={t("contact.messagePlaceholder")}
                className="w-full px-4 py-2.5 border border-gray-300 rounded-xl text-gray-900 focus:ring-2 focus:ring-emerald-400 focus:border-emerald-400 outline-none transition-shadow text-sm resize-y"
              />
              <p className="text-xs text-gray-500 mt-1">{form.message.length}/5000</p>
            </div>

            {error && (
              <div className="bg-red-50 border border-red-200 rounded-xl p-3 text-sm text-red-700">
                ❌ {error}
              </div>
            )}

            <button
              type="submit"
              disabled={submitting}
              className="w-full py-3 bg-emerald-600 hover:bg-emerald-700 disabled:bg-gray-300 disabled:text-gray-500 text-white font-semibold rounded-xl transition-colors flex items-center justify-center gap-2"
            >
              {submitting ? (
                <>
                  <span className="inline-block w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin" />
                  {t("contact.sending")}
                </>
              ) : (
                <>
                  📨 {t("contact.send")}
                </>
              )}
            </button>
          </form>
        )}
      </main>

      {/* Footer */}
      <footer className="py-6 border-t border-gray-200 bg-white/60">
        <div className="max-w-4xl mx-auto px-4 text-center text-sm text-gray-600">
          <p>
            {t("footer.developedBy")}{" "}
            <a
              href="https://nipuna.netlify.app/"
              target="_blank"
              rel="noopener noreferrer"
              className="font-semibold text-emerald-700 hover:text-emerald-500 underline decoration-emerald-300 transition-colors"
            >
              Nipuna Gomes
            </a>
          </p>
        </div>
      </footer>
    </div>
  );
}

export default function ContactPage() {
  return (
    <LocalizationProvider>
      <ContactContent />
    </LocalizationProvider>
  );
}
