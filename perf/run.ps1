param(
    [ValidateSet('smoke', 'load', 'stress')]
    [string]$Profile = 'load',
    [string]$BaseUrl = 'http://localhost:8080/api/v1'
)

$env:BASE_URL = $BaseUrl

switch ($Profile) {
    'smoke' {
        Write-Host "Smoke test: 1 VU, 30s" -ForegroundColor Cyan
        k6 run --vus 1 --duration 30s "$PSScriptRoot\k6\main.js"
    }
    'load' {
        Write-Host "Load test: 30 VUs, 3 minutos" -ForegroundColor Cyan
        k6 run "$PSScriptRoot\k6\main.js"
    }
    'stress' {
        Write-Host "Stress test: rampa 0→50 VUs" -ForegroundColor Cyan
        k6 run --stage '0s:0,1m:50,3m:50,1m:0' --vus 0 "$PSScriptRoot\k6\main.js"
    }
}
