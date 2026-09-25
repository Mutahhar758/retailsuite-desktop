<#
.SYNOPSIS
    Automated ClickOnce publish and release script for RetailSuite Desktop ERP.

.DESCRIPTION
    1. Increments <ApplicationRevision> in ERP/ERP.csproj (or uses provided -Revision).
    2. Updates AssemblyVersion and AssemblyFileVersion in ERP/Properties/AssemblyInfo.cs.
    3. Builds and publishes the ClickOnce app into ERP/publish.
    4. Commits both version files and pushes to GitHub origin master.
    5. Optionally copies publish output to a server directory (e.g. \\server\share or D:\...).

.EXAMPLE
    .\publish.ps1
    .\publish.ps1 -Revision 85
    .\publish.ps1 -ServerDir "\\192.168.1.100\Websites\RetailSuite_Desktop"
    .\publish.ps1 -NoPush
#>

param(
    [Parameter(Mandatory = $false)]
    [int]$Revision = 0,

    [Parameter(Mandatory = $false)]
    [string]$ServerDir = "",

    [Parameter(Mandatory = $false)]
    [switch]$NoPush
)

$ErrorActionPreference = "Stop"
Set-Location -Path $PSScriptRoot

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host " RetailSuite Desktop - Publish & Release " -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan

# 1. Locate MSBuild
$vswhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
$msbuild = $null
if (Test-Path $vswhere) {
    $msbuild = & $vswhere -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe | Select-Object -First 1
}
if (-not $msbuild) {
    $candidates = @(
        "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe",
        "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe",
        "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe",
        "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe",
        "C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
    )
    foreach ($c in $candidates) {
        if (Test-Path $c) {
            $msbuild = $c
            break
        }
    }
}

if (-not $msbuild) {
    throw "MSBuild.exe not found! Please ensure Visual Studio or Visual Studio Build Tools is installed."
}
Write-Host "Found MSBuild at: $msbuild" -ForegroundColor Green

# 2. Determine Version
$csprojPath = "ERP\ERP.csproj"
$assemblyInfoPath = "ERP\Properties\AssemblyInfo.cs"

if (-not (Test-Path $csprojPath)) {
    throw "Project file '$csprojPath' not found!"
}

[xml]$proj = Get-Content $csprojPath
$currentRev = [int]($proj.Project.PropertyGroup | Where-Object { $_.ApplicationRevision -ne $null } | Select-Object -First 1).ApplicationRevision

if ($Revision -gt 0) {
    $newRev = $Revision
} else {
    $newRev = $currentRev + 1
}

$version = "1.0.0.$newRev"
Write-Host "Updating version: 1.0.0.$currentRev -> $version" -ForegroundColor Yellow

# 2a. Update ERP.csproj with new revision
$content = Get-Content $csprojPath -Raw
$content = $content -replace "<ApplicationRevision>\d+</ApplicationRevision>", "<ApplicationRevision>$newRev</ApplicationRevision>"
Set-Content -Path $csprojPath -Value $content -Encoding UTF8

# 2b. Update AssemblyInfo.cs (AssemblyVersion and AssemblyFileVersion)
if (Test-Path $assemblyInfoPath) {
    $infoContent = Get-Content $assemblyInfoPath -Raw
    $infoContent = $infoContent -replace '(?m)^\[assembly:\s*AssemblyVersion\("[^"]+"\)', "[assembly: AssemblyVersion(`"$version`")"
    $infoContent = $infoContent -replace '(?m)^\[assembly:\s*AssemblyFileVersion\("[^"]+"\)', "[assembly: AssemblyFileVersion(`"$version`")"
    Set-Content -Path $assemblyInfoPath -Value $infoContent -Encoding UTF8
    Write-Host "Updated AssemblyInfo.cs to $version" -ForegroundColor Green
}

# 3. Publish via MSBuild
Write-Host "`nPublishing ClickOnce application..." -ForegroundColor Cyan
& $msbuild $csprojPath /target:Publish /p:Configuration=Release /p:Platform=x86 /p:PublishDir="publish\" /p:ApplicationRevision=$newRev /p:ApplicationVersion=$version /v:m

if ($LASTEXITCODE -ne 0) {
    throw "MSBuild publish failed with exit code $LASTEXITCODE"
}

$setupPath = "ERP\publish\setup.exe"
if (-not (Test-Path $setupPath)) {
    throw "Publish output '$setupPath' was not found!"
}
Write-Host "Publish succeeded! Files generated in ERP\publish\" -ForegroundColor Green

# 4. Optional Server Directory Copy
if ($ServerDir -and $ServerDir.Trim() -ne "") {
    Write-Host "`nCopying publish files to server: $ServerDir" -ForegroundColor Cyan
    if (-not (Test-Path $ServerDir)) {
        New-Item -ItemType Directory -Force -Path $ServerDir | Out-Null
    }
    Copy-Item -Path "ERP\publish\*" -Destination $ServerDir -Recurse -Force
    Write-Host "Files copied to server destination successfully." -ForegroundColor Green
}

# 5. Git Commit & Push
if (-not $NoPush) {
    Write-Host "`nCommitting and pushing to GitHub..." -ForegroundColor Cyan
    git add $csprojPath $assemblyInfoPath
    git commit -m "chore(release): bump desktop version to $version"
    git push origin master
    Write-Host "Pushed version bump ($version) to GitHub master." -ForegroundColor Green
} else {
    Write-Host "`nSkipped git push (-NoPush specified)." -ForegroundColor Yellow
}

Write-Host "`n=========================================" -ForegroundColor Green
Write-Host " All steps completed successfully!       " -ForegroundColor Green
Write-Host " Version: $version" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Green
