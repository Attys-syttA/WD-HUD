# CHANGELOG.dev

## 2026-07-04 - Invalid CPU temperature filtering

- Goal: fix the manual smoke-test bug where CPU temperature displayed as false `0°C`.
- Modified files: `Directory.Build.props`, `STATE.md`, `docs/acceptance-checklist.md`, `docs/CHANGELOG.dev.md`, `src/WdHud.Infrastructure/LibreHardwareMonitorMetricsProvider.cs`, `tests/WdHud.Tests/LibreHardwareMonitorMetricsProviderTests.cs`.
- Commands/probes run:
  - raw LibreHardwareMonitor CPU/motherboard/controller temperature probe
  - Windows ACPI/WMI thermal probe
  - provider snapshot probe
  - visible HUD UI Automation re-check
  - `pwsh -File .\scripts\local-build.ps1`
- Result: invalid zero temperature readings are filtered, the provider enables motherboard/controller sources and recursively checks sub-sensors, and the visible HUD now shows `CPU °C N/A` instead of false `0°C`. Version bumped to `0.1.00004` because runtime temperature handling changed.
- Follow-up: this machine still needs a real CPU temperature source; LibreHardwareMonitor currently reports the CPU `Core (Tctl/Tdie)` temperature as `0`, and Windows ACPI/WMI did not provide a usable fallback.

## 2026-07-04 - LibreHardwareMonitor visitor refresh probe

- Goal: test the LibreHardwareMonitor `IVisitor`/`Accept` refresh pattern suggested by the planning discussion.
- Modified files: `src/WdHud.Infrastructure/LibreHardwareMonitorMetricsProvider.cs`, `STATE.md`, `docs/acceptance-checklist.md`, `docs/CHANGELOG.dev.md`.
- Commands/probes run:
  - provider snapshot probe
  - `pwsh -File .\scripts\local-build.ps1`
- Result: the provider now refreshes the full LibreHardwareMonitor tree through `computer.Accept(UpdateVisitor.Instance)`. GPU readings remained healthy, selecting `NVIDIA GeForce RTX 5080` and reading GPU temperature. CPU temperature still remained unavailable after invalid zero filtering, so the issue is not just manual `Update()` traversal order.
- Follow-up: find a real CPU temperature source for this specific hardware.

## 2026-07-04 - HWiNFO CPU sensor evidence

- Goal: record external sensor evidence after installing HWiNFO64.
- Modified files: `STATE.md`, `docs/acceptance-checklist.md`, `docs/CHANGELOG.dev.md`.
- Evidence: HWiNFO64 Sensors view shows real CPU temperatures on the target machine, including `CPU (Tctl/Tdie)` around `64.1 °C`, CPU package/case around `61.8 °C`, and visible CCD/IOD/core temperatures.
- Result: the hardware and cooling telemetry exist; the remaining app issue is that the current LibreHardwareMonitor path does not expose the same CPU Enhanced sensor values on this ASRock X870E Taichi / Ryzen 9 9950X3D setup.
- Follow-up: do not use HWiNFO shared memory as an MVP baseline runtime source while the free version has a 12-hour/manual-reactivation limit. Prefer a newer/different LibreHardwareMonitor-compatible path; HWiNFO remains a manual diagnostic cross-check or later optional integration. Do not add runtime internet, telemetry, cloud, or updater behavior.

## 2026-07-04 - HWiNFO shared-memory runtime decision

- Goal: record the runtime-source decision after checking HWiNFO64 behavior and licensing/limit constraints.
- Modified files: `STATE.md`, `docs/acceptance-checklist.md`, `docs/CHANGELOG.dev.md`.
- Decision: HWiNFO64 shared-memory export is not an MVP baseline dependency because the free version is limited to 12 hours and requires manual reactivation.
- Result: WD-HUD keeps `LibreHardwareMonitorLib` as the only planned runtime hardware-monitoring dependency. HWiNFO64 can be used for manual diagnostics or a future optional provider, but the HUD must not depend on it for always-on operation.
- Follow-up: continue looking for a reliable LibreHardwareMonitor-compatible CPU temperature source for the ASRock X870E Taichi / Ryzen 9 9950X3D setup.

## 2026-07-04 - GPU temperature selection fix

- Goal: fix the manual smoke-test bug where GPU temperature displayed as `N/A` despite an available NVIDIA dGPU temperature sensor.
- Modified files: `Directory.Build.props`, `STATE.md`, `docs/acceptance-checklist.md`, `docs/CHANGELOG.dev.md`, `src/WdHud.Infrastructure/LibreHardwareMonitorMetricsProvider.cs`, `src/WdHud.Infrastructure/WdHud.Infrastructure.csproj`, `tests/WdHud.Tests/LibreHardwareMonitorMetricsProviderTests.cs`, `repo-file-inventory.json`.
- Commands/probes run:
  - raw LibreHardwareMonitor GPU sensor probe
  - provider snapshot probe
  - `pwsh -File .\scripts\local-build.ps1`
- Result: `AMD Radeon(TM) Graphics` is now classified as integrated, Auto mode selects `NVIDIA GeForce RTX 5080`, and the provider snapshot returns GPU temperature. A visible HUD re-check showed `GPU °C 44°C` instead of `N/A`. Tests now include the `(TM)` AMD integrated-name case. Version bumped to `0.1.00003` because runtime GPU selection behavior changed.
- Follow-up: CPU temperature still rendered as `0°C` in the smoke test and should be investigated separately.

