using System.Text.Json.Serialization;

namespace ClimateAdvisor.Api.Services;

public class PriceService : IPriceService
{
    private readonly HttpClient _http;
    private List<ItemPrice>? _cachedPrices;
    private DateTime _lastFetch = DateTime.MinValue;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(6);

    public PriceService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<ItemPrice>> GetPricesAsync(CancellationToken ct = default)
    {
        if (_cachedPrices != null && DateTime.UtcNow - _lastFetch < CacheDuration)
            return _cachedPrices;

        var prices = await FetchFromWorldBankAsync(ct);
        _cachedPrices = prices;
        _lastFetch = DateTime.UtcNow;
        return prices;
    }

    private async Task<List<ItemPrice>> FetchFromWorldBankAsync(CancellationToken ct)
    {
        try
        {
            // World Bank API: commodity price data in JSON format
            var url = "https://api.worldbank.org/v2/commodity/PriceData?format=json";
            var response = await _http.GetFromJsonAsync<WorldBankResponse>(url, ct);

            if (response?.Data != null && response.Data.Count > 0)
            {
                return MapWorldBankData(response.Data);
            }
        }
        catch
        {
            // Fall through to seeded data
        }

        return GetSeededPrices();
    }

    private List<ItemPrice> MapWorldBankData(List<WorldBankCommodity> data)
    {
        var map = new Dictionary<string, (decimal price, string unit)>(StringComparer.OrdinalIgnoreCase);

        foreach (var item in data)
        {
            if (item.Value.HasValue && !string.IsNullOrEmpty(item.Commodity))
            {
                var key = item.Commodity.Trim();
                map[key] = (Math.Round((decimal)item.Value.Value, 2), item.Unit ?? "metric ton");
            }
        }

        // Map World Bank commodities to our items
        var results = new List<ItemPrice>();
        var mappings = new Dictionary<string, (string item, string category)>
        {
            ["Rice, Thailand, 5% broken"] = ("Rice", "Grains"),
            ["Wheat, US, HRW"] = ("Flour / Wheat", "Grains"),
            ["Corn"] = ("Corn", "Grains"),
            ["Soybean Oil"] = ("Cooking Oil", "Oils & Fats"),
            ["Soybeans"] = ("Soybeans", "Grains"),
            ["Sugar, EU"] = ("Sugar", "Food"),
            ["Beef"] = ("Beef / Meat", "Protein"),
            ["Chicken"] = ("Chicken / Poultry", "Protein"),
            ["Coffee, Arabica"] = ("Coffee", "Beverages"),
            ["Tea, Colombo"] = ("Tea", "Beverages"),
            ["Cocoa"] = ("Cocoa / Chocolate", "Food"),
            ["Urea, E. Europe"] = ("Fertilizers", "Agriculture")
        };

        foreach (var (wbKey, (ourItem, category)) in mappings)
        {
            if (map.TryGetValue(wbKey, out var wbPrice))
            {
                results.Add(new ItemPrice
                {
                    ItemName = ourItem,
                    Category = category,
                    EstimatedPrice = wbPrice.price,
                    Currency = "USD",
                    Unit = wbPrice.unit,
                    Source = "World Bank",
                    LastUpdated = DateTime.UtcNow
                });
            }
        }

        return results.Count > 0 ? results : GetSeededPrices();
    }

