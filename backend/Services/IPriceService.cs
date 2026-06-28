namespace ClimateAdvisor.Api.Services;

public interface IPriceService
{
    Task<List<ItemPrice>> GetPricesAsync(CancellationToken ct = default);
}

public class ItemPrice
{
    public string ItemName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal EstimatedPrice { get; set; }
    public string Currency { get; set; } = "USD";
    public string Unit { get; set; } = string.Empty;
    public string Source { get; set; } = "World Bank / Estimated";
    public DateTime LastUpdated { get; set; }
}
