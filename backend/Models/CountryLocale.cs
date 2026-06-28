namespace ClimateAdvisor.Api.Models;

public class CountryLocale
{
    public string CountryCode { get; set; } = string.Empty;
    public string LanguageCode { get; set; } = "en";
    public string LanguageName { get; set; } = "English";
    public string CurrencyCode { get; set; } = "USD";
    public string CurrencySymbol { get; set; } = "$";
    public string Locale { get; set; } = "en-US";
}
