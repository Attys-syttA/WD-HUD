# CHANGELOG.dev

## 2026-07-04 - CI inventory determinism fix

- Goal: fix the GitHub Actions `Inventory Check` and aggregate `CI` failures on commit `1e5bb40`.
- Root cause: `scripts/update-inventory.ps1` generated inventory from a raw filesystem crawl, and `scripts/check-inventory.ps1` rewrote then byte-compared JSON text. GitHub Actions clean checkout and local runtime/build state can differ, and JSON text formatting can vary, so the CI runner failed with `repo-file-inventory.json was stale and has been updated`.
- Modified files: `scripts/update-inventory.ps1`, `scripts/check-inventory.ps1`, `STATE.md`, `docs/CHANGELOG.dev.md`.
- Result: inventory generation now uses `git ls-files`, and inventory checking compares normalized `path|role|tracked` entries without rewriting `repo-file-inventory.json`.
- Validation: after stopping the locally running HUD process that locked Release DLLs, `pwsh -File .\scripts\update-inventory.ps1`, `pwsh -File .\scripts\check-inventory.ps1`, `pwsh -File .\scripts\local-build.ps1`, `pwsh -File .\scripts\check-encoding.ps1`, `ggshield secret scan path --recursive --yes --use-gitignore .`, and `git diff --check` passed. Tests passed: 26.
- Version bump: not needed; this is CI/tooling behavior only, not application runtime behavior.

## 2026-07-04 - Startup, tray minimize, and draggable HUD

- Goal: make WD-HUD behave like a small desktop utility that can start with Windows, stay out of the taskbar, and be moved by the user.
- Modified files: `Directory.Build.props`, `README.md`, `STATE.md`, `docs/acceptance-checklist.md`, `docs/CHANGELOG.dev.md`, `docs/security-baseline.md`, `docs/threat-surface.md`, `src/WdHud.App/App.xaml.cs`, `src/WdHud.App/MainWindow.xaml`, `src/WdHud.App/MainWindow.xaml.cs`, `src/WdHud.App/WdHud.App.csproj`, `src/WdHud.Contracts/HudSettings.cs`, `tests/WdHud.Tests/HudSettingsValidatorTests.cs`, `repo-file-inventory.json`.
- Result: `StartWithWindows` defaults to enabled; the app registers the current `WdHud.App.exe` under the current user's Run key; the HUD can be dragged; the corner button minimizes it to the notification area; the tray icon can restore or exit the app.
- Startup behavior: the registration points directly at the `WinExe` executable, not at `cmd`, PowerShell, or a wrapper script.
- Validation: `pwsh -File .\scripts\update-inventory.ps1`, `pwsh -File .\scripts\local-build.ps1`, `ggshield secret scan path --recursive --yes --use-gitignore .`, and `git diff --check` passed. Tests passed: 26. Fresh Release `WdHud.App.exe` started successfully, HKCU Run was verified to point directly to the current Release executable, and user-visible smoke test confirmed drag movement plus tray minimize/restore. Version bumped to `0.1.00007` because startup and window behavior changed.

## 2026-07-04 - HUD status colors

- Goal: color existing HUD values by status without adding new metrics or network behavior.
- Modified files: `Directory.Build.props`, `STATE.md`, `docs/acceptance-checklist.md`, `docs/CHANGELOG.dev.md`, `src/WdHud.App/HudViewModel.cs`, `src/WdHud.App/MainWindow.xaml`, `src/WdHud.Core/HudMetricSeverity.cs`, `src/WdHud.Core/HudMetricStatusPolicy.cs`, `tests/WdHud.Tests/HudMetricStatusPolicyTests.cs`, `repo-file-inventory.json`.
- Result: CPU/RAM/GPU load and CPU/GPU temperature values now use blue for cool, green for optimal, orange for warm, and red for critical. Unknown values remain white.
- Thresholds: load `<20` blue, `20-69` green, `70-89` orange, `90+` red; temperature `<45°C` blue, `45-74°C` green, `75-84°C` orange, `85°C+` red.
- Validation: `pwsh -File .\scripts\update-inventory.ps1`, `pwsh -File .\scripts\local-build.ps1`, `ggshield secret scan path --recursive --yes --use-gitignore .`, and `git diff --check` passed. Tests passed: 25. Version bumped to `0.1.00006` because user-visible HUD behavior changed.
- Visual follow-up: user-visible smoke test confirmed the colored HUD values render on the desktop.

## 2026-07-04 - Admin elevation runtime decision

- Goal: make the accepted runtime mode explicit after proving that administrator rights unlock CPU temperature access on the target machine.
- Modified files: `Directory.Build.props`, `README.md`, `STATE.md`, `docs/acceptance-checklist.md`, `docs/CHANGELOG.dev.md`, `docs/security-baseline.md`, `docs/threat-surface.md`, `src/WdHud.App/WdHud.App.csproj`, `src/WdHud.App/app.manifest`, `repo-file-inventory.json`.
- Decision: WD-HUD now requests administrator rights through its application manifest. The elevation is only for local LibreHardwareMonitor sensor access.
- Result: version bumped to `0.1.00005` because runtime security behavior changed.
- Validation: `pwsh -File .\scripts\update-inventory.ps1`, `pwsh -File .\scripts\local-build.ps1`, `ggshield secret scan path --recursive --yes --use-gitignore .`, and `git diff --check` passed. Tests passed: 15. Manifest-built Release `WdHud.App.exe` started successfully, and user-visible elevated HUD smoke test confirmed `CPU °C 66°C` and `GPU °C 40°C`.
- Network hardening: `scripts/block-network.ps1` was run elevated for the current Release `WdHud.App.exe`, and `scripts/verify-no-network.ps1` confirmed the outbound block rule exists.
- Guardrail: the no-network, no-telemetry, no-updater, no-cloud, no-remote-config, and no-WebView baseline remains unchanged.

## 2026-07-04 - Elevated CPU temperature probe

- Goal: verify whether administrator rights change LibreHardwareMonitor CPU temperature availability inside WD-HUD.
- Modified files: `STATE.md`, `docs/acceptance-checklist.md`, `docs/CHANGELOG.dev.md`.
- Commands/probes run:
  - elevated temporary PowerShell provider probe loading the WD-HUD Release DLLs
- Result: with administrator rights, the same WD-HUD provider returned `CpuTemperatureC = 66.75 °C`, `GpuTemperatureC = 39.81 °C`, and `SelectedGpuName = NVIDIA GeForce RTX 5080`.
- Decision impact: the CPU temperature issue is now confirmed as a permissions/driver-access problem for non-elevated runs, not a missing hardware sensor or mismatched LibreHardwareMonitor DLL. Normal non-elevated runtime should keep the safe `N/A` fallback unless an explicit elevated mode is chosen.

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
