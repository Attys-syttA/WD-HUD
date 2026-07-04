# CHANGELOG.dev

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
