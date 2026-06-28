namespace ClimateAdvisor.Api.Models;

public class ClimateForecast
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string LocationName { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    
    /// <summary>Temperature anomaly in °C (positive = warmer than normal)</summary>
    public double? TemperatureAnomaly { get; set; }
    
    /// <summary>Precipitation anomaly in mm</summary>
    public double? PrecipitationAnomaly { get; set; }
    
    /// <summary>Extreme Forecast Index for temperature (-1 to 1, >0.8 = extreme)</summary>
    public double? ExtremeTemperatureIndex { get; set; }
    
    /// <summary>Extreme Forecast Index for precipitation (-1 to 1, >0.8 = extreme)</summary>
    public double? ExtremePrecipitationIndex { get; set; }
    
    /// <summary>Forecast probability (0-100%)</summary>
    public int Probability { get; set; }
    
    /// <summary>Forecast valid start date</summary>
    public DateTime ForecastDate { get; set; }
    
    /// <summary>Human-readable label for forecast horizon (e.g. "7-Month Seasonal Outlook", "7-Day Weather Forecast")</summary>
    public string ForecastPeriodLabel { get; set; } = string.Empty;
    
    /// <summary>Detected climate signals</summary>
    public List<ClimateSignal> DetectedSignals { get; set; } = new();

    /// <summary>Hydrometeorological data (soil moisture, river discharge, etc.)</summary>
    public HydroMeteoData? HydroMeteo { get; set; }
}

public enum ClimateSignal
{
    ElNino,
    LaNina,
    Drought,
    HeavyRainfall,
    Heatwave,
    ColdSpell,
    FloodRisk,
    StormRisk,
    ExtremeDrought,
    Normal
}

public static class ClimateSignalExtensions
{
    public static string ToDisplayName(this ClimateSignal signal) => signal switch
    {
        ClimateSignal.ElNino => "El Niño",
        ClimateSignal.LaNina => "La Niña",
        ClimateSignal.Drought => "Drought",
        ClimateSignal.HeavyRainfall => "Heavy Rainfall",
        ClimateSignal.Heatwave => "Heatwave",
        ClimateSignal.ColdSpell => "Cold Spell",
        ClimateSignal.FloodRisk => "Flood Risk",
        ClimateSignal.StormRisk => "Storm Risk",
        ClimateSignal.ExtremeDrought => "Extreme Drought",
        ClimateSignal.Normal => "Normal Conditions",
        _ => "Unknown"
    };
    
    public static string GetDescription(this ClimateSignal signal) => signal switch
    {
        ClimateSignal.ElNino => "Warmer-than-average sea temperatures in Pacific → disrupted weather patterns, crop stress, food price inflation",
        ClimateSignal.LaNina => "Cooler-than-average sea temperatures in Pacific → increased rainfall, flooding risk, supply chain disruption",
        ClimateSignal.Drought => "Prolonged dry period → water scarcity, crop failure, livestock losses",
        ClimateSignal.HeavyRainfall => "Above-average precipitation → flooding, transportation disruption, crop damage",
        ClimateSignal.Heatwave => "Extreme high temperatures → heat stress on crops, increased cooling demand, energy price spikes",
        ClimateSignal.ColdSpell => "Abnormally cold weather → heating demand surge, crop frost damage, transportation delays",
        ClimateSignal.FloodRisk => "Elevated river discharge and soil saturation → imminent flood risk, supply chain disruption",
        ClimateSignal.StormRisk => "High wind gusts combined with heavy precipitation → property damage risk, power outages",
        ClimateSignal.ExtremeDrought => "Critically low soil moisture and high evapotranspiration → severe agricultural losses, water shortages",
        ClimateSignal.Normal => "Weather patterns within expected seasonal norms",
        _ => "Unknown"
    };
}
