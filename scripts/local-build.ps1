Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$PSNativeCommandUseErrorActionPreference = $true

$repoRoot = Split-Path -Parent $PSScriptRoot
Set-Location $repoRoot

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
  $defaultDotnet = 'C:\Program Files\dotnet\dotnet.exe'
  if (Test-Path -LiteralPath $defaultDotnet) {
    $env:Path = 'C:\Program Files\dotnet;' + $env:Path
  }
  else {
    throw 'dotnet CLI was not found. Install the .NET SDK or add dotnet to PATH.'
  }
}

dotnet restore .\WD-HUD.sln
dotnet build .\WD-HUD.sln --configuration Release --no-restore
dotnet test .\WD-HUD.sln --configuration Release --no-build
pwsh -File .\scripts\security-scan.ps1
pwsh -File .\scripts\check-inventory.ps1
pwsh -File .\scripts\check-encoding.ps1