## 2026-07-04 - Manual WPF HUD smoke test

- Goal: run the next pending validation phase from `STATE.md` without adding features.
- Modified files: `STATE.md`, `docs/acceptance-checklist.md`, `docs/CHANGELOG.dev.md`.
- Commands/probes run:
  - `pwsh -File .\scripts\local-build.ps1`
  - Release `WdHud.App.exe` launch
  - Win32 top-level window enumeration
  - UI Automation text probe
  - Win32 extended-window-style probe
  - `Get-CimInstance Win32_VideoController`
- Result: the app started, a visible topmost layered `WD-HUD` window was detected, expected HUD fields rendered, values updated while running, and missing/unavailable sensors rendered without crashing. The user-provided screenshot confirmed the visible glass-like HUD panel.
- Follow-up: the target machine has both NVIDIA RTX 5080 and AMD integrated graphics, and automated tests cover discrete-preferred policy; the current HUD does not expose selected GPU name, so direct manual runtime confirmation of selected GPU identity remains limited.

## 2026-07-04 - First metrics snapshot hardening

- Goal: align README/STATE with the installed .NET SDK and continue the next MVP phase without adding extra features.
- Modified files: `README.md`, `CODEX_AGENT_PLAN.md`, `STATE.md`, `docs/CHANGELOG.dev.md`, `Directory.Build.props`, `.gitignore`, `scripts/update-inventory.ps1`, `scripts/check-encoding.ps1`, `src/WdHud.Core/HudMetricsNormalizer.cs`, `src/WdHud.Infrastructure/LibreHardwareMonitorMetricsProvider.cs`, `tests/WdHud.Tests/HudMetricsNormalizerTests.cs`, `repo-file-inventory.json`.
- Commands run:
  - `pwsh -File .\scripts\update-inventory.ps1`
  - `pwsh -File .\scripts\local-build.ps1`
  - `ggshield secret scan path --recursive --yes --use-gitignore .`
- Result: README no longer says `dotnet` is blocked; `LibreHardwareMonitorMetricsProvider` opens sensors lazily and returns an empty snapshot on local sensor-read failure; `HudMetricsNormalizer` removes NaN/Infinity values. Version bumped to `0.1.00002` because runtime behavior changed.
- Open follow-up: manual visible WPF HUD smoke test.

## 2026-07-04 - Bootstrap start

- Goal: start the `WD-HUD` repository from the user-provided `CODEX_AGENT_PLAN.md`.
- Modified files: `AGENTS.md`, `CODEX_AGENT_PLAN.md`, `STATE.md`, `README.md`, `WD-HUD.sln`, `.editorconfig`, `.gitattributes`, `.gitignore`, `.env.example`, `.github/workflows/ci.yml`, `Directory.Build.props`, `Directory.Packages.props`, `docs/*`, `scripts/*`, `src/*`, `tests/*`, `repo-file-inventory.json`.
- Commands run:
  - `git init -b main`
  - `git remote add origin https://github.com/Attys-syttA/WD-HUD.git`
  - `dotnet --info`
  - `where.exe dotnet`
  - `pwsh -File .\scripts\update-inventory.ps1`
  - `pwsh -File .\scripts\local-build.ps1`
- Result: local git repository and remote were initialized; solution/project scaffold, MVP app skeleton, docs, tests, scripts, CI, and inventory were created. Build succeeded, 8 tests passed, security scan passed, inventory check passed.
- Open follow-up: manual WPF HUD smoke test, optional firewall rule test after publish, commit/push only with user confirmation.

## 2026-07-04 - .NET target adjustment

- Goal: make validation match the installed local .NET runtime.
- Modified files: `src/*/*.csproj`, `tests/WdHud.Tests/WdHud.Tests.csproj`, `.github/workflows/ci.yml`, `README.md`, `STATE.md`.
- Commands run:
  - `C:\Program Files\dotnet\dotnet.exe --info`
  - `pwsh -File .\scripts\local-build.ps1`
- Result: project target changed to `net10.0` / `net10.0-windows`; local build/test works with the installed .NET 10 SDK/runtime.
- Open follow-up: none for build validation.

## 2026-07-04 - GitHub Actions baseline

- Goal: add the usual justified GitHub Actions checks for this new C# WPF repository.
- Modified files: `.github/workflows/ci.yml`, `.github/workflows/build-check.yml`, `.github/workflows/security-scan.yml`, `.github/workflows/inventory-check.yml`, `.github/workflows/encoding-check.yml`, `.github/workflows/secret-scan.yml`, `scripts/check-encoding.ps1`, `scripts/local-build.ps1`, `scripts/update-inventory.ps1`, `README.md`, `AGENTS.md`, `STATE.md`, `repo-file-inventory.json`.
- Commands run:
  - `pwsh -File .\scripts\update-inventory.ps1`
  - `pwsh -File .\scripts\local-build.ps1`
  - `ggshield secret scan path --recursive --yes --use-gitignore .`
- Result: build succeeded, 8 tests passed, security scan passed, inventory check passed, encoding check passed, GitGuardian found no secrets.
- Open follow-up: GitHub-side Actions can run after the first commit/push.
