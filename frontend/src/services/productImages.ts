/**
 * Product image lookup using the Wikipedia API (free, no API key, CORS-friendly).
 * Fetches the page thumbnail image for a given product term.
 */

interface ImageResult {
  imageUrl: string | null;
  loading: boolean;
}

// ── Curated Wikipedia page titles for better image results ──
const WIKI_PAGES: Record<string, string> = {
  // Grains
  "Rice": "Rice",
  "Flour / Wheat": "Flour",
  "Flour": "Flour",
  "Pasta": "Pasta",
  "Bread": "Bread",

  // Canned & Preserved
  "Canned Food (Mixed)": "Canned_food",
  "Canned Food": "Canned_food",
  "Canned Beans": "Canned_beans",
  "Canned Vegetables": "Canning",
  "Canned Soup": "Soup",
  "Canned Meat": "Canned_meat",
  "Canned Fish": "Canned_fish",
  "Canned Tuna": "Tuna",
  "Canned Tomatoes": "Tomato_sauce",

  // Oils & Fats
  "Cooking Oil": "Cooking_oil",
  "Olive Oil": "Olive_oil",
  "Coconut Oil": "Coconut_oil",

  // Protein
  "Beef / Meat": "Beef",
  "Beef": "Beef",
  "Chicken / Poultry": "Chicken_(food)",
  "Chicken": "Chicken_(food)",
  "Pork": "Pork",
  "Eggs": "Egg_(food)",
  "Milk": "Milk",
  "Powdered Milk": "Powdered_milk",
  "Cheese": "Cheese",

  // Essentials
  "Bottled Water": "Bottled_water",
  "Sugar": "Sugar",
  "Salt": "Salt",
  "Batteries": "Battery_(electricity)",
  "Toilet Paper": "Toilet_paper",
  "Soap": "Soap",
  "Medicine": "Pharmaceutical_drug",
  "First Aid Kit": "First_aid_kit",

  // Beverages
  "Coffee": "Coffee",
  "Tea": "Tea",
  "Juice": "Juice",

  // Home
  "Duct Tape & Tarps": "Duct_tape",
  "Torch": "Flashlight",
  "Flashlight": "Flashlight",
  "Charcoal": "Charcoal",
  "Firewood": "Firewood",
};

const API_URL = "https://en.wikipedia.org/w/api.php";

// In-memory cache to avoid repeated API calls
const cache = new Map<string, string | null>();

/**
 * Fetch a product image URL from Wikipedia.
 * Returns null if no image is found or if the request fails.
 */
export async function fetchProductImage(itemName: string): Promise<string | null> {
  // Check cache first
  if (cache.has(itemName)) return cache.get(itemName)!;

  const pageTitle = WIKI_PAGES[itemName] || itemName.replace(/\s+/g, "_");

  try {
    const url = `${API_URL}?action=query&titles=${encodeURIComponent(pageTitle)}&prop=pageimages&format=json&pithumbsize=400&origin=*`;
    const res = await fetch(url);
    if (!res.ok) {
      cache.set(itemName, null);
      return null;
    }

    const data = await res.json();
    const pages = data?.query?.pages;
    if (!pages) {
      cache.set(itemName, null);
      return null;
    }

    // Get the first (and only) page
    const pageId = Object.keys(pages)[0];
    const imageUrl = pages[pageId]?.thumbnail?.source || null;

    cache.set(itemName, imageUrl);
    return imageUrl;
  } catch {
    cache.set(itemName, null);
    return null;
  }
}
