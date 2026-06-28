using ClimateAdvisor.Api.Models;
using ClimateAdvisor.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClimateAdvisor.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CropsController : ControllerBase
{
    private readonly IClimateService _climate;
    private readonly ICropRecommendationService _crops;
    private readonly ILogger<CropsController> _logger;

    public CropsController(IClimateService climate, ICropRecommendationService crops, ILogger<CropsController> logger)
    {
        _climate = climate;
        _crops = crops;
        _logger = logger;
    }

    /// <summary>GET /api/crops?lat=X&lng=X&countryCode=US — crop recommendations for the given location</summary>
    [HttpGet]
    public async Task<IActionResult> GetCrops([FromQuery] double lat, [FromQuery] double lng, [FromQuery] string? countryCode = null, CancellationToken ct = default)
    {
        if (lat < -90 || lat > 90 || lng < -180 || lng > 180)
            return BadRequest(new { error = "Invalid coordinates" });

        try
        {
            var forecast = await _climate.GetForecastAsync(lat, lng, ct);
            var result = await _crops.GetCropRecommendationsAsync(forecast, countryCode, ct);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting crop recommendations for {Lat},{Lng}", lat, lng);
            return StatusCode(500, new { error = "Failed to get crop recommendations" });
        }
    }
}
