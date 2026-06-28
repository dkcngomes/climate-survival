using System.Text.Json.Serialization;
using ClimateAdvisor.Api.Models;

namespace ClimateAdvisor.Api.Services;

public class HydroMeteoService : IHydroMeteoService
{
    private readonly HttpClient _http;

    public HydroMeteoService(HttpClient http)
    {
        _http = http;
    }

    public async Task<HydroMeteoData> GetHydroMeteoDataAsync(double latitude, double longitude, CancellationToken ct = default)
    {
        var data = new HydroMeteoData();

        await Task.WhenAll(
            FetchWeatherDataAsync(data, latitude, longitude, ct),
            FetchFloodDataAsync(data, latitude, longitude, ct)
        );

        ComputeIndices(data);

        return data;
    }

    private async Task FetchWeatherDataAsync(HydroMeteoData data, double lat, double lng, CancellationToken ct)
    {
        try
        {
            // Open-Meteo Weather Forecast API — hourly hydrometeorological variables
            var url = $"https://api.open-meteo.com/v1/forecast?" +
                      $"latitude={lat}&longitude={lng}" +
                      $"&hourly=soil_moisture_0_to_1cm,soil_moisture_1_to_3cm,soil_moisture_3_to_9cm,soil_moisture_9_to_27cm," +
                      $"precipitation,wind_gusts_10m,evapotranspiration,relative_humidity_2m" +
                      $"&daily=precipitation_sum,precipitation_hours,wind_speed_10m_max,wind_gusts_10m_max," +
                      $"et0_fao_evapotranspiration,shortwave_radiation_sum" +
                      $"&timezone=auto&forecast_days=3";

            var response = await _http.GetFromJsonAsync<WeatherApiResponse>(url, ct);
            if (response == null) return;

            // ── Hourly data: average/average across next 72 hours ──
            var hourly = response.Hourly;
            if (hourly != null)
            {
                var sm0 = hourly.SoilMoisture0To1?.Where(v => v.HasValue).Select(v => v!.Value).ToList();
                var sm1 = hourly.SoilMoisture1To3?.Where(v => v.HasValue).Select(v => v!.Value).ToList();
                var sm3 = hourly.SoilMoisture3To9?.Where(v => v.HasValue).Select(v => v!.Value).ToList();
                var sm9 = hourly.SoilMoisture9To27?.Where(v => v.HasValue).Select(v => v!.Value).ToList();
                var precip = hourly.Precipitation?.Where(v => v.HasValue).Select(v => v!.Value).ToList();
                var windGusts = hourly.WindGusts10m?.Where(v => v.HasValue).Select(v => v!.Value).ToList();
                var evapo = hourly.Evapotranspiration?.Where(v => v.HasValue).Select(v => v!.Value).ToList();

                if (sm0?.Count > 0) data.SoilMoisture0To1cm = Math.Round(sm0.Average(), 3);
                if (sm1?.Count > 0) data.SoilMoisture1To3cm = Math.Round(sm1.Average(), 3);
                if (sm3?.Count > 0) data.SoilMoisture3To9cm = Math.Round(sm3.Average(), 3);
                if (sm9?.Count > 0) data.SoilMoisture9To27cm = Math.Round(sm9.Average(), 3);

                // Mean soil moisture of top layers (0-9cm)
                var tops = new[] { sm0, sm1, sm3 }.Where(x => x?.Count > 0);
                if (tops.Any())
                    data.MeanSoilMoisture = Math.Round(tops.Average(x => x!.Average()), 3);

                // Max precipitation intensity (hourly)
                data.PrecipitationIntensityMm = precip?.Count > 0
                    ? Math.Round(precip.Max(), 1) : null;

                // Max wind gust
                data.MaxWindGustKmh = windGusts?.Count > 0
                    ? Math.Round(windGusts.Max(), 1) : null;

                // Total evapotranspiration
                data.EvapotranspirationSum = evapo?.Count > 0
                    ? Math.Round(evapo.Sum(), 2) : null;
            }

            // ── Daily data ──
            var daily = response.Daily;
            if (daily != null)
            {
                var precipSums = daily.PrecipitationSum?.Where(v => v.HasValue).Select(v => v!.Value).ToList();
                data.DailyPrecipitationSum = precipSums?.Count > 0
                    ? Math.Round(precipSums.Sum(), 1) : null;

                var refEt = daily.ReferenceEvapotranspirationSum?.Where(v => v.HasValue).Select(v => v!.Value).ToList();
                data.ReferenceEvapotranspirationSum = refEt?.Count > 0
                    ? Math.Round(refEt.Sum(), 2) : null;
            }

            // ── Classifications ──
            data.IsDroughtCondition = data.MeanSoilMoisture < 0.15;
            data.IsExtremePrecipitation = data.PrecipitationIntensityMm > 20 
                || (data.DailyPrecipitationSum > 100);

            data.IsStormWind = data.MaxWindGustKmh > 80; // > 80 km/h = storm force
        }
        catch
        {
            // Silently fail — data will remain null/defaults
        }
    }

