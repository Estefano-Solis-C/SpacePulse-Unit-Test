# SpacePulse Unified Automated Test Runner
param(
    [string]$Target = "all" # all, frontend, backend, pytest, postman
)

Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host " SpacePulse (RentalPe) Automated Test Orchestrator" -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$unitTestRoot = Split-Path -Parent $scriptRoot
$frontendRoot = Join-Path (Split-Path -Parent $unitTestRoot) "FRONTEND"

if ($Target -eq "all" -or $Target -eq "frontend") {
    Write-Host "`n[STAGE 1/4] Running Frontend Jasmine/Karma Specs..." -ForegroundColor Yellow
    Push-Location $frontendRoot
    if (Test-Path "package.json") {
        npm test -- --watch=false --browsers=ChromeHeadless
    }
    Pop-Location
}

if ($Target -eq "all" -or $Target -eq "backend") {
    Write-Host "`n[STAGE 2/4] Running Backend .NET xUnit / Moq CQRS Tests..." -ForegroundColor Yellow
    Push-Location (Join-Path $unitTestRoot "unit\backend")
    dotnet test --logger "console;verbosity=normal"
    Pop-Location
}

if ($Target -eq "all" -or $Target -eq "pytest") {
    Write-Host "`n[STAGE 3/4] Running API Integration & Contract Tests (Pytest)..." -ForegroundColor Yellow
    Push-Location (Join-Path $unitTestRoot "integration\pytest")
    python -m pytest -v
    Pop-Location
}

if ($Target -eq "all" -or $Target -eq "postman") {
    Write-Host "`n[STAGE 4/4] Running Postman / Newman Regression Suite..." -ForegroundColor Yellow
    Push-Location (Join-Path $unitTestRoot "integration\postman")
    if (Get-Command newman -ErrorAction SilentlyContinue) {
        newman run spacepulse.postman_collection.json -e spacepulse.environment.json
    } else {
        Write-Host "Newman is not installed globally. Run: npm install -g newman" -ForegroundColor Gray
    }
    Pop-Location
}

Write-Host "`nAll test stages processed successfully." -ForegroundColor Green