    private static List<ItemPrice> GetSeededPrices()
    {
        return new List<ItemPrice>
        {
            // Grains
            new() { ItemName = "Rice", Category = "Grains", EstimatedPrice = 0.85m, Currency = "USD", Unit = "kg", Source = "Estimated (World Bank baseline)", LastUpdated = DateTime.UtcNow },
            new() { ItemName = "Flour / Wheat", Category = "Grains", EstimatedPrice = 0.50m, Currency = "USD", Unit = "kg", Source = "Estimated (World Bank baseline)", LastUpdated = DateTime.UtcNow },
            new() { ItemName = "Corn", Category = "Grains", EstimatedPrice = 0.45m, Currency = "USD", Unit = "kg", Source = "Estimated (World Bank baseline)", LastUpdated = DateTime.UtcNow },

            // Canned & Preserved
            new() { ItemName = "Canned Food (Mixed)", Category = "Canned & Preserved", EstimatedPrice = 2.50m, Currency = "USD", Unit = "can", Source = "Estimated", LastUpdated = DateTime.UtcNow },
            new() { ItemName = "Canned Fish", Category = "Canned & Preserved", EstimatedPrice = 3.00m, Currency = "USD", Unit = "can", Source = "Estimated", LastUpdated = DateTime.UtcNow },
            new() { ItemName = "Powdered Milk", Category = "Canned & Preserved", EstimatedPrice = 12.00m, Currency = "USD", Unit = "kg", Source = "Estimated", LastUpdated = DateTime.UtcNow },

            // Oils & Fats
            new() { ItemName = "Cooking Oil", Category = "Oils & Fats", EstimatedPrice = 2.20m, Currency = "USD", Unit = "liter", Source = "Estimated (World Bank baseline)", LastUpdated = DateTime.UtcNow },

            // Protein
            new() { ItemName = "Beef / Meat", Category = "Protein", EstimatedPrice = 8.50m, Currency = "USD", Unit = "kg", Source = "Estimated (World Bank baseline)", LastUpdated = DateTime.UtcNow },
            new() { ItemName = "Chicken / Poultry", Category = "Protein", EstimatedPrice = 4.50m, Currency = "USD", Unit = "kg", Source = "Estimated (World Bank baseline)", LastUpdated = DateTime.UtcNow },
            new() { ItemName = "Eggs", Category = "Protein", EstimatedPrice = 3.00m, Currency = "USD", Unit = "dozen", Source = "Estimated", LastUpdated = DateTime.UtcNow },

            // Other Staples
            new() { ItemName = "Sugar", Category = "Food", EstimatedPrice = 1.20m, Currency = "USD", Unit = "kg", Source = "Estimated (World Bank baseline)", LastUpdated = DateTime.UtcNow },
            new() { ItemName = "Salt", Category = "Food", EstimatedPrice = 0.80m, Currency = "USD", Unit = "kg", Source = "Estimated", LastUpdated = DateTime.UtcNow },
            new() { ItemName = "Bottled Water", Category = "Beverages", EstimatedPrice = 1.50m, Currency = "USD", Unit = "1.5L", Source = "Estimated", LastUpdated = DateTime.UtcNow },
            new() { ItemName = "Coffee", Category = "Beverages", EstimatedPrice = 15.00m, Currency = "USD", Unit = "kg", Source = "Estimated (World Bank baseline)", LastUpdated = DateTime.UtcNow },
            new() { ItemName = "Tea", Category = "Beverages", EstimatedPrice = 8.00m, Currency = "USD", Unit = "kg", Source = "Estimated (World Bank baseline)", LastUpdated = DateTime.UtcNow },

            // Non-Food Essentials
            new() { ItemName = "Fertilizers", Category = "Agriculture", EstimatedPrice = 25.00m, Currency = "USD", Unit = "50kg bag", Source = "Estimated (World Bank baseline)", LastUpdated = DateTime.UtcNow },
            new() { ItemName = "Batteries", Category = "Essentials", EstimatedPrice = 5.00m, Currency = "USD", Unit = "pack", Source = "Estimated", LastUpdated = DateTime.UtcNow },
        };
    }

    // ---- World Bank API DTOs ----
    private record WorldBankResponse(List<WorldBankCommodity>? Data);
    private record WorldBankCommodity(
        [property: JsonPropertyName("commodity")] string? Commodity,
        [property: JsonPropertyName("value")] double? Value,
        [property: JsonPropertyName("unit")] string? Unit
    );
}
