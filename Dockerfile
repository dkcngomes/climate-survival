# ── Build Stage ──
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file from backend/
COPY backend/ClimateAdvisor.Api.csproj .
RUN dotnet restore

# Copy all backend source
COPY backend/ .
RUN dotnet publish -c Release -o /app

# ── Runtime Stage ──
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS runtime
WORKDIR /app

# Install curl for health checks
RUN apk add --no-cache curl

COPY --from=build /app .

EXPOSE 7860

ENV ASPNETCORE_URLS=http://+:7860
ENV ASPNETCORE_ENVIRONMENT=Production

HEALTHCHECK --interval=30s --timeout=5s --retries=3 \
  CMD curl -f http://localhost:7860/ || exit 1

ENTRYPOINT ["dotnet", "ClimateAdvisor.Api.dll"]
