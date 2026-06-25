param(
    [ValidateSet('smoke', 'load', 'stress')]
    [string]$Profile = 'load',
    [string]$BaseUrl = 'http://localhost:8080/api/v1'
)

$resultsDir = "$PSScriptRoot\results"
New-Item -ItemType Directory -Force -Path $resultsDir | Out-Null
$timestamp = Get-Date -Format 'yyyyMMdd-HHmm'
$logFile   = "$resultsDir\$Profile-$timestamp.txt"
$jsonFile  = "$resultsDir\$Profile-$timestamp.json"

switch ($Profile) {
    'smoke' {
        Write-Host "Smoke test: 1 VU, 30s por escenario" -ForegroundColor Cyan
        k6 run --env "PROFILE=smoke" --env "BASE_URL=$BaseUrl" --env "SUMMARY_JSON=$jsonFile" "$PSScriptRoot\k6\main.js" 2>&1 | Tee-Object -FilePath $logFile
    }
    'load' {
        Write-Host "Load test: 30 VUs, 2 minutos por escenario" -ForegroundColor Cyan
        k6 run --env "BASE_URL=$BaseUrl" --env "SUMMARY_JSON=$jsonFile" "$PSScriptRoot\k6\main.js" 2>&1 | Tee-Object -FilePath $logFile
    }
    'stress' {
        Write-Host "Stress test: VUs x1.6, 4 minutos por escenario" -ForegroundColor Cyan
        k6 run --env "PROFILE=stress" --env "BASE_URL=$BaseUrl" --env "SUMMARY_JSON=$jsonFile" "$PSScriptRoot\k6\main.js" 2>&1 | Tee-Object -FilePath $logFile
    }
}

Write-Host ""
Write-Host "Informe guardado en:" -ForegroundColor Green
Write-Host "  Log:  $logFile" -ForegroundColor Gray
Write-Host "  JSON: $jsonFile" -ForegroundColor Gray
