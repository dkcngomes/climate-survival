using ClimateAdvisor.Api.Models;

namespace ClimateAdvisor.Api.Services;

public interface ILocationService
{
    Task<List<CountryInfo>> GetCountriesAsync();
    Task<string?> DetectCountryFromIpAsync(string ipAddress, CancellationToken ct = default);
}
