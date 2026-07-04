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
- [x] Multiple-GPU selection is handled by the core policy tests.
- [x] Missing sensor values do not crash the app.
- [ ] Settings are saved and restored.
- [x] Always-on-top works.
- [ ] Click-through can be enabled.
- [ ] Outbound network block script can create a rule.
- [ ] Verify script can detect the rule.
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
- Remaining follow-up: find a real in-app CPU temperature source for this hardware. Since HWiNFO64 sees the data but shared memory is time-limited/manual in the free version, the preferred MVP route is newer/different LibreHardwareMonitor support for this board/CPU combination.
