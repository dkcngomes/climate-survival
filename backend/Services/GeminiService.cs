using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ClimateAdvisor.Api.Models;

namespace ClimateAdvisor.Api.Services;

public class GeminiService : IGeminiService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;
    private readonly ILogger<GeminiService> _logger;
    private readonly string? _apiKey;
    private readonly bool _enabled;
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
    };

    private const string GeminiEndpoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";

    public bool IsEnabled => _enabled;

    public GeminiService(HttpClient http, IConfiguration config, ILogger<GeminiService> logger)
    {
        _http = http;
        _config = config;
        _logger = logger;
        _apiKey = config["Gemini:ApiKey"];
        _enabled = !string.IsNullOrEmpty(_apiKey);
    }

    public async Task<GeminiCropResponse?> RerankCropsAsync(
        ClimateForecast forecast,
        string? countryCode,
        string regionName,
        List<(string Name, int Score, string Reason, string Tip)> candidateCrops,
        CancellationToken ct = default)
    {
        if (!_enabled) return null;

        try
        {
            var signals = string.Join(", ", forecast.DetectedSignals.Select(s => s.ToDisplayName()));
            var hydro = forecast.HydroMeteo;

            // Build a compact crop list for the prompt
            var cropLines = string.Join("\n", candidateCrops.Select(c =>
                $"  - {{c.Name}} (score: {{c.Score}}, reason: {{c.Reason}})"));

            var jsonInstruction = """
{"rerankedCrops":[{"cropName":"...","suitabilityScore":0-100,"recommendationReason":"...","growingTip":"...","keyFactor":"..."}], "generalAdvice":"..."}
""";

            var prompt = $"""
You are a climate-adaptive agriculture expert. Analyze the climate data below and rerank the candidate crops for the coming season.

CLIMATE DATA
Location: {forecast.LocationName}, {forecast.Region}
Region: {regionName}
Country Code: {countryCode ?? "unknown"}
Temperature Anomaly: {forecast.TemperatureAnomaly:F1}°C
Precipitation Anomaly: {forecast.PrecipitationAnomaly:F1}mm
Detected Signals: {signals}
Soil Moisture: {hydro?.MeanSoilMoisture * 100 ?? 0:F0}%
Drought Index: {hydro?.DroughtSeverityIndex ?? 0}/100
Flood Risk Index: {hydro?.FloodRiskIndex ?? 0}/100
Storm Risk Index: {hydro?.StormSeverityIndex ?? 0}/100
River Discharge: {hydro?.RiverDischargeMax ?? 0:F0} m³/s
Max Wind Gust: {hydro?.MaxWindGustKmh ?? 0:F0} km/h
Forecast Period: {forecast.ForecastPeriodLabel}

CANDIDATE CROPS (pre-scored by rule engine):
{cropLines}

TASK:
1. Rerank these crops for actual suitability given the climate data.
2. The rule engine scores are a starting point — adjust up/down based on your expertise.
3. Consider: the specific climate signals, soil moisture trends, flood/drought risks, and the location's agricultural region.
4. For each crop, provide:
   - suitabilityScore (0-100): your expert-adjusted score
   - recommendationReason: why this crop is or isn't suitable, specific to the climate data
   - growingTip: a practical planting tip tailored to these exact conditions
   - keyFactor: the single most important factor (e.g., "heat tolerance", "drought resistance", "cold hardiness", "flood tolerance", "fast harvest before storms")
5. Output a short generalAdvice string with season strategy.

Return ONLY valid JSON (no markdown, no code fences):
{jsonInstruction}
""";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                },
                generationConfig = new
                {
                    temperature = 0.3,
                    maxOutputTokens = 4096,
                    responseMimeType = "application/json"
                }
            };

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(10)); // timeout after 10s

            var response = await _http.PostAsJsonAsync(
                $"{GeminiEndpoint}?key={_apiKey}",
                requestBody,
                JsonOpts,
                cts.Token);

            response.EnsureSuccessStatusCode();

            var raw = await response.Content.ReadFromJsonAsync<GeminiRawResponse>(JsonOpts, cts.Token);
            if (raw?.Candidates == null || raw.Candidates.Count == 0)
            {
                _logger.LogWarning("Gemini returned no candidates");
                return null;
            }

            var text = raw.Candidates[0]?.Content?.Parts?[0]?.Text;
            if (string.IsNullOrEmpty(text))
            {
                _logger.LogWarning("Gemini returned empty text");
                return null;
            }

            // Parse the JSON response
            var result = JsonSerializer.Deserialize<GeminiCropResponse>(text, JsonOpts);
            if (result?.RerankedCrops == null || result.RerankedCrops.Count == 0)
            {
                _logger.LogWarning("Gemini returned unparseable crop list");
                return null;
            }

            _logger.LogInformation("Gemini reranked {Count} crops successfully", result.RerankedCrops.Count);
            return result;
        }
        catch (TaskCanceledException)
        {
            _logger.LogWarning("Gemini request timed out — falling back to rule engine");
            return null;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Gemini API request failed — falling back to rule engine");
            return null;
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Gemini response parse failed — falling back to rule engine");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Unexpected Gemini error — falling back to rule engine");
            return null;
        }
    }
}

// ── Gemini API raw response models ──
public class GeminiRawResponse
{
    [JsonPropertyName("candidates")]
    public List<GeminiCandidate>? Candidates { get; set; }
}

public class GeminiCandidate
{
    [JsonPropertyName("content")]
    public GeminiContent? Content { get; set; }
}

public class GeminiContent
{
    [JsonPropertyName("parts")]
    public List<GeminiPart>? Parts { get; set; }
}

public class GeminiPart
{
    [JsonPropertyName("text")]
    public string? Text { get; set; }
}
