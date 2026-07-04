# Threat Surface

Last updated: 2026-07-04

## Attack Surface

- Local hardware sensor library.
- Elevated local process token for hardware sensor access.
- Local JSON settings file.
- Current-user startup registry entry.
- Notification-area icon and restore/exit menu.
- Optional Windows Defender Firewall rule scripts.
- WPF window interop for click-through behavior.

## Mitigations

- No runtime network feature is designed into the app.
- Security scan blocks common network, updater, telemetry, and analytics patterns.
- Administrator elevation is limited to local sensor access and does not permit any network/update/cloud feature in the application design.
- Settings are local-only.
- Startup registration is current-user scoped.
- Startup uses the app executable directly, not a command/script wrapper.
- Firewall scripts require an explicit executable path.
- Third-party source reuse is limited to `LibreHardwareMonitorLib`; other projects are reference-only.

## Remaining Risk

- Hardware sensor availability and accuracy depend on drivers and motherboard/GPU support.
- Low-level sensor reads may behave differently across machines.
- Running elevated increases the local trust boundary, so the codebase must stay small, offline-only, and free of updater/network features.
- Firewall rule creation depends on local Windows policy and permissions.
- The static security scan is conservative but not a formal proof.

## Not Guaranteed

- This app does not guarantee forensic-grade hardware readings.
- This app does not guarantee sensor availability.
- This app does not guarantee OS-level isolation.
- This app does not replace endpoint security controls.
