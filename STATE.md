# WD-HUD Project State

## Current State

- Date: 2026-07-04
- Repo: `E:\codex_works\WD_HUD`
- Phase: MVP scaffold through first visible HUD smoke test
- Status: solution, projects, contracts, core logic, LibreHardwareMonitor metrics provider, null-safe snapshot normalization, WPF HUD shell, tests, docs, scripts, CI/workflows, inventory, first manual HUD smoke test, GPU temperature/dGPU selection bugfix, and invalid CPU temperature filtering are complete.

## Open Items

- .NET 10 SDK is installed. Local validation works through `scripts/local-build.ps1`; the script still prepends `C:\Program Files\dotnet` if the current shell PATH has not refreshed yet.
- Manual smoke test confirmed the visible WPF HUD starts, stays topmost, renders the expected fields, updates values, and tolerates unavailable sensors without crashing.
- GPU temperature identification bug is fixed: `AMD Radeon(TM) Graphics` is now classified as integrated, so Auto mode selects the discrete `NVIDIA GeForce RTX 5080` when both GPUs are present.
- CPU temperature no longer displays a false `0°C`; invalid zero temperature readings are filtered and shown as `N/A`.
- Actual CPU temperature still needs a usable in-app sensor source on this machine. LibreHardwareMonitor currently reports `Core (Tctl/Tdie) = 0`, and Windows ACPI/WMI thermal probes did not return a usable CPU temperature.
- HWiNFO64 Sensors view confirms the hardware does expose real CPU temperatures: `CPU (Tctl/Tdie)` around `64.1 °C`, CPU package/case around `61.8 °C`, CCD/IOD/core temperatures also visible.
- HWiNFO64 shared-memory export is not accepted as an MVP baseline runtime dependency because the free version is limited to 12 hours and requires manual reactivation.
- LibreHardwareMonitor was switched to its `IVisitor`/`Accept` refresh pattern; this kept GPU readings working but did not make a real CPU temperature available on this machine.
- Firewall rule creation was not executed because no published executable path was requested for local firewall changes.
- Remote is configured as `https://github.com/Attys-syttA/WD-HUD.git`.

## Recent Decisions

- Architecture follows separate `Contracts`, `Core`, `Infrastructure`, and `App` projects.
- `LibreHardwareMonitorLib` is the only planned runtime hardware-monitoring dependency.
- HWiNFO64 may be used as a manual diagnostic cross-check, but not as required MVP runtime input while shared-memory export has a 12-hour/manual-reactivation limit in the free version.
- `WindowsEdgeLight` and `glassmorphism-wpf` are reference-only sources.
- The repository starts with tracked `STATE.md`, `docs/CHANGELOG.dev.md`, task folders, and full file inventory discipline.
- Target framework is `net10.0` / `net10.0-windows`, matching the installed local SDK/runtime.
- Current version is `0.1.00004`; the patch5 bump was needed for invalid CPU temperature filtering after the GPU temperature/dGPU selection bugfix.
- GitHub Actions are split into `ci.yml`, `build-check.yml`, `security-scan.yml`, `inventory-check.yml`, `encoding-check.yml`, and `secret-scan.yml`.
- The first metrics provider is `LibreHardwareMonitorMetricsProvider`; it opens LibreHardwareMonitor lazily and returns `HudMetricsSnapshot.Empty(...)` if local sensor reading fails.
- `HudMetricsNormalizer` clamps percent values and removes non-finite sensor values before the HUD formats them.
- Manual smoke test on the target Windows machine found both `NVIDIA GeForce RTX 5080` and `AMD Radeon(TM) Graphics`; provider snapshot verification now selects the NVIDIA dGPU and reads GPU temperature.

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
    - Visible HUD re-check showed `CPU °C N/A` instead of false `0°C`, while `GPU °C` still showed a real value.

## Next Step

- Investigate a real CPU temperature source separately. HWiNFO64 can see the data, but its free shared-memory export is time-limited/manual, so the preferred MVP route remains a newer/different LibreHardwareMonitor-compatible path for this ASRock X870E Taichi / Ryzen 9 9950X3D setup.
- Continue to publish or commit only after user confirmation.

## Operational Notes

- Do not store secrets, tokens, credentials, or private machine data in this file.
