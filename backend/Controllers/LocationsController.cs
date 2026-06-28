using ClimateAdvisor.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClimateAdvisor.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LocationsController : ControllerBase
{
    private readonly ILocationService _locations;
    private readonly ICountryLocaleService _locales;

    public LocationsController(ILocationService locations, ICountryLocaleService locales)
    {
        _locations = locations;
        _locales = locales;
    }

    /// <summary>
    /// Get all countries with their center coordinates.
    /// </summary>
    [HttpGet("countries")]
    public async Task<IActionResult> GetCountries()
    {
        var countries = await _locations.GetCountriesAsync();
        return Ok(countries);
    }

    /// <summary>
    /// Detect user's country based on their IP address.
    /// </summary>
    [HttpGet("ip-country")]
    public async Task<IActionResult> GetIpCountry()
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";

        // If behind a proxy/load balancer, check X-Forwarded-For
        var forwarded = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwarded))
            ip = forwarded.Split(',')[0].Trim();

        var countryCode = await _locations.DetectCountryFromIpAsync(ip);
        var countries = await _locations.GetCountriesAsync();
        var country = countries.FirstOrDefault(c => c.Code == countryCode);

        if (country == null)
            return Ok(new { countryCode = (string?)null, country = (object?)null, locale = (object?)null });

        return Ok(new { countryCode = country.Code, country, locale = _locales.GetLocale(country.Code) });
    }

    /// <summary>
    /// Get locale info (language, currency) for a country code.
    /// </summary>
    [HttpGet("locale/{countryCode}")]
    public IActionResult GetLocale(string countryCode)
    {
        var locale = _locales.GetLocale(countryCode);
        return Ok(locale);
    }

    /// <summary>
    /// Get all supported languages.
    /// </summary>
    [HttpGet("languages")]
    public IActionResult GetLanguages()
    {
        var languages = CountryLocaleService.GetSupportedLanguages();
        return Ok(languages.Select(l => new { code = l.Code, name = l.Name }));
    }
}
