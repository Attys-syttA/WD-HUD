Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Parent $PSScriptRoot
$excludedDirectories = @('.git', 'bin', 'obj', '.vs', 'TestResults', 'artifacts', 'publish')
$crlfAllowedExtensions = @('.ps1', '.bat', '.cmd', '.sln')

$issues = @()

$files = Get-ChildItem -LiteralPath $repoRoot -Recurse -File |
  Where-Object {
    $relative = $_.FullName.Substring($repoRoot.Length + 1)
    $segments = $relative -split '[\\/]'
    -not ($segments | Where-Object { $excludedDirectories -contains $_ })
  }

foreach ($file in $files) {
  $relative = $file.FullName.Substring($repoRoot.Length + 1)
  $bytes = [System.IO.File]::ReadAllBytes($file.FullName)

  if ($bytes.Length -ge 2) {
    $hasUtf16LeBom = $bytes[0] -eq 0xFF -and $bytes[1] -eq 0xFE
    $hasUtf16BeBom = $bytes[0] -eq 0xFE -and $bytes[1] -eq 0xFF
    if ($hasUtf16LeBom -or $hasUtf16BeBom) {
      $issues += "${relative}: UTF-16 BOM is not allowed"
    }
  }

  $extension = [System.IO.Path]::GetExtension($file.Name).ToLowerInvariant()
  $allowsCrlf = $crlfAllowedExtensions -contains $extension
  if (-not $allowsCrlf -and $bytes.Length -ge 2) {
    for ($index = 0; $index -lt ($bytes.Length - 1); $index++) {
      if ($bytes[$index] -eq 0x0D -and $bytes[$index + 1] -eq 0x0A) {
        $issues += "${relative}: CRLF line endings are not allowed for this file type"
        break
      }
    }
  }
}

if ($issues.Count -gt 0) {
  $issues | ForEach-Object { Write-Error $_ }
  throw 'Encoding check failed.'
}

Write-Host 'Encoding check passed.'
