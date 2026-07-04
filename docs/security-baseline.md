# Security Baseline

Last updated: 2026-07-04

## Runtime Guarantees By Design

- no network communication
- no telemetry
- no analytics
- no updater
- no remote config
- no cloud sync
- no embedded browser or WebView
- no hidden background service
- no web dependency during normal runtime

## Forbidden APIs And Patterns

The security scan is intentionally conservative and searches for:

- `HttpClient`
- `WebClient`
- `Socket`
- `TcpClient`
- `UdpClient`
- `ClientWebSocket`
- `github.com`
- `telemetry`
- `analytics`
- `download`
- `Squirrel`
- `WinSparkle`
- `NetSparkle`
- `Process.Start`

False positives are acceptable. A match must be reviewed before continuing.

## Forbidden Package Categories

- updater framework
- telemetry SDK
- analytics SDK
- embedded browser
- remote config package
- cloud SDK

## Allowed Local Windows Integrations

- current-user startup registration
- local JSON settings under `%LocalAppData%\WD-HUD\settings.json`
- Windows API calls for window click-through behavior
- Windows Defender Firewall scripts for optional outbound blocking

## Scan Strategy

Local command:

```powershell
pwsh -File scripts/security-scan.ps1
```

Full local validation:

```powershell
pwsh -File scripts/local-build.ps1
```
