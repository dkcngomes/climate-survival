namespace ClimateAdvisor.Api.Models;

/// <summary>Hydrometeorological measurements from short-term forecasts</summary>
public class HydroMeteoData
{
    // ── Soil Moisture (volumetric water content, 0-1 scale) ──
    public double? SoilMoisture0To1cm { get; set; }
    public double? SoilMoisture1To3cm { get; set; }
    public double? SoilMoisture3To9cm { get; set; }
    public double? SoilMoisture9To27cm { get; set; }
    
    /// Mean of top layers (0-9cm), 0-1 scale. < 0.15 = severe drought
    public double? MeanSoilMoisture { get; set; }
    public bool IsDroughtCondition { get; set; }
    
    // ── Precipitation ──
    public double? PrecipitationIntensityMm { get; set; } // max hourly precipitation mm/h
    public double? DailyPrecipitationSum { get; set; }    // total over forecast horizon, mm
    public bool IsExtremePrecipitation { get; set; }
    
    // ── Wind ──
    public double? MaxWindGustKmh { get; set; }
    public bool IsStormWind { get; set; }
    
    // ── Evapotranspiration ──
    public double? EvapotranspirationSum { get; set; }       // mm over forecast horizon
    public double? ReferenceEvapotranspirationSum { get; set; } // ET₀ mm
    
    // ── Flood ──
    public double? RiverDischargeMax { get; set; }   // m³/s
    public double? RiverDischargeMean { get; set; }  // m³/s
    public bool IsFloodRisk { get; set; }
    
    // ── Composite Indices (0-100) ──
    public double? DroughtSeverityIndex { get; set; }
    public double? FloodRiskIndex { get; set; }
    public double? StormSeverityIndex { get; set; }
}
