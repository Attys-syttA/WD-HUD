# Architecture

Last updated: 2026-07-04

## Layers

- `WdHud.Contracts`: DTOs and interfaces only.
- `WdHud.Core`: formatting, normalization, GPU selection, and settings validation.
- `WdHud.Infrastructure`: local Windows and hardware integrations.
- `WdHud.App`: WPF UI and view model.

## Data Flow

1. `LibreHardwareMonitorMetricsProvider` reads local hardware sensors.
2. `GpuSelectionPolicy` chooses one GPU for display.
3. `HudMetricsNormalizer` clamps percentage values.
4. `HudViewModel` formats values for the UI.
5. `MainWindow` displays a compact always-on-top panel.

## Sensor Refresh Cycle

- The HUD refreshes every 1 second.
- Missing sensor values remain `null`.
- UI renders missing values as `N/A`.
- A missing GPU sensor must not crash the app.

## GPU Selection Rule

Supported modes:

- `Auto`
- `DiscretePreferred`
- `IntegratedPreferred`
- `FirstAvailable`

MVP default:

- prefer discrete GPU when available, especially NVIDIA
- fallback to integrated GPU
- show only one GPU at a time

## Settings

Settings are stored locally:

- `%LocalAppData%\WD-HUD\settings.json`

Rules:

- create defaults when missing
- back up corrupt JSON and recreate defaults
- no roaming sync
- no remote export

## UI Lifecycle

1. `MainWindow` creates app services.
2. Settings load on window load.
3. Initial position is restored or defaults to top-right.
4. Metrics refresh starts.
5. Position and opacity save on close.
