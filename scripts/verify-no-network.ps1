param(
  [string]$ExePath
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

if ([string]::IsNullOrWhiteSpace($ExePath)) {
  $rules = Get-NetFirewallRule -DisplayName 'WD-HUD outbound block -*' -ErrorAction SilentlyContinue
}
else {
  $resolvedPath = (Resolve-Path -LiteralPath $ExePath).Path
  $rules = Get-NetFirewallRule -DisplayName "WD-HUD outbound block - $resolvedPath" -ErrorAction SilentlyContinue
}

if ($null -eq $rules) {
  throw 'No WD-HUD outbound block firewall rule was found.'
}

$rules | Format-Table DisplayName, Enabled, Direction, Action, Profile -AutoSize
Write-Host 'WD-HUD outbound block rule exists.'