    private async Task FetchFloodDataAsync(HydroMeteoData data, double lat, double lng, CancellationToken ct)
    {
        try
        {
            // Open-Meteo Flood API — river discharge
            var url = $"https://flood-api.open-meteo.com/v1/flood?" +
                      $"latitude={lat}&longitude={lng}" +
                      $"&daily=river_discharge_max,river_discharge_mean&forecast_days=7";

            var response = await _http.GetFromJsonAsync<FloodApiResponse>(url, ct);
            if (response?.Daily == null) return;

            var maxDischarge = response.Daily.RiverDischargeMax?.Where(v => v.HasValue).Select(v => v!.Value).ToList();
            var meanDischarge = response.Daily.RiverDischargeMean?.Where(v => v.HasValue).Select(v => v!.Value).ToList();

            if (maxDischarge?.Count > 0)
                data.RiverDischargeMax = Math.Round(maxDischarge.Max(), 2);

            if (meanDischarge?.Count > 0)
                data.RiverDischargeMean = Math.Round(meanDischarge.Average(), 2);

            // Flood risk: check if discharge exceeds typical thresholds
            // For most rivers, discharge > 500 m³/s indicates potential flooding
            // This is a simplification — real flood thresholds vary by river
            data.IsFloodRisk = data.RiverDischargeMax > 500;
        }
        catch
        {
            // Silently fail
        }
    }

    private void ComputeIndices(HydroMeteoData data)
    {
        // ── Drought Severity Index (0-100) ──
        var droughtScore = 0.0;

        // Soil moisture contribution (weight: 0.5)
        if (data.MeanSoilMoisture.HasValue)
        {
            // Soil moisture < 0.15 = severe, < 0.2 = moderate, < 0.25 = mild
            var sm = data.MeanSoilMoisture.Value;
            if (sm < 0.12) droughtScore += 45;
            else if (sm < 0.18) droughtScore += 30;
            else if (sm < 0.25) droughtScore += 15;
        }

        // Evapotranspiration contribution (weight: 0.3)
        if (data.ReferenceEvapotranspirationSum.HasValue && data.ReferenceEvapotranspirationSum > 8)
        {
            droughtScore += Math.Min(30, (data.ReferenceEvapotranspirationSum.Value - 8) * 3);
        }

        // Precipitation deficit (weight: 0.2)
        if (data.DailyPrecipitationSum.HasValue && data.DailyPrecipitationSum < 1)
            droughtScore += 15;

        data.DroughtSeverityIndex = Math.Round(Math.Min(100, droughtScore), 1);

        // ── Flood Risk Index (0-100) ──
        var floodScore = 0.0;

        if (data.IsFloodRisk) floodScore += 40;
        if (data.RiverDischargeMax > 1000) floodScore += 20;
        if (data.IsExtremePrecipitation) floodScore += 30;
        if (data.DailyPrecipitationSum > 150) floodScore += 10;

        data.FloodRiskIndex = Math.Round(Math.Min(100, floodScore), 1);

        // ── Storm Severity Index (0-100) ──
        var stormScore = 0.0;

        if (data.IsStormWind) stormScore += 50;
        if (data.MaxWindGustKmh > 100) stormScore += 30;
        else if (data.MaxWindGustKmh > 60) stormScore += 15;
        if (data.IsExtremePrecipitation) stormScore += 20;

        data.StormSeverityIndex = Math.Round(Math.Min(100, stormScore), 1);
    }

    // ── API Response DTOs ──

    private record WeatherApiResponse(
        WeatherHourly? Hourly,
        WeatherDaily? Daily
    );

    private record WeatherHourly(
        [property: JsonPropertyName("soil_moisture_0_to_1cm")] double?[]? SoilMoisture0To1,
        [property: JsonPropertyName("soil_moisture_1_to_3cm")] double?[]? SoilMoisture1To3,
        [property: JsonPropertyName("soil_moisture_3_to_9cm")] double?[]? SoilMoisture3To9,
        [property: JsonPropertyName("soil_moisture_9_to_27cm")] double?[]? SoilMoisture9To27,
        double?[]? Precipitation,
        [property: JsonPropertyName("wind_gusts_10m")] double?[]? WindGusts10m,
        double?[]? Evapotranspiration,
        [property: JsonPropertyName("relative_humidity_2m")] double?[]? RelativeHumidity
    );

    private record WeatherDaily(
        [property: JsonPropertyName("precipitation_sum")] double?[]? PrecipitationSum,
        [property: JsonPropertyName("et0_fao_evapotranspiration")] double?[]? ReferenceEvapotranspirationSum
    );

    private record FloodApiResponse(FloodDaily? Daily);
    private record FloodDaily(
        [property: JsonPropertyName("river_discharge_max")] double?[]? RiverDischargeMax,
        [property: JsonPropertyName("river_discharge_mean")] double?[]? RiverDischargeMean
    );
}
