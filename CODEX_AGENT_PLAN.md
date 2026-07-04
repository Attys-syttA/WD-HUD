# WD-HUD - Codex Agent Plan

Repository: `Attys-syttA/WD-HUD`

This plan was imported from `C:\Users\gép\Downloads\v2.txt` and is the active implementation guide.

## Current Status

- Status: Bootstrap through initial MVP scaffold completed locally.
- Current repo relation: local repository initialized in `E:\codex_works\WD_HUD`.
- Completed parts: local git repository and remote configured; solution, projects, docs, scripts, tests, CI, and inventory created; build/test/security/inventory validation passed.
- Open parts: manual visible HUD smoke test and firewall rule test after publish.

## 1. Goal

Build a Windows desktop HUD application with the required MVP features:

- time: `HH:mm`
- CPU usage %
- RAM usage %
- GPU usage %
- CPU temperature C
- GPU temperature C
- always-on-top
- translucent glassmorphism-style panel
- saved position
- saved opacity
- optional click-through
- optional startup with Windows

This is not a game overlay. It is a normal Windows desktop HUD.

## 2. Fixed Technical Decisions

Required:

- language: `C#`
- platform: `.NET`
- UI: `WPF`
- architecture: separate `Contracts`, `Core`, `Infrastructure`, `App`
- sensor data: `LibreHardwareMonitor`
- offline-first operation
- no network communication
- no telemetry
- no analytics
- no auto-update
- no cloud sync
- no remote config
- no embedded browser / WebView

Not allowed:

- Electron
- MAUI
- switching to WinUI without explicit approval
- full fork-and-rename of an existing third-party HUD app
- feature creep

## 3. Implementation Phases

1. Phase 0 - Bootstrap
2. Phase 1 - Source audit
3. Phase 2 - Security baseline
4. Phase 3 - Contracts + Core
5. Phase 4 - Sensor provider
6. Phase 5 - HUD UI
7. Phase 6 - Settings
8. Phase 7 - Security scripts
9. Phase 8 - CI
10. Phase 9 - Final documentation

## 4. Commit Order

1. `chore: bootstrap solution structure`
2. `docs: add source audit and security baseline`
3. `feat(core): add contracts formatting and policies`
4. `feat(infra): add metrics provider using libre hardware monitor`
5. `feat(app): implement minimal glass hud window`
6. `feat(settings): add local settings persistence`
7. `feat(security): add code scan and network block scripts`
8. `docs: finalize architecture threat model and readme`
9. `ci: add build test and security workflow`

## 5. Completion Rule

A phase is only complete when its stop condition is met and validation is documented in `STATE.md` and `docs/CHANGELOG.dev.md`.
