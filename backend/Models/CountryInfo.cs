namespace ClimateAdvisor.Api.Models;

public class CountryInfo
{
    public string Code { get; set; } = string.Empty;       // ISO 3166-1 alpha-2 (e.g. "LK")
    public string Name { get; set; } = string.Empty;       // "Sri Lanka"
    public string NameNative { get; set; } = string.Empty; // Native name (optional)
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Region { get; set; } = string.Empty;     // "Asia"
    public string SubRegion { get; set; } = string.Empty;  // "Southern Asia"
}
