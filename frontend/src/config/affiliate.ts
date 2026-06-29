/**
 * Affiliate link configuration for Climate Survival.
 *
 * To use your own Amazon Associates tracking ID:
 * 1. Sign up at https://affiliate-program.amazon.com/
 * 2. Replace the value below with your tracking ID
 */

export const AFFILIATE_CONFIG = {
  amazon: {
    tag: "climatisurvi-20", // ← Replace with your own Amazon Associates tag
    baseUrl: "https://www.amazon.com",
  },
} as const;

/**
 * Map item names to Amazon search keywords for relevant product results.
 * Falls back to the item name itself if not in the map.
 */
const amazonSearchMap: Record<string, string> = {
  "Bottled Water": "water storage containers",
  Rice: "rice bulk supply",
  "Flour / Wheat": "flour bulk",
  "Canned Food (Mixed)": "canned food variety pack",
  "Cooking Oil": "cooking oil bulk",
  "Dried Beans": "dried beans bulk",
  Sugar: "sugar bulk",
  Salt: "salt bulk",
  "Pasta / Noodles": "pasta bulk",
  "Oats / Cereal": "oatmeal bulk",
  "Powdered Milk": "powdered milk bulk",
  "Coffee / Tea": "coffee beans bulk",
  "First Aid Kit": "first aid kit emergency",
  "Water Filter": "water filter emergency",
  "Seeds (Non-Hybrid)": "non hybrid seeds garden",
  "Gardening Tools": "gardening tools set",
  "Batteries": "batteries bulk",
  Flashlights: "flashlights emergency",
  "Solar Charger": "solar charger portable",
  "Fuel (Propane)": "propane camping",
  Multivitamins: "multivitamins bulk",
  "Freeze-Dried Food": "freeze dried food emergency",
  "Prescription Meds": "emergency medication organizer",
  "Hygiene Products": "emergency hygiene kit",
  "Bleach / Sanitizer": "water purification tablets",
  Fertilizer: "garden fertilizer organic",
  "Soil / Compost": "potting soil organic",
  "Pesticide (Natural)": "natural pesticide garden",
  "Drip Irrigation": "drip irrigation kit",
  "Frost Blanket": "frost protection fabric",
  "Shade Cloth": "shade cloth garden",
  "Greenhouse Supplies": "greenhouse small",
  "Rain Barrel": "rain barrel water collection",
  "Seed Trays": "seed starting trays",
  "Grow Lights": "grow lights indoor",
};

export function getAmazonSearchUrl(itemName: string): string {
  const searchTerm = amazonSearchMap[itemName] || itemName;
  const encoded = encodeURIComponent(searchTerm);
  return `${AFFILIATE_CONFIG.amazon.baseUrl}/s?k=${encoded}&tag=${AFFILIATE_CONFIG.amazon.tag}`;
}
