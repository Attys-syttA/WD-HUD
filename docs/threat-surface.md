# Threat Surface

Last updated: 2026-07-04

## Attack Surface

- Local hardware sensor library.
- Local JSON settings file.
- Current-user startup registry entry.
- Optional Windows Defender Firewall rule scripts.
- WPF window interop for click-through behavior.

## Mitigations

- No runtime network feature is designed into the app.
- Security scan blocks common network, updater, telemetry, and analytics patterns.
- Settings are local-only.
- Startup registration is current-user scoped.
- Firewall scripts require an explicit executable path.
- Third-party source reuse is limited to `LibreHardwareMonitorLib`; other projects are reference-only.

## Remaining Risk

- Hardware sensor availability and accuracy depend on drivers and motherboard/GPU support.
- Low-level sensor reads may behave differently across machines.
- Firewall rule creation depends on local Windows policy and permissions.
- The static security scan is conservative but not a formal proof.

## Not Guaranteed

- This app does not guarantee forensic-grade hardware readings.
- This app does not guarantee sensor availability.
- This app does not guarantee OS-level isolation.
- This app does not replace endpoint security controls.
