<#
.SYNOPSIS
    Produces a release build of CodeShuttle, and optionally the installer.

.DESCRIPTION
    Publishes a single-file, self-contained, ReadyToRun x64 build to
    publish\win-x64\ and, with -Installer, runs Inno Setup over it to produce
    artifacts\CodeShuttle-<version>-setup.exe.

    Deliberately NOT trimmed and NOT AOT. Both are hard-blocked for Windows
    Forms (PublishTrimmed fails with NETSDK1175), and this application would
    break specifically even if they were not: AppSettings and UpdateChecker use
    reflection-based JsonSerializer.Deserialize<T>, which trimming reduces to
    silently default-valued objects. Settings would appear to load, empty.

    InvariantGlobalization is likewise deliberately NOT set. It would save
    around 28 MB by dropping ICU, but this is a file tool that will meet
    Cyrillic, CJK and accented paths, and under invariant mode culture-sensitive
    comparison silently changes behaviour. SatelliteResourceLanguages=en in the
    csproj takes the size win without the correctness bug.

.PARAMETER Version
    Version to stamp, without a leading 'v'. Defaults to 1.0.0.

.PARAMETER Installer
    Also build the Inno Setup installer. Requires ISCC.exe on PATH or at the
    default install location.

.PARAMETER Clean
    Delete publish\ and artifacts\ first.

.EXAMPLE
    .\build\publish.ps1 -Version 1.0.0 -Installer
#>
[CmdletBinding()]
param(
    [string] $Version = '1.0.0',
    [switch] $Installer,
    [switch] $Clean
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$RepoRoot   = Split-Path -Parent $PSScriptRoot
$Project    = Join-Path $RepoRoot 'CodeShuttle.csproj'
$TestProj   = Join-Path $RepoRoot 'tests\CodeShuttle.Tests'
$PublishDir = Join-Path $RepoRoot 'publish\win-x64'
$ArtifactDir= Join-Path $RepoRoot 'artifacts'
$IssFile    = Join-Path $RepoRoot 'installer\CodeShuttle.iss'

# Version must be plain numeric for the assembly attributes; a tag like
# "v1.2.3" or "1.2.3-beta.1" is normalised here rather than at every use.
$FileVersion = ($Version -replace '^v', '') -replace '[-+].*$', ''
if ($FileVersion -notmatch '^\d+(\.\d+){0,3}$') {
    throw "Version '$Version' does not reduce to a numeric version (got '$FileVersion')."
}

Write-Host "CodeShuttle publish" -ForegroundColor Cyan
Write-Host "  version      : $Version (file version $FileVersion)"
Write-Host "  repo root    : $RepoRoot"
Write-Host ""

if ($Clean) {
    Write-Host "Cleaning..." -ForegroundColor Yellow
    foreach ($d in @($PublishDir, $ArtifactDir)) {
        if (Test-Path $d) { Remove-Item -Recurse -Force $d }
    }
}

# --- Build --------------------------------------------------------------
# --no-incremental is NOT optional. A warm obj/ reports "0 Warning(s)"
# regardless of the true state, because nothing recompiles.
Write-Host "Building (Release, no-incremental)..." -ForegroundColor Yellow
dotnet build $Project -c Release --no-incremental -warnaserror
if ($LASTEXITCODE -ne 0) { throw "Build failed." }

# --- Test ---------------------------------------------------------------
Write-Host "Testing..." -ForegroundColor Yellow
dotnet test $TestProj -c Release --nologo
if ($LASTEXITCODE -ne 0) { throw "Tests failed." }

# --- Publish ------------------------------------------------------------
Write-Host "Publishing single-file self-contained x64..." -ForegroundColor Yellow
dotnet publish $Project `
    -c Release `
    -r win-x64 `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:EnableCompressionInSingleFile=true `
    -p:PublishReadyToRun=true `
    -p:DebugType=embedded `
    -p:Version=$FileVersion `
    -p:InformationalVersion=$Version `
    -o $PublishDir
if ($LASTEXITCODE -ne 0) { throw "Publish failed." }

$Exe = Join-Path $PublishDir 'CodeShuttle.exe'
if (-not (Test-Path $Exe)) { throw "Expected $Exe to exist after publish." }

$sizeMb = [math]::Round((Get-Item $Exe).Length / 1MB, 1)
Write-Host ""
Write-Host "  -> $Exe ($sizeMb MB)" -ForegroundColor Green

# Copy the documents the installer lays down beside the exe. A compliance
# reviewer looks in the install directory, not inside the assembly.
foreach ($doc in @('LICENSE.txt', 'THIRD-PARTY-NOTICES.txt', 'PRIVACY.md', 'README.md', 'CHANGELOG.md')) {
    $src = Join-Path $RepoRoot $doc
    if (Test-Path $src) { Copy-Item $src -Destination $PublishDir -Force }
}

# --- Sign (deferred) ----------------------------------------------------
# Code signing is deferred pending business identity; see CHANGELOG.md.
# When a certificate is available, uncomment and supply the values through
# environment variables — never commit a .pfx, and note that .gitignore blocks
# *.pfx, *.snk and *.p12 precisely so an accident cannot happen quietly.
#
# Sign the exe BEFORE building the installer, and the installer afterwards
# (the .iss has its own SignTool hook for that half).
#
# $signtool = "${env:ProgramFiles(x86)}\Windows Kits\10\bin\10.0.22621.0\x64\signtool.exe"
# & $signtool sign `
#     /fd SHA256 `
#     /tr http://timestamp.digicert.com /td SHA256 `      # RFC 3161: signature outlives cert expiry
#     /f $env:CODESHUTTLE_PFX_PATH `
#     /p $env:CODESHUTTLE_PFX_PASSWORD `
#     $Exe
# if ($LASTEXITCODE -ne 0) { throw "Signing failed." }

# --- Installer ----------------------------------------------------------
if ($Installer) {
    Write-Host ""
    Write-Host "Building installer..." -ForegroundColor Yellow

    $iscc = (Get-Command 'ISCC.exe' -ErrorAction SilentlyContinue)
    if ($null -ne $iscc) {
        $isccPath = $iscc.Source
    } else {
        # The last entry is where winget's per-user install lands, which is easy
        # to miss because the documented location is the Program Files one.
        $candidates = @(
            "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe",
            "$env:ProgramFiles\Inno Setup 6\ISCC.exe",
            "$env:LOCALAPPDATA\Programs\Inno Setup 6\ISCC.exe"
        )
        $isccPath = $candidates | Where-Object { Test-Path $_ } | Select-Object -First 1
    }
    if (-not $isccPath) {
        throw "ISCC.exe not found. Install Inno Setup 6 (https://jrsoftware.org/isdl.php) or add it to PATH."
    }

    New-Item -ItemType Directory -Force -Path $ArtifactDir | Out-Null
    & $isccPath "/DAppVersion=$FileVersion" "/O$ArtifactDir" $IssFile
    if ($LASTEXITCODE -ne 0) { throw "ISCC failed." }

    $setup = Get-ChildItem $ArtifactDir -Filter '*.exe' | Sort-Object LastWriteTime -Descending | Select-Object -First 1
    if ($setup) {
        $setupMb = [math]::Round($setup.Length / 1MB, 1)
        Write-Host "  -> $($setup.FullName) ($setupMb MB)" -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "Done." -ForegroundColor Cyan
