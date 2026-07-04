# WD-HUD

WD-HUD is a small Windows desktop HUD for local system metrics.

It is planned as a native C#/.NET/WPF application that displays:

- local time
- CPU usage
- RAM usage
- GPU usage
- CPU temperature
- GPU temperature

## What this app does NOT do

- no telemetry
- no analytics
- no auto-update
- no cloud sync
- no remote config
- no internet access by design
- no embedded browser or WebView

## Current Status

Bootstrap is in progress. The local machine currently does not expose the `dotnet` CLI, so build validation is blocked until the .NET SDK is installed or added to PATH.

## Build

After installing the .NET 10 SDK:

```powershell
pwsh -File scripts/local-build.ps1
```

## Security Scan

```powershell
pwsh -File scripts/security-scan.ps1
```

## Workflow Checks

The repository has GitHub Actions for:

- aggregate CI
- build and tests
- security pattern scan
- repository file inventory check
- encoding and line-ending check
- optional GitGuardian secret scan when `GITGUARDIAN_API_KEY` is configured

Local equivalent:

```powershell
pwsh -File scripts/local-build.ps1
```

## Network Block

After publishing the app, create an outbound Windows Defender Firewall block rule for the app executable:

```powershell
pwsh -File scripts/block-network.ps1 -ExePath .\publish\WdHud.App.exe
```

Verify:

```powershell
pwsh -File scripts/verify-no-network.ps1
```
