param(
  [Parameter(Mandatory = $true)]
  [string]$ExePath
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$resolvedPath = (Resolve-Path -LiteralPath $ExePath).Path
$ruleName = "WD-HUD outbound block - $resolvedPath"

Get-NetFirewallRule -DisplayName $ruleName -ErrorAction SilentlyContinue |
  Remove-NetFirewallRule

Write-Host "Removed firewall rule if it existed: $ruleName"
