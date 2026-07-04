Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Parent $PSScriptRoot
$inventoryPath = Join-Path $repoRoot 'repo-file-inventory.json'

if (-not (Test-Path -LiteralPath $inventoryPath)) {
  throw 'repo-file-inventory.json is missing. Run scripts/update-inventory.ps1.'
}

$inventory = Get-Content -LiteralPath $inventoryPath -Raw | ConvertFrom-Json
if ($inventory.schemaVersion -ne 1) {
  throw 'repo-file-inventory.json has an unsupported schemaVersion.'
}

function Get-InventoryRole {
  param([Parameter(Mandatory = $true)][string]$RelativePath)

  if ($RelativePath -like 'src/*') { return 'source' }
  if ($RelativePath -like 'tests/*') { return 'test' }
  if ($RelativePath -like 'docs/*' -or $RelativePath -eq 'README.md' -or $RelativePath -eq 'CODEX_AGENT_PLAN.md') {
    return 'documentation'
  }
  if ($RelativePath -like 'scripts/*') { return 'tooling' }
  if ($RelativePath -like '.github/*') { return 'ci' }
  return 'project'
}

$expected = git -C $repoRoot ls-files |
  Where-Object { -not [string]::IsNullOrWhiteSpace($_) } |
  Sort-Object |
  ForEach-Object {
    $relative = $_.Replace('\', '/')
    "$relative|$(Get-InventoryRole -RelativePath $relative)|True"
  }

$actual = @($inventory.files) |
  Sort-Object path |
  ForEach-Object {
    "$($_.path)|$($_.role)|$($_.tracked)"
  }

$difference = Compare-Object -ReferenceObject $expected -DifferenceObject $actual

if ($null -ne $difference) {
  $sample = ($difference | Select-Object -First 20 | ForEach-Object { "$($_.SideIndicator) $($_.InputObject)" }) -join "`n"
  throw "repo-file-inventory.json is stale. Run scripts/update-inventory.ps1. First differences:`n$sample"
}

Write-Host 'Inventory check passed.'
