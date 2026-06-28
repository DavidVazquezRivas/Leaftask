# Genera informe de cobertura de las capas Domain y Application
# Uso: .\coverage.ps1

Set-Location $PSScriptRoot

# Limpia resultados anteriores
if (Test-Path TestResults) {
    Remove-Item -Recurse -Force TestResults
}

# Instala herramientas locales si no están
dotnet tool restore

# Encuentra todos los proyectos de test
$testProjects = Get-ChildItem -Path . -Filter "*.UnitTests.csproj" -Recurse |
    Select-Object -ExpandProperty FullName

$coverageFiles = @()

foreach ($proj in $testProjects) {
    $projName = [System.IO.Path]::GetFileNameWithoutExtension($proj)
    $outputDir = "$PSScriptRoot\TestResults\$projName"
    New-Item -ItemType Directory -Force -Path $outputDir | Out-Null
    $outputFile = "$outputDir\coverage.cobertura.xml"

    Write-Host "Running: $projName" -ForegroundColor Cyan

    dotnet dotnet-coverage collect "dotnet test `"$proj`" --no-build" `
        --output $outputFile `
        --output-format cobertura

    if ($LASTEXITCODE -ne 0) {
        Write-Error "Tests fallaron: $projName"
        exit 1
    }

    if (Test-Path $outputFile) {
        $coverageFiles += $outputFile
    }
}

if ($coverageFiles.Count -eq 0) {
    Write-Error "No se generaron archivos de cobertura."
    exit 1
}

Write-Host ""
Write-Host "Generando informe HTML..." -ForegroundColor Green

$reportsArg = $coverageFiles -join ";"
$gitTag = git rev-parse --short HEAD 2>$null

# assemblyfilters: include only Domain/Application layers, exclude test projects
$assemblyFilters = "+Modules.*.Domain;+Modules.*.Application;+BuildingBlocks.Domain;+BuildingBlocks.Application;-*.UnitTests;-*.Integration;-*.Infrastructure;-*.DrivenInfrastructure;-*.DrivingInfrastructure;-Api.Host"

# classfilters: exclude pure data containers and declarative infrastructure
# - *Validator: FluentValidation rule declarations (tested via integration tests)
# - *Dto / *Request / *Response: data-transfer objects (no business logic)
# - *Attribute: attribute markers
# - *PermissionBehavior: pipeline behaviors (integration concerns)
# - *AgentOrchestrator / *LlmLoggingBehavior / *LlmPromptInjectionBehavior: AI pipeline (integration)
$classFilters = "-*Validator*;-*Dto*;-*Attribute*;-*PermissionBehavior*;-*AgentOrchestrator*;-*LlmLoggingBehavior*;-*LlmPromptInjectionBehavior*;-*BootstrapEventTrigger*;-*ReplayContext*;-*ActivityLog*;-*LoggingBehavior*;-*ValidationBehavior*;-*CursorPaginationHelper*;-*CursorSortFieldDefinition*;-*SortCriterion*;-*PresignedUploadResult*"

dotnet reportgenerator `
    "-reports:$reportsArg" `
    "-targetdir:$PSScriptRoot\TestResults\CoverageReport" `
    "-reporttypes:Html;HtmlSummary;Badges;TextSummary" `
    "-title:Leaftask API — Domain & Application Coverage" `
    "-tag:$gitTag" `
    "-assemblyfilters:$assemblyFilters" `
    "-classfilters:$classFilters"

Write-Host ""
Write-Host "Informe: TestResults/CoverageReport/index.html" -ForegroundColor Green

if (Test-Path "$PSScriptRoot\TestResults\CoverageReport\Summary.txt") {
    Get-Content "$PSScriptRoot\TestResults\CoverageReport\Summary.txt"
}

Start-Process "$PSScriptRoot\TestResults\CoverageReport\index.html"
