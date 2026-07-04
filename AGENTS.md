# Repository-local AGENTS.md for WD-HUD
Last reviewed: 2026-07-04
Applies only to this repository.

## 1. Scope

This local `AGENTS.md` applies only to:

- `E:\codex_works\WD_HUD`

The parent `E:\codex_works\AGENTS.md` remains authoritative for shared workspace rules. This file adds only WD-HUD-specific rules.

## 2. Repository Role

Treat this repository as the Windows desktop HUD application repository.

Runtime model:

- C#
- .NET
- WPF desktop app
- offline-first local Windows application

The app must not include telemetry, analytics, auto-update, cloud sync, remote config, embedded browser/WebView, or runtime internet access by design.

## 3. Project Memory

Required project memory:

- `STATE.md`
- `docs/CHANGELOG.dev.md`

Read `STATE.md` before continuing prior work. Update `STATE.md` and append to `docs/CHANGELOG.dev.md` after meaningful implementation, failed validation, security-relevant decisions, or release/build work.

## 4. Documentation And Task Discipline

Keep these aligned with real implementation state:

- `CODEX_AGENT_PLAN.md`
- `STATE.md`
- `docs/CHANGELOG.dev.md`
- `docs/codex-tasks/`
- `repo-file-inventory.json`

Active plans go under `docs/codex-tasks/plans/pending/active/`. Closed plans go under `docs/codex-tasks/plans/done/`.

## 5. Source And Dependency Rules

Allowed runtime external dependency:

- `LibreHardwareMonitorLib`, for local hardware sensor reading only.

Reference-only sources:

- `WindowsEdgeLight`
- `glassmorphism-wpf`

Do not copy full third-party applications into this repository. Do not adopt updater, release-check, download, telemetry, analytics, embedded browser, or cloud-related code.

## 6. Security And Sensitive Data

Never commit:

- `.env`
- secrets, tokens, credentials
- machine-local runtime settings
- private logs
- real user or machine data

Tracked examples, tests, and documentation must use synthetic data only.

Security baseline:

- no telemetry
- no analytics
- no updater
- no cloud sync
- no remote config
- no runtime network access by design
- Windows startup must be current-user scoped

## 7. Inventory Discipline

This repo uses a full tracked file inventory:

- `repo-file-inventory.json`
- `scripts/update-inventory.ps1`
- `scripts/check-inventory.ps1`

When adding, moving, deleting, or changing the role of tracked files, update the inventory and run the inventory check before considering the task complete.

## 8. Validation

Task closeout validation should include, when the .NET SDK is available:

- `pwsh -File scripts/local-build.ps1`
- `pwsh -File scripts/security-scan.ps1`
- `pwsh -File scripts/check-inventory.ps1`
- `pwsh -File scripts/check-encoding.ps1`
- `ggshield secret scan repo .` before meaningful commits

If a tool is missing, document the exact blocker in `STATE.md` and `docs/CHANGELOG.dev.md`.

## 9. Encoding And Line Endings

Default text files:

- UTF-8
- LF

Windows shell files:

- `.ps1`, `.bat`, `.cmd` may use CRLF when needed.

Follow `.editorconfig` and `.gitattributes`.

## 10. Versioning Policy

Version format:

- `major.minor.patch5`
- example: `0.1.00001`

Rules:

- bump only for real application behavior, runtime, UI, build, packaging, or security-script changes
- no bump for docs-only, plan-only, changelog-only, or local ignored state changes
- keep visible version values in project files, app metadata, and docs aligned once the app exposes a version

Each task must explicitly state whether a version bump was needed.

## 11. Commit Policy

Preferred commit order follows `CODEX_AGENT_PLAN.md`.

Do not commit or push unless the user explicitly asks, or the active user-provided plan requires a stable checkpoint and validation has reached the matching stop condition.

## 12. Rollback Policy

Stable rollback points are:

- latest green build/test/security-scan state
- latest committed checkpoint
- latest factual `STATE.md` update before a risky operation

Do not automatically revert user changes.

## 13. What Not To Do

- Do not add Electron, MAUI, WinUI, WebView, telemetry, analytics, auto-update, or cloud sync.
- Do not add graphs, history, plugin systems, skin engines, or installer work before the MVP.
- Do not weaken parent workspace safety rules.
