# Source Audit

Last updated: 2026-07-04

## LibreHardwareMonitor

- Name: `LibreHardwareMonitor` / `LibreHardwareMonitorLib`
- Role: runtime hardware sensor provider.
- License: MPL-2.0.
- Maintenance: active public project with NuGet package availability.
- Potential risks:
  - hardware sensor availability differs by machine and driver
  - some sensors may require elevated permissions or may return no value
  - package brings low-level hardware access code into the app
- Forbidden parts:
  - full demo app import
  - unrelated UI
  - web server or remote access features, if encountered
- Allowed parts:
  - library dependency
  - local sensor traversal pattern
  - CPU, memory, GPU, and temperature reads
- Final decision: `runtime dependency`

## WindowsEdgeLight

- Name: `WindowsEdgeLight`
- Role: overlay behavior reference.
- License: repository license must be rechecked before any code-level reuse.
- Maintenance: public WPF overlay project with release artifacts.
- Potential risks:
  - includes automatic update behavior
  - release-check/download concepts conflict with WD-HUD security baseline
  - full app import would exceed MVP scope
- Forbidden parts:
  - updater
  - GitHub release check
  - download logic
  - full application fork
- Allowed parts:
  - conceptual reference for always-on-top behavior
  - conceptual reference for click-through behavior
  - conceptual reference for DPI and monitor positioning
- Final decision: `reference only`

## glassmorphism-wpf

- Name: `glassmorphism-wpf`
- Role: visual reference.
- License: MIT.
- Maintenance: public sample project.
- Potential risks:
  - advanced rendering dependencies could be unnecessary for MVP
  - visual sample code may not match this app's architecture
- Forbidden parts:
  - heavy rendering stack without explicit need
  - full sample app import
- Allowed parts:
  - visual inspiration for translucent panel
  - conceptual reference for border, highlight, and blur treatment
- Final decision: `reference only`

## Final Decision Summary

- `LibreHardwareMonitor`: runtime dependency through `LibreHardwareMonitorLib`.
- `WindowsEdgeLight`: reference only.
- `glassmorphism-wpf`: reference only.
