# WD-HUD Project State

## Current State

- Date: 2026-07-04
- Repo: `E:\codex_works\WD_HUD`
- Phase: MVP scaffold through first visible HUD smoke test
- Status: solution, projects, contracts, core logic, LibreHardwareMonitor metrics provider, null-safe snapshot normalization, WPF HUD shell, tests, docs, scripts, CI/workflows, inventory, first manual HUD smoke test, GPU temperature/dGPU selection bugfix, invalid CPU temperature filtering, explicit elevated runtime manifest, first status-color HUD values, startup registration, tray minimize/restore, and draggable HUD behavior are complete.

## Open Items

- .NET 10 SDK is installed. Local validation works through `scripts/local-build.ps1`; the script still prepends `C:\Program Files\dotnet` if the current shell PATH has not refreshed yet.
- Manual smoke test confirmed the visible WPF HUD starts, stays topmost, renders the expected fields, updates values, and tolerates unavailable sensors without crashing.
- GPU temperature identification bug is fixed: `AMD Radeon(TM) Graphics` is now classified as integrated, so Auto mode selects the discrete `NVIDIA GeForce RTX 5080` when both GPUs are present.
- CPU temperature no longer displays a false `0°C`; invalid zero temperature readings are filtered and shown as `N/A`.
- CPU temperature is available in-app when WD-HUD runs elevated through its manifest; non-elevated probes could not access the usable CPU sensor path on this machine.
- HWiNFO64 Sensors view confirms the hardware does expose real CPU temperatures: `CPU (Tctl/Tdie)` around `64.1 °C`, CPU package/case around `61.8 °C`, CCD/IOD/core temperatures also visible.
- HWiNFO64 shared-memory export is not accepted as an MVP baseline runtime dependency because the free version is limited to 12 hours and requires manual reactivation.
- LibreHardwareMonitor was switched to its `IVisitor`/`Accept` refresh pattern; this kept GPU readings working but did not make a real CPU temperature available on this machine.
- Elevated WD-HUD provider probe confirmed the root cause: with administrator rights, the same provider reads CPU temperature (`CpuTemperatureC` around `66.75 °C`), GPU temperature, and the NVIDIA dGPU correctly.
- WD-HUD now requests administrator rights through `src/WdHud.App/app.manifest`; this is accepted for the single-user target machine and is limited to local hardware sensor access.
- The elevated runtime decision does not relax the no-network, no-telemetry, no-updater, no-cloud, no-remote-config, and no-WebView baseline.
- Outbound Windows Defender Firewall block rule was created and verified for the current Release `WdHud.App.exe` path.
- HUD metric values now use status colors: cool blue, optimal green, warm orange, and critical red for CPU/RAM/GPU load and CPU/GPU temperature.
- `StartWithWindows` now defaults to enabled and registers the current `WdHud.App.exe` under the current user's Run key.
- Startup registration points directly at the `WinExe` app executable, not at `cmd`, PowerShell, or a wrapper script.
- The HUD can be dragged, minimized to the notification area, restored from the tray icon, and exited from the tray menu.
- GitHub Actions inventory failure after commit `1e5bb40` was diagnosed as non-deterministic inventory checking on the CI runner; inventory generation now uses `git ls-files`, and `scripts/check-inventory.ps1` compares normalized inventory objects instead of rewriting and byte-comparing JSON text.
- Remote is configured as `https://github.com/Attys-syttA/WD-HUD.git`.

## Recent Decisions

- Architecture follows separate `Contracts`, `Core`, `Infrastructure`, and `App` projects.
- `LibreHardwareMonitorLib` is the only planned runtime hardware-monitoring dependency.
- HWiNFO64 may be used as a manual diagnostic cross-check, but not as required MVP runtime input while shared-memory export has a 12-hour/manual-reactivation limit in the free version.
- `WindowsEdgeLight` and `glassmorphism-wpf` are reference-only sources.
- The repository starts with tracked `STATE.md`, `docs/CHANGELOG.dev.md`, task folders, and full file inventory discipline.
- Target framework is `net10.0` / `net10.0-windows`, matching the installed local SDK/runtime.
- Current version is `0.1.00007`; the patch5 bump was needed because startup and window behavior changed.
- GitHub Actions are split into `ci.yml`, `build-check.yml`, `security-scan.yml`, `inventory-check.yml`, `encoding-check.yml`, and `secret-scan.yml`.
- The first metrics provider is `LibreHardwareMonitorMetricsProvider`; it opens LibreHardwareMonitor lazily and returns `HudMetricsSnapshot.Empty(...)` if local sensor reading fails.
- `HudMetricsNormalizer` clamps percent values and removes non-finite sensor values before the HUD formats them.
- Manual smoke test on the target Windows machine found both `NVIDIA GeForce RTX 5080` and `AMD Radeon(TM) Graphics`; provider snapshot verification now selects the NVIDIA dGPU and reads GPU temperature.
- Repository inventory is generated and checked from tracked Git files, not from a raw filesystem crawl, to keep local and GitHub Actions checks aligned.

