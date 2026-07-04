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

Bootstrap is complete and local validation is available. The machine has the .NET 10 SDK installed; `scripts/local-build.ps1` restores, builds, tests, and runs the local safety checks.

The current MVP scaffold includes Contracts/Core models, a LibreHardwareMonitor-based `ISystemMetricsProvider`, null-safe metric normalization, and a minimal WPF HUD window. Manual visible HUD smoke testing confirmed that the window opens, stays topmost, renders the expected fields, and updates safely when some sensors are unavailable.

Metric values are color-coded by status: cool blue, optimal green, warm orange, and critical red. Unknown values remain white and render as `N/A`.

The HUD starts with Windows by registering the current `WdHud.App.exe` under the current user's Run key. It is a `WinExe` app, so startup uses the executable directly rather than opening a command window. The HUD can be dragged, minimized to the notification area, and restored from the tray icon.

## Elevation

WD-HUD requests administrator rights because reliable CPU temperature access through LibreHardwareMonitor requires elevated local sensor access on the target machine. The elevation is only for local hardware sensor reading; the app still has no runtime network, telemetry, updater, cloud, remote-config, or WebView behavior by design.

## Build

With the .NET 10 SDK installed:

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
