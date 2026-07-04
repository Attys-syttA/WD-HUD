Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Parent $PSScriptRoot
$inventoryPath = Join-Path $repoRoot 'repo-file-inventory.json'

$excludedDirectories = @('.git', '.serena', 'bin', 'obj', '.vs', 'node_modules')

$files = Get-ChildItem -LiteralPath $repoRoot -Recurse -File |
  Where-Object {
    $relative = $_.FullName.Substring($repoRoot.Length + 1)
    $segments = $relative -split '[\\/]'
    -not ($segments | Where-Object { $excludedDirectories -contains $_ })
  } |
  Sort-Object FullName |
  ForEach-Object {
    $relative = $_.FullName.Substring($repoRoot.Length + 1).Replace('\', '/')
    [pscustomobject]@{
      path = $relative
      role = if ($relative -like 'src/*') { 'source' }
        elseif ($relative -like 'tests/*') { 'test' }
        elseif ($relative -like 'docs/*' -or $relative -eq 'README.md' -or $relative -eq 'CODEX_AGENT_PLAN.md') { 'documentation' }
        elseif ($relative -like 'scripts/*') { 'tooling' }
        elseif ($relative -like '.github/*') { 'ci' }
        else { 'project' }
      tracked = $true
    }
  }

$inventory = [pscustomobject]@{
  schemaVersion = 1
  files = $files
}

$json = ($inventory | ConvertTo-Json -Depth 6) -replace "`r?`n", "`n"
$utf8NoBom = [System.Text.UTF8Encoding]::new($false)
[System.IO.File]::WriteAllText($inventoryPath, "$json`n", $utf8NoBom)

Write-Host "Updated $inventoryPath"
