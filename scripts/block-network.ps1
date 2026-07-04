param(
  [Parameter(Mandatory = $true)]
  [string]$ExePath
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$resolvedPath = (Resolve-Path -LiteralPath $ExePath).Path
$ruleName = "WD-HUD outbound block - $resolvedPath"

if (-not (Get-Command New-NetFirewallRule -ErrorAction SilentlyContinue)) {
  throw 'Windows Defender Firewall PowerShell cmdlets are not available.'
}

$existing = Get-NetFirewallRule -DisplayName $ruleName -ErrorAction SilentlyContinue
if ($null -ne $existing) {
  Write-Host "Firewall rule already exists: $ruleName"
  exit 0
}

New-NetFirewallRule `
  -DisplayName $ruleName `
  -Direction Outbound `
  -Program $resolvedPath `
  -Action Block `
  -Profile Any `
  | Out-Null

Write-Host "Created firewall rule: $ruleName"
