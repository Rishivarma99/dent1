#!/usr/bin/env pwsh
param(
    [int]$Port = 4200
)

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot "../..")).Path

$angularProjects = Get-ChildItem -Path $repoRoot -Filter "angular.json" -Recurse -File -ErrorAction SilentlyContinue |
    Where-Object { $_.FullName -notmatch '[\\/]node_modules[\\/]' }

if (-not $angularProjects) {
    Write-Error "No Angular project (angular.json) found under $repoRoot"
    exit 1
}

$project = if ($angularProjects.Count -gt 1) {
    Write-Host "Found $($angularProjects.Count) Angular projects; using the shallowest match."
    $angularProjects | Sort-Object { $_.FullName.Length } | Select-Object -First 1
} else {
    $angularProjects[0]
}

$projectDir = $project.DirectoryName
Write-Host "Angular project: $projectDir"
Write-Host "Starting dev server on port $Port..."

Set-Location -LiteralPath $projectDir
npx ng serve --port $Port
