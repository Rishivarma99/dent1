#!/usr/bin/env pwsh
$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot "../..")).Path

$apiFolders = Get-ChildItem -Path $repoRoot -Directory -Filter "api" -Recurse -ErrorAction SilentlyContinue |
    Where-Object { $_.FullName -notmatch '[\\/]node_modules[\\/]' }

$apiProject = $null
foreach ($apiFolder in $apiFolders) {
    $match = Get-ChildItem -Path $apiFolder.FullName -Filter "*.Api.csproj" -Recurse -File -ErrorAction SilentlyContinue |
        Where-Object { $_.FullName -notmatch '[\\/](obj|bin)[\\/]' } |
        Select-Object -First 1

    if ($match) {
        $apiProject = $match
        break
    }
}

if (-not $apiProject) {
    $apiProject = Get-ChildItem -Path $repoRoot -Filter "*.Api.csproj" -Recurse -File -ErrorAction SilentlyContinue |
        Where-Object { $_.FullName -notmatch '[\\/](obj|bin|node_modules)[\\/]' } |
        Sort-Object { $_.FullName.Length } |
        Select-Object -First 1
}

if (-not $apiProject) {
    Write-Error "No .NET API project (*.Api.csproj) found under $repoRoot"
    exit 1
}

Write-Host "API project: $($apiProject.FullName)"
Write-Host "Starting .NET API..."

Set-Location -LiteralPath $apiProject.DirectoryName
dotnet run --project $apiProject.FullName
