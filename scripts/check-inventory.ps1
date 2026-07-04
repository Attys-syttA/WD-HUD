Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Parent $PSScriptRoot
$inventoryPath = Join-Path $repoRoot 'repo-file-inventory.json'

if (-not (Test-Path -LiteralPath $inventoryPath)) {
  throw 'repo-file-inventory.json is missing. Run scripts/update-inventory.ps1.'
}

$before = Get-Content -LiteralPath $inventoryPath -Raw
pwsh -File (Join-Path $PSScriptRoot 'update-inventory.ps1') | Out-Null
$after = Get-Content -LiteralPath $inventoryPath -Raw

if ($before -ne $after) {
  throw 'repo-file-inventory.json was stale and has been updated. Review and rerun this check.'
}

Write-Host 'Inventory check passed.'
