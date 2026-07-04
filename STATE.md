# WD-HUD Project State

## Current State

- Date: 2026-07-04
- Repo: `E:\codex_works\WD_HUD`
- Phase: Bootstrap through initial MVP scaffold
- Status: solution, projects, contracts, core logic, infrastructure skeleton, WPF HUD shell, tests, docs, scripts, CI/workflows, and inventory are created.

## Open Items

- The current shell does not inherit the updated Windows PATH yet, so validation prepends `C:\Program Files\dotnet` locally.
- Manual smoke test of the visible WPF HUD is still open.
- Firewall rule creation was not executed because no published executable path was requested for local firewall changes.
- Remote is configured as `https://github.com/Attys-syttA/WD-HUD.git`.

## Recent Decisions

- Architecture follows separate `Contracts`, `Core`, `Infrastructure`, and `App` projects.
- `LibreHardwareMonitorLib` is the only planned runtime hardware-monitoring dependency.
- `WindowsEdgeLight` and `glassmorphism-wpf` are reference-only sources.
- The repository starts with tracked `STATE.md`, `docs/CHANGELOG.dev.md`, task folders, and full file inventory discipline.
- Target framework is `net10.0` / `net10.0-windows`, matching the installed local SDK/runtime.
- Version starts at `0.1.00001` because this is the first application scaffold.
- GitHub Actions are split into `ci.yml`, `build-check.yml`, `security-scan.yml`, `inventory-check.yml`, `encoding-check.yml`, and `secret-scan.yml`.

## Latest Validation

- Commands:
  - `pwsh -File .\scripts\update-inventory.ps1`
  - `pwsh -File .\scripts\local-build.ps1`
  - `ggshield secret scan path --recursive --yes --use-gitignore .`
- Result:
  - build succeeded
  - tests passed: 8 passed, 0 failed
  - security scan passed
  - inventory check passed
  - encoding check passed
  - GitGuardian scan found no secrets

## Next Step

- Run a manual WPF HUD smoke test.
- Publish or commit only after user confirmation.

## Operational Notes

- Do not store secrets, tokens, credentials, or private machine data in this file.
