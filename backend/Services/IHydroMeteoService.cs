using ClimateAdvisor.Api.Models;

namespace ClimateAdvisor.Api.Services;

public interface IHydroMeteoService
{
    Task<HydroMeteoData> GetHydroMeteoDataAsync(double latitude, double longitude, CancellationToken ct = default);
}
