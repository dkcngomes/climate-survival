using ClimateAdvisor.Api.Models;

namespace ClimateAdvisor.Api.Services;

public interface IClimateService
{
    Task<ClimateForecast> GetForecastAsync(double latitude, double longitude, CancellationToken ct = default);
}
