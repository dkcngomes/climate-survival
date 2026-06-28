namespace ClimateAdvisor.Api.Models;

/// <summary>Recommendation for what to plant/grow based on forecast conditions</summary>
public class CropRecommendation
{
    public string CropName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // Leafy Greens, Root Veg, Fruits, Grains, Herbs, Legumes
    public string Description { get; set; } = string.Empty;
    public int DaysToHarvestMin { get; set; }
    public int DaysToHarvestMax { get; set; }
    
    /// <summary>Date if planted this week (current date)</summary>
    public DateTime PlantByDate { get; set; }
    
    /// <summary>Expected harvest window start</summary>
    public DateTime HarvestStartDate { get; set; }
    
    /// <summary>Expected harvest window end</summary>
    public DateTime HarvestEndDate { get; set; }
    
    /// <summary>Heat tolerance: Low, Medium, High</summary>
    public string HeatTolerance { get; set; } = "Medium";
    
    /// <summary>Cold tolerance: Low, Medium, High</summary>
    public string ColdTolerance { get; set; } = "Medium";
    
    /// <summary>Drought tolerance: Low, Medium, High</summary>
    public string DroughtTolerance { get; set; } = "Medium";
    
    /// <summary>Flood tolerance: Low, Medium, High</summary>
    public string FloodTolerance { get; set; } = "Low";
    
    /// <summary>Overall suitability score 0-100 given the forecast</summary>
    public int SuitabilityScore { get; set; }
    
    /// <summary>Why this crop is recommended for the coming conditions</summary>
    public string RecommendationReason { get; set; } = string.Empty;
    
    /// <summary>Quick growing tip specific to the forecast</summary>
    public string GrowingTip { get; set; } = string.Empty;
    
    /// <summary>Best planting method for the conditions</summary>
    public string PlantingMethod { get; set; } = string.Empty;
    
    /// <summary>URL to a thumbnail image of the crop (from Wikimedia)</summary>
    public string? ImageUrl { get; set; }
    
    /// <summary>URL to the Wikipedia article for the crop</summary>
    public string? WikiUrl { get; set; }
}

public class CropRecommendationResponse
{
    public List<CropRecommendation> Crops { get; set; } = new();
    public int TotalCrops { get; set; }
    public string GeneralAdvice { get; set; } = string.Empty;
}
