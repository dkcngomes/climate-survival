using ClimateAdvisor.Api.Models;

namespace ClimateAdvisor.Api.Services;

public interface ICountryLocaleService
{
    CountryLocale GetLocale(string countryCode);
    List<CountryLocale> GetAllLocales();
}

public class CountryLocaleService : ICountryLocaleService
{
    private static readonly Dictionary<string, CountryLocale> LocaleMap = new()
    {
        // ── Americas ──
        ["US"] = new() { CountryCode = "US", LanguageCode = "en", LanguageName = "English", CurrencyCode = "USD", CurrencySymbol = "$", Locale = "en-US" },
        ["CA"] = new() { CountryCode = "CA", LanguageCode = "en", LanguageName = "English", CurrencyCode = "CAD", CurrencySymbol = "CA$", Locale = "en-CA" },
        ["MX"] = new() { CountryCode = "MX", LanguageCode = "es", LanguageName = "Español", CurrencyCode = "MXN", CurrencySymbol = "MX$", Locale = "es-MX" },
        ["BR"] = new() { CountryCode = "BR", LanguageCode = "pt", LanguageName = "Português", CurrencyCode = "BRL", CurrencySymbol = "R$", Locale = "pt-BR" },
        ["AR"] = new() { CountryCode = "AR", LanguageCode = "es", LanguageName = "Español", CurrencyCode = "ARS", CurrencySymbol = "AR$", Locale = "es-AR" },
        ["CO"] = new() { CountryCode = "CO", LanguageCode = "es", LanguageName = "Español", CurrencyCode = "COP", CurrencySymbol = "COL$", Locale = "es-CO" },
        ["CL"] = new() { CountryCode = "CL", LanguageCode = "es", LanguageName = "Español", CurrencyCode = "CLP", CurrencySymbol = "CL$", Locale = "es-CL" },
        ["PE"] = new() { CountryCode = "PE", LanguageCode = "es", LanguageName = "Español", CurrencyCode = "PEN", CurrencySymbol = "S/", Locale = "es-PE" },

        // ── Europe ──
        ["GB"] = new() { CountryCode = "GB", LanguageCode = "en", LanguageName = "English", CurrencyCode = "GBP", CurrencySymbol = "£", Locale = "en-GB" },
        ["DE"] = new() { CountryCode = "DE", LanguageCode = "de", LanguageName = "Deutsch", CurrencyCode = "EUR", CurrencySymbol = "€", Locale = "de-DE" },
        ["FR"] = new() { CountryCode = "FR", LanguageCode = "fr", LanguageName = "Français", CurrencyCode = "EUR", CurrencySymbol = "€", Locale = "fr-FR" },
        ["IT"] = new() { CountryCode = "IT", LanguageCode = "it", LanguageName = "Italiano", CurrencyCode = "EUR", CurrencySymbol = "€", Locale = "it-IT" },
        ["ES"] = new() { CountryCode = "ES", LanguageCode = "es", LanguageName = "Español", CurrencyCode = "EUR", CurrencySymbol = "€", Locale = "es-ES" },
        ["PT"] = new() { CountryCode = "PT", LanguageCode = "pt", LanguageName = "Português", CurrencyCode = "EUR", CurrencySymbol = "€", Locale = "pt-PT" },
        ["NL"] = new() { CountryCode = "NL", LanguageCode = "nl", LanguageName = "Nederlands", CurrencyCode = "EUR", CurrencySymbol = "€", Locale = "nl-NL" },
        ["BE"] = new() { CountryCode = "BE", LanguageCode = "nl", LanguageName = "Nederlands", CurrencyCode = "EUR", CurrencySymbol = "€", Locale = "nl-BE" },
        ["CH"] = new() { CountryCode = "CH", LanguageCode = "de", LanguageName = "Deutsch", CurrencyCode = "CHF", CurrencySymbol = "CHF", Locale = "de-CH" },
        ["SE"] = new() { CountryCode = "SE", LanguageCode = "sv", LanguageName = "Svenska", CurrencyCode = "SEK", CurrencySymbol = "kr", Locale = "sv-SE" },
        ["NO"] = new() { CountryCode = "NO", LanguageCode = "nb", LanguageName = "Norsk", CurrencyCode = "NOK", CurrencySymbol = "kr", Locale = "nb-NO" },
        ["DK"] = new() { CountryCode = "DK", LanguageCode = "da", LanguageName = "Dansk", CurrencyCode = "DKK", CurrencySymbol = "kr", Locale = "da-DK" },
        ["FI"] = new() { CountryCode = "FI", LanguageCode = "fi", LanguageName = "Suomi", CurrencyCode = "EUR", CurrencySymbol = "€", Locale = "fi-FI" },
        ["PL"] = new() { CountryCode = "PL", LanguageCode = "pl", LanguageName = "Polski", CurrencyCode = "PLN", CurrencySymbol = "zł", Locale = "pl-PL" },
        ["RU"] = new() { CountryCode = "RU", LanguageCode = "ru", LanguageName = "Русский", CurrencyCode = "RUB", CurrencySymbol = "₽", Locale = "ru-RU" },
        ["UA"] = new() { CountryCode = "UA", LanguageCode = "uk", LanguageName = "Українська", CurrencyCode = "UAH", CurrencySymbol = "₴", Locale = "uk-UA" },
        ["TR"] = new() { CountryCode = "TR", LanguageCode = "tr", LanguageName = "Türkçe", CurrencyCode = "TRY", CurrencySymbol = "₺", Locale = "tr-TR" },

        // ── Asia ──
        ["CN"] = new() { CountryCode = "CN", LanguageCode = "zh", LanguageName = "中文", CurrencyCode = "CNY", CurrencySymbol = "¥", Locale = "zh-CN" },
        ["JP"] = new() { CountryCode = "JP", LanguageCode = "ja", LanguageName = "日本語", CurrencyCode = "JPY", CurrencySymbol = "¥", Locale = "ja-JP" },
        ["IN"] = new() { CountryCode = "IN", LanguageCode = "hi", LanguageName = "हिन्दी", CurrencyCode = "INR", CurrencySymbol = "₹", Locale = "hi-IN" },
        ["KR"] = new() { CountryCode = "KR", LanguageCode = "ko", LanguageName = "한국어", CurrencyCode = "KRW", CurrencySymbol = "₩", Locale = "ko-KR" },
        ["ID"] = new() { CountryCode = "ID", LanguageCode = "id", LanguageName = "Bahasa Indonesia", CurrencyCode = "IDR", CurrencySymbol = "Rp", Locale = "id-ID" },
        ["MY"] = new() { CountryCode = "MY", LanguageCode = "ms", LanguageName = "Bahasa Melayu", CurrencyCode = "MYR", CurrencySymbol = "RM", Locale = "ms-MY" },
        ["TH"] = new() { CountryCode = "TH", LanguageCode = "th", LanguageName = "ไทย", CurrencyCode = "THB", CurrencySymbol = "฿", Locale = "th-TH" },
        ["VN"] = new() { CountryCode = "VN", LanguageCode = "vi", LanguageName = "Tiếng Việt", CurrencyCode = "VND", CurrencySymbol = "₫", Locale = "vi-VN" },
        ["PH"] = new() { CountryCode = "PH", LanguageCode = "tl", LanguageName = "Filipino", CurrencyCode = "PHP", CurrencySymbol = "₱", Locale = "tl-PH" },
        ["SG"] = new() { CountryCode = "SG", LanguageCode = "en", LanguageName = "English", CurrencyCode = "SGD", CurrencySymbol = "S$", Locale = "en-SG" },
        ["HK"] = new() { CountryCode = "HK", LanguageCode = "zh", LanguageName = "中文", CurrencyCode = "HKD", CurrencySymbol = "HK$", Locale = "zh-HK" },
        ["TW"] = new() { CountryCode = "TW", LanguageCode = "zh", LanguageName = "中文", CurrencyCode = "TWD", CurrencySymbol = "NT$", Locale = "zh-TW" },
        ["AE"] = new() { CountryCode = "AE", LanguageCode = "ar", LanguageName = "العربية", CurrencyCode = "AED", CurrencySymbol = "د.إ", Locale = "ar-AE" },
        ["SA"] = new() { CountryCode = "SA", LanguageCode = "ar", LanguageName = "العربية", CurrencyCode = "SAR", CurrencySymbol = "﷼", Locale = "ar-SA" },
        ["IL"] = new() { CountryCode = "IL", LanguageCode = "he", LanguageName = "עברית", CurrencyCode = "ILS", CurrencySymbol = "₪", Locale = "he-IL" },
        ["PK"] = new() { CountryCode = "PK", LanguageCode = "ur", LanguageName = "اردو", CurrencyCode = "PKR", CurrencySymbol = "₨", Locale = "ur-PK" },
        ["BD"] = new() { CountryCode = "BD", LanguageCode = "bn", LanguageName = "বাংলা", CurrencyCode = "BDT", CurrencySymbol = "৳", Locale = "bn-BD" },
        ["LK"] = new() { CountryCode = "LK", LanguageCode = "si", LanguageName = "සිංහල", CurrencyCode = "LKR", CurrencySymbol = "Rs", Locale = "si-LK" },
        ["NP"] = new() { CountryCode = "NP", LanguageCode = "ne", LanguageName = "नेपाली", CurrencyCode = "NPR", CurrencySymbol = "Rs", Locale = "ne-NP" },

        // ── Africa ──
        ["ZA"] = new() { CountryCode = "ZA", LanguageCode = "af", LanguageName = "Afrikaans", CurrencyCode = "ZAR", CurrencySymbol = "R", Locale = "af-ZA" },
        ["NG"] = new() { CountryCode = "NG", LanguageCode = "en", LanguageName = "English", CurrencyCode = "NGN", CurrencySymbol = "₦", Locale = "en-NG" },
        ["KE"] = new() { CountryCode = "KE", LanguageCode = "sw", LanguageName = "Kiswahili", CurrencyCode = "KES", CurrencySymbol = "KSh", Locale = "sw-KE" },
        ["EG"] = new() { CountryCode = "EG", LanguageCode = "ar", LanguageName = "العربية", CurrencyCode = "EGP", CurrencySymbol = "E£", Locale = "ar-EG" },
        ["MA"] = new() { CountryCode = "MA", LanguageCode = "ar", LanguageName = "العربية", CurrencyCode = "MAD", CurrencySymbol = "د.م.", Locale = "ar-MA" },
        ["GH"] = new() { CountryCode = "GH", LanguageCode = "en", LanguageName = "English", CurrencyCode = "GHS", CurrencySymbol = "₵", Locale = "en-GH" },
        ["ET"] = new() { CountryCode = "ET", LanguageCode = "am", LanguageName = "አማርኛ", CurrencyCode = "ETB", CurrencySymbol = "Br", Locale = "am-ET" },
        ["TZ"] = new() { CountryCode = "TZ", LanguageCode = "sw", LanguageName = "Kiswahili", CurrencyCode = "TZS", CurrencySymbol = "TSh", Locale = "sw-TZ" },
        ["UG"] = new() { CountryCode = "UG", LanguageCode = "sw", LanguageName = "Kiswahili", CurrencyCode = "UGX", CurrencySymbol = "USh", Locale = "sw-UG" },

        // ── Oceania ──
        ["AU"] = new() { CountryCode = "AU", LanguageCode = "en", LanguageName = "English", CurrencyCode = "AUD", CurrencySymbol = "A$", Locale = "en-AU" },
        ["NZ"] = new() { CountryCode = "NZ", LanguageCode = "en", LanguageName = "English", CurrencyCode = "NZD", CurrencySymbol = "NZ$", Locale = "en-NZ" },

        // ── Default fallback ──
        ["*"] = new() { CountryCode = "*", LanguageCode = "en", LanguageName = "English", CurrencyCode = "USD", CurrencySymbol = "$", Locale = "en-US" },
    };

