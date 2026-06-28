@echo off
echo ── Climate Survival Local Development ──
echo.
echo NOTE: To enable contact form email forwarding, set:
echo       set EMAIL__SMTPPASS=your-gmail-app-password
echo.
echo See SETUP_GMAIL_SMTP.md for details.
echo.

echo [1/2] Starting Backend (.NET 9) on http://localhost:8080 ...
start "ClimateBuy Backend" cmd /c "cd /d %~dp0backend && dotnet run --urls http://localhost:8080"

echo [2/2] Starting Frontend (Next.js) on http://localhost:3000 ...
start "ClimateBuy Frontend" cmd /c "cd /d %~dp0frontend && npm run dev"

echo.
echo Backend:  http://localhost:8080/api/recommendations/health
echo Frontend: http://localhost:3000
echo.
