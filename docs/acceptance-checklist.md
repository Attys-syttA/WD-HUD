# Acceptance Checklist

Last updated: 2026-07-04

- [x] Build succeeds.
- [x] Tests succeed.
- [x] Security scan succeeds.
- [x] App starts.
- [x] HUD is visible.
- [x] HUD is glass-like and translucent.
- [x] Time is visible.
- [x] CPU usage is visible.
- [x] RAM usage is visible.
- [x] GPU value is a number or `N/A`.
- [x] CPU temperature is a number or `N/A`.
- [x] GPU temperature is detected for the NVIDIA dGPU on the target machine.
- [x] Elevated runtime can read CPU temperature on the target machine.
- [x] CPU/RAM/GPU load values have status colors.
- [x] CPU/GPU temperature values have status colors.
- [x] Multiple-GPU selection is handled by the core policy tests.
- [x] Missing sensor values do not crash the app.
- [ ] Settings are saved and restored.
- [x] Always-on-top works.
- [x] HUD can be dragged to a new screen position.
- [x] HUD can be minimized to the notification area.
- [x] Notification-area icon can restore the HUD.
- [x] Startup registration is current-user scoped and uses the app executable directly.
- [ ] Click-through can be enabled.
- [x] Outbound network block script can create a rule.
- [x] Verify script can detect the rule.
- [x] No updater exists.
- [x] No telemetry exists.
- [x] No analytics exists.
- [x] No unnecessary dependency is present.

## Manual Smoke Test - 2026-07-04

- Build/test/security/inventory/encoding validation passed before launch.
- The Release WPF executable started and kept running without crashing.
- A visible `WD-HUD` top-level window was present at the top-right of the desktop.
- UI Automation showed visible text fields for time, CPU, RAM, GPU, CPU temperature, and GPU temperature.
- Observed values updated while running; CPU changed from `67%` to `37%` in the automation sample.
- The user-provided screenshot confirmed the HUD rendered as a translucent glass-like panel with expected fields.
- Missing or unavailable sensor values were rendered as `0°C` or `N/A` and did not crash the app.
- Window extended style probe reported `Topmost=True` and `Layered=True`.
- Initial issue found: the visible HUD showed GPU temperature as `N/A` even though the machine has an NVIDIA dGPU with temperature sensors.
- Root cause: `AMD Radeon(TM) Graphics` was classified as discrete because the `(TM)` marker prevented the integrated AMD name match.
- Fix validation: provider snapshot now selects `NVIDIA GeForce RTX 5080`, reports `IsGpuDiscrete=True`, and reads GPU temperature.
- Visible HUD re-check confirmed `GPU °C 44°C`, so the user-visible GPU temperature bug is fixed.
- Separate follow-up: CPU temperature still rendered as `0°C` in the smoke test and should be investigated independently.

## CPU Temperature Follow-Up - 2026-07-04

- Initial issue found: the visible HUD showed `CPU °C 0°C`, which is not a valid running CPU temperature.
- Raw LibreHardwareMonitor probe showed only `Core (Tctl/Tdie) = 0` for the CPU temperature on this machine.
- Windows ACPI/WMI thermal probes did not return a usable CPU temperature fallback.
- Fix validation: invalid zero temperature readings are now filtered; visible HUD re-check showed `CPU °C N/A` and `GPU °C 42°C`.
- LHM `IVisitor`/`Accept` refresh pattern was tested; GPU readings still worked, but CPU temperature remained unavailable after invalid zero filtering.
- HWiNFO64 Sensors view confirmed real CPU telemetry is available on the machine: `CPU (Tctl/Tdie)` around `64.1 °C`, CPU package/case around `61.8 °C`, plus CCD/IOD/core temperatures.
- HWiNFO64 shared-memory export is not an MVP baseline runtime source because the free version is limited to 12 hours and requires manual reactivation.
- Elevated WD-HUD provider probe confirmed administrator rights unlock the CPU temperature: `CpuTemperatureC` around `66.75 °C`, with GPU temperature and NVIDIA dGPU selection still correct.
- Decision: WD-HUD now requests administrator rights through its app manifest so the same LibreHardwareMonitor provider can read CPU temperature on this machine.
- Constraint: the admin runtime decision does not relax the no-network, no-telemetry, no-updater, no-cloud, no-remote-config, and no-WebView baseline.
- Manifest-built Release `WdHud.App.exe` started successfully; user-visible elevated HUD smoke test confirmed `CPU °C 66°C` and `GPU °C 40°C`.
- Outbound Windows Defender Firewall block rule was created and verified for `src/WdHud.App/bin/Release/net10.0-windows/WdHud.App.exe`.

## HUD Status Colors - 2026-07-04

- Load values use blue below 20%, green from 20% to 69%, orange from 70% to 89%, and red from 90%.
- Temperature values use blue below 45°C, green from 45°C to 74°C, orange from 75°C to 84°C, and red from 85°C.
- Unknown values stay white and continue to render as `N/A`.
- Threshold logic is covered by `HudMetricStatusPolicyTests`.
- User-visible smoke test confirmed the colored HUD values render on the desktop.

## Window And Startup Behavior - 2026-07-04

- `StartWithWindows` defaults to enabled.
- On load, WD-HUD registers the current `WdHud.App.exe` under the current user's Run key.
- Startup registration points directly to the executable, not to `cmd`, PowerShell, or a wrapper script.
- Fresh Release startup registration was verified in HKCU Run and points directly to the current Release `WdHud.App.exe`.
- The HUD surface is draggable and saves its window position on exit.
- The corner button minimizes the HUD to the notification area.
- Closing the HUD window hides it to the notification area; the tray menu has `Open` and `Exit`.
- User-visible smoke test confirmed drag movement and tray minimize/restore work.