    private static readonly List<string> SupportedLanguages = new()
    {
        "en", "es", "fr", "pt", "de", "it", "nl", "zh", "ja", "ko",
        "hi", "id", "ms", "th", "vi", "tl", "ar", "he", "tr", "ru",
        "uk", "pl", "sv", "nb", "da", "fi", "cs", "hu", "ro", "el",
        "bn", "ur", "ne", "si", "am", "sw", "af"
    };

    public CountryLocale GetLocale(string countryCode)
    {
        return LocaleMap.TryGetValue(countryCode.ToUpperInvariant(), out var locale)
            ? locale
            : LocaleMap["*"];
    }

    public List<CountryLocale> GetAllLocales()
    {
        return LocaleMap.Values
            .Where(l => l.CountryCode != "*")
            .OrderBy(l => l.CountryCode)
            .ToList();
    }

    public static List<(string Code, string Name)> GetSupportedLanguages()
    {
        var all = new List<(string, string)>
        {
            ("en", "English"),
            ("es", "Español"),
            ("fr", "Français"),
            ("pt", "Português"),
            ("de", "Deutsch"),
            ("it", "Italiano"),
            ("nl", "Nederlands"),
            ("ru", "Русский"),
            ("zh", "中文"),
            ("ja", "日本語"),
            ("ko", "한국어"),
            ("hi", "हिन्दी"),
            ("id", "Bahasa Indonesia"),
            ("ms", "Bahasa Melayu"),
            ("th", "ไทย"),
            ("vi", "Tiếng Việt"),
            ("tl", "Filipino"),
            ("ar", "العربية"),
            ("he", "עברית"),
            ("tr", "Türkçe"),
            ("uk", "Українська"),
            ("pl", "Polski"),
            ("sv", "Svenska"),
            ("nb", "Norsk"),
            ("da", "Dansk"),
            ("fi", "Suomi"),
            ("bn", "বাংলা"),
            ("ur", "اردو"),
            ("ne", "नेपाली"),
            ("si", "සිංහල"),
            ("am", "አማርኛ"),
            ("sw", "Kiswahili"),
            ("af", "Afrikaans"),
        };
        return all;
    }
}
