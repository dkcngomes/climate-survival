using ClimateAdvisor.Api.Models;

namespace ClimateAdvisor.Api.Services;

public class GeminiCropRerank
{
    public string CropName { get; set; } = string.Empty;
    public int SuitabilityScore { get; set; }
    public string RecommendationReason { get; set; } = string.Empty;
    public string GrowingTip { get; set; } = string.Empty;
    public string KeyFactor { get; set; } = string.Empty;
}

public class GeminiCropResponse
{
    public List<GeminiCropRerank> RerankedCrops { get; set; } = new();
    public string GeneralAdvice { get; set; } = string.Empty;
}

public interface IGeminiService
{
    /// <summary>
    /// Uses Gemini to rerank crop recommendations based on climate data.
    /// Returns null if Gemini is not configured, unavailable, or times out.
    /// </summary>
    Task<GeminiCropResponse?> RerankCropsAsync(
        ClimateForecast forecast,
        string? countryCode,
        string regionName,
        List<(string Name, int Score, string Reason, string Tip)> candidateCrops,
        CancellationToken ct = default);

    bool IsEnabled { get; }
}