## Latest Validation

- Commands:
  - `pwsh -File .\scripts\update-inventory.ps1`
  - `pwsh -File .\scripts\local-build.ps1`
  - `ggshield secret scan path --recursive --yes --use-gitignore .`
- Result:
  - build succeeded
  - tests passed: 26 passed, 0 failed
  - security scan passed
  - inventory check passed
  - encoding check passed
  - GitGuardian scan found no secrets
  - app manifest build succeeded with administrator execution level
  - manifest-built Release `WdHud.App.exe` started successfully as a visible runtime smoke test
  - outbound Windows Defender Firewall block rule was created and verified for the current Release executable
  - user-visible elevated HUD smoke test confirmed `CPU °C 66°C` and `GPU °C 40°C`
  - status-color policy tests passed for load and temperature thresholds
  - colored-HUD visual launch was attempted, but the elevated start was canceled by the user before the window opened
  - startup/tray/draggable behavior built successfully
  - fresh Release `WdHud.App.exe` started successfully after the startup/tray changes
  - HKCU Run value `WD-HUD` was verified and points directly to the current Release `WdHud.App.exe`
  - user-visible smoke test confirmed drag movement and tray minimize/restore work
  - user-visible smoke test confirmed colored HUD values render as expected
  - CI inventory fix validation passed after stopping the locally running HUD process: build, 26 tests, security scan, normalized inventory check, encoding check, GitGuardian, and `git diff --check`
- Manual smoke test:
  - Release `WdHud.App.exe` started and remained running.
  - Visible `WD-HUD` top-level WPF window was detected.
  - UI Automation observed visible fields: time, CPU, RAM, GPU, CPU temperature, GPU temperature.
  - Metrics updated while running; unavailable sensor values rendered as `0°C` or `N/A`.
  - Window style probe reported `Topmost=True` and `Layered=True`.
  - User-provided screenshot confirmed the visible translucent HUD panel.
- GPU fix validation:
  - Raw LibreHardwareMonitor probe showed `NVIDIA GeForce RTX 5080` with `GPU Core` temperature.
  - Provider snapshot after the fix selected `NVIDIA GeForce RTX 5080`, reported `IsGpuDiscrete=True`, and returned GPU temperature.
  - Visible HUD re-check showed `GPU °C 44°C` instead of `N/A`.
- CPU temperature fix validation:
    - Raw LibreHardwareMonitor probe showed `Core (Tctl/Tdie) = 0`, which is invalid for a running CPU.
    - Windows ACPI/WMI thermal probes did not return a usable CPU temperature.
    - LHM `IVisitor`/`Accept` refresh pattern still produced `CpuTemperatureC = null` after invalid zero filtering.
    - HWiNFO64 Sensors view showed real CPU telemetry, including `CPU (Tctl/Tdie)` around `64.1 °C`, proving the sensor exists but is not exposed correctly through the current LibreHardwareMonitor path.
    - Elevated WD-HUD provider probe returned `CpuTemperatureC = 66.75 °C`, proving administrator rights unlock the needed sensor access.
    - App manifest now requests administrator rights so normal app launch uses the same sensor-access mode.
    - Visible HUD re-check showed `CPU °C N/A` instead of false `0°C`, while `GPU °C` still showed a real value.
    - User-visible elevated HUD smoke test confirmed the final app now renders CPU temperature as `CPU °C 66°C` and GPU temperature as `GPU °C 40°C`.

## Next Step

- Commit and push the CI inventory determinism fix, then verify GitHub Actions.
- Continue to publish or commit only after user confirmation.

## Operational Notes

- Do not store secrets, tokens, credentials, or private machine data in this file.
