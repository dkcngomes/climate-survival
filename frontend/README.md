# Climate Survival — Frontend

Next.js 16 app with Tailwind CSS, static-exported to Netlify.

## Features

- Climate risk overview with interactive Recharts charts
- Stock-up recommendations with **localized currency prices** (auto-converted via live exchange rates)
- Crop recommendations with AI re-ranking (optional Gemini)
- Sri Lanka CBSL market prices (LK users only)
- Daraz affiliate links (LK users only)
- Multi-language support (en, si, es, fr, zh)
- PDF survival report download
- Contact form
- Google Analytics

## Currency Localization

When you select a location, the app detects your country and passes your local currency code (e.g. `LKR`, `EUR`, `GBP`) to the backend. The backend:

1. Gets the base price in USD for each stock-up item
2. Fetches the live USD → your currency exchange rate (Frankfurter API, free)
3. Returns the converted price with the correct currency code

The frontend uses `Intl.NumberFormat` to display the price in your local format.

## Local Development

```bash
npm install
npm run dev
```

Set `NEXT_PUBLIC_API_URL=http://localhost:8080` in `.env.local` to use local backend.

## Build

```bash
npm run build
# Output in out/ — ready for static hosting
```

## Deployment

Auto-deploys on Netlify when pushing to `dkcngomes/climate-survival-0b8a1`.
