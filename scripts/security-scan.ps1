Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Parent $PSScriptRoot

$scanRoots = @(
  Join-Path $repoRoot 'src'
  Join-Path $repoRoot 'tests'
)

$sourcePatterns = @(
  'HttpClient',
  'WebClient',
  'Socket',
  'TcpClient',
  'UdpClient',
  'ClientWebSocket',
  'github.com',
  'telemetry',
  'analytics',
  'download',
  'Squirrel',
  'WinSparkle',
  'NetSparkle',
  'Process.Start'
)

$packagePatterns = @(
  'Squirrel',
  'WinSparkle',
  'NetSparkle',
  'WebView',
  'Telemetry',
  'Analytics',
  'RemoteConfig'
)

$hits = @()

foreach ($root in $scanRoots) {
  if (-not (Test-Path -LiteralPath $root)) {
    continue
  }

  $files = Get-ChildItem -LiteralPath $root -Recurse -File |
    Where-Object { $_.Extension -in @('.cs', '.xaml', '.csproj') }

  foreach ($file in $files) {
    foreach ($pattern in $sourcePatterns) {
      $matches = Select-String -LiteralPath $file.FullName -Pattern $pattern -SimpleMatch -CaseSensitive:$false
      foreach ($match in $matches) {
        $hits += [pscustomobject]@{
          File = $file.FullName.Substring($repoRoot.Length + 1)
          Line = $match.LineNumber
          Pattern = $pattern
          Text = $match.Line.Trim()
        }
      }
    }
  }
}

$projectFiles = Get-ChildItem -LiteralPath $repoRoot -Recurse -File |
  Where-Object { $_.Name -in @('Directory.Packages.props') -or $_.Extension -eq '.csproj' }

foreach ($file in $projectFiles) {
  foreach ($pattern in $packagePatterns) {
    $matches = Select-String -LiteralPath $file.FullName -Pattern $pattern -SimpleMatch -CaseSensitive:$false
    foreach ($match in $matches) {
      $hits += [pscustomobject]@{
        File = $file.FullName.Substring($repoRoot.Length + 1)
        Line = $match.LineNumber
        Pattern = $pattern
        Text = $match.Line.Trim()
      }
    }
  }
}

if ($hits.Count -gt 0) {
  $hits | Format-Table -AutoSize | Out-String | Write-Error
  throw 'Security scan failed because forbidden or suspicious patterns were found.'
}

Write-Host 'Security scan passed.'
