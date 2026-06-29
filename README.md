---
title: Climate Survival API
emoji: 🌍
colorFrom: green
colorTo: blue
sdk: docker
app_file: backend/Dockerfile
app_port: 7860
---

# ClimateBuy 🛒🌤

**Smart purchasing recommendations based on climate forecasts.**

Know what to stock before prices rise. ClimateBuy uses seasonal climate forecasts to detect El Niño, La Niña, drought, and other extreme weather signals, then recommends consumer items to pre-purchase before prices surge.

## Architecture

```
┌─────────────────────┐     ┌──────────────────────┐     ┌─────────────────┐
│   Next.js Frontend   │────▶│  .NET 9 Backend API  │────▶│   Open-Meteo    │
│  (S3 + CloudFront)   │     │   (ECS Fargate)      │     │ Seasonal API    │
│                      │     │                      │     │ (free, no key)  │
│  • Browser Geo       │     │  • Rule Engine        │     └─────────────────┘
│  • Climate Overview  │     │  • Price Aggregator   │     ┌─────────────────┐
│  • Recommendations   │     │  • Caching            │────▶│   World Bank    │
└─────────────────────┘     └──────────────────────┘     │   Commodity API  │
                                                          │   (free)         │
                                                          └─────────────────┘
```

## Tech Stack

| Layer | Technology |
|-------|-----------|
| **Frontend** | Next.js 16, React, TypeScript, Tailwind CSS |
| **Backend** | .NET 9, ASP.NET Core Web API |
| **APIs** | Open-Meteo Seasonal Forecast, World Bank Pink Sheet, BigDataCloud Reverse Geo |
| **Deployment** | AWS (ECS Fargate / S3 + CloudFront) |
| **Infrastructure** | Docker, Terraform |

## Project Structure

```
climate-advisor/
├── backend/                    # .NET 9 Web API
│   ├── Controllers/            # API endpoints
│   ├── Models/                 # Domain models
│   ├── Services/               # Business logic
│   │   ├── ClimateService.cs   # Open-Meteo integration
│   │   ├── PriceService.cs     # World Bank price data
│   │   └── RecommendationService.cs  # Rule engine
│   ├── Dockerfile
│   └── Program.cs
├── frontend/                   # Next.js 16 app
│   ├── src/
│   │   ├── app/                # Pages
│   │   ├── components/         # UI components
│   │   ├── services/           # API client
│   │   └── types/              # TypeScript types
│   ├── Dockerfile
│   └── .env.local
└── infrastructure/             # Terraform / AWS config
```

## Quick Start (Local Development)

### Prerequisites
- .NET 9 SDK
- Node.js 20+
- Docker (optional)

### Backend
```bash
cd backend
dotnet run --urls "http://localhost:8080"
# Health check: http://localhost:8080/api/recommendations/health
# Test: http://localhost:8080/api/recommendations?lat=6.9271&lng=79.8612
```

### Frontend
```bash
cd frontend
npm install
npm run dev
# Open: http://localhost:3000
```

### Docker Compose
```bash
docker-compose up
# Backend: http://localhost:8080
# Frontend: http://localhost:3000
```

## API Endpoints

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/recommendations?lat=X&lng=X` | GET | Get climate + purchase recommendations |
| `/api/recommendations/health` | GET | Health check |

## Rule Engine Logic

Climate signals are detected from seasonal forecast anomalies:

| Signal | Threshold | Items |
|--------|-----------|-------|
| **El Niño** | Temp anomaly > +1.5°C, Precip < -20mm | Rice, Flour, Canned Food, Cooking Oil, Sugar |
| **La Niña** | Temp anomaly < -1.0°C, Precip > +30mm | Canned Food, Bottled Water, Rice, Batteries |
| **Drought** | Precip anomaly < -30mm | Rice, Flour, Canned Food, Cooking Oil, Powdered Milk, Beef |
| **Heavy Rain** | Precip anomaly > +40mm | Canned Food, Bottled Water, Batteries, Chicken |
| **Heatwave** | Extreme Temp Index > 0.7 | Bottled Water, Canned Food |
| **Cold Spell** | Temp anomaly < -3.0°C | Canned Food, Powdered Milk |

## Deployment to AWS

See [infrastructure/README.md](infrastructure/README.md) for Terraform deployment guide.

## Free APIs Used

- **Open-Meteo Seasonal Forecast**: 7-month ECMWF SEAS5 climate forecast (free, no API key)
- **Open-Meteo Weather Forecast**: 16-day high-resolution forecast (free, no API key)
- **BigDataCloud Reverse Geocoding**: Convert lat/lng to city names (free, 10K/day)
- **World Bank Pink Sheet**: Monthly commodity price data (free)

## License

MIT
