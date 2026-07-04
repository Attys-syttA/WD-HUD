# WD-HUD Project State

## Current State

- Date: 2026-07-04
- Repo: `E:\codex_works\WD_HUD`
- Phase: MVP scaffold through first metrics snapshot
- Status: solution, projects, contracts, core logic, LibreHardwareMonitor metrics provider, null-safe snapshot normalization, WPF HUD shell, tests, docs, scripts, CI/workflows, and inventory are created.

## Open Items

- .NET 10 SDK is installed. Local validation works through `scripts/local-build.ps1`; the script still prepends `C:\Program Files\dotnet` if the current shell PATH has not refreshed yet.
- Manual smoke test of the visible WPF HUD is still open.
- Firewall rule creation was not executed because no published executable path was requested for local firewall changes.
- Remote is configured as `https://github.com/Attys-syttA/WD-HUD.git`.

## Recent Decisions

- Architecture follows separate `Contracts`, `Core`, `Infrastructure`, and `App` projects.
- `LibreHardwareMonitorLib` is the only planned runtime hardware-monitoring dependency.
- `WindowsEdgeLight` and `glassmorphism-wpf` are reference-only sources.
- The repository starts with tracked `STATE.md`, `docs/CHANGELOG.dev.md`, task folders, and full file inventory discipline.
- Target framework is `net10.0` / `net10.0-windows`, matching the installed local SDK/runtime.
- Current version is `0.1.00002`; the patch5 bump was needed for the first metrics snapshot hardening.
- GitHub Actions are split into `ci.yml`, `build-check.yml`, `security-scan.yml`, `inventory-check.yml`, `encoding-check.yml`, and `secret-scan.yml`.
- The first metrics provider is `LibreHardwareMonitorMetricsProvider`; it opens LibreHardwareMonitor lazily and returns `HudMetricsSnapshot.Empty(...)` if local sensor reading fails.
- `HudMetricsNormalizer` clamps percent values and removes non-finite sensor values before the HUD formats them.

## Latest Validation

- Commands:
  - `pwsh -File .\scripts\update-inventory.ps1`
  - `pwsh -File .\scripts\local-build.ps1`
  - `ggshield secret scan path --recursive --yes --use-gitignore .`
- Result:
  - build succeeded
  - tests passed: 10 passed, 0 failed
  - security scan passed
  - inventory check passed
  - encoding check passed
  - GitGuardian scan found no secrets

## Next Step

- Run a manual WPF HUD smoke test.
- Publish or commit only after user confirmation.

## Operational Notes

- Do not store secrets, tokens, credentials, or private machine data in this file.
