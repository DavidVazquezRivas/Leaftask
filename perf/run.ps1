param(
    [ValidateSet('smoke', 'load', 'stress')]
    [string]$Profile = 'load',
    [string]$BaseUrl = 'http://localhost:8080/api/v1'
)

$env:BASE_URL = $BaseUrl

switch ($Profile) {
    'smoke' {
        Write-Host "Smoke test: 1 VU, 30s por escenario" -ForegroundColor Cyan
        k6 run --env "PROFILE=smoke" --env "BASE_URL=$BaseUrl" "$PSScriptRoot\k6\main.js"
    }
    'load' {
        Write-Host "Load test: 30 VUs, 2 minutos por escenario" -ForegroundColor Cyan
        k6 run --env "BASE_URL=$BaseUrl" "$PSScriptRoot\k6\main.js"
    }
    'stress' {
        Write-Host "Stress test: VUs x1.6, 4 minutos por escenario" -ForegroundColor Cyan
        k6 run --env "PROFILE=stress" --env "BASE_URL=$BaseUrl" "$PSScriptRoot\k6\main.js"
    }
}
