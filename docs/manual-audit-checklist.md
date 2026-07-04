# Manual Audit Checklist

Last updated: 2026-07-04

- [ ] Review all package references.
- [ ] Search for network APIs.
- [ ] Search for update mechanisms.
- [ ] Search for `Process.Start`.
- [ ] Search for URLs.
- [ ] Review registry writes.
- [ ] Review startup registration.
- [ ] Review shell/process launch behavior.
- [ ] Review logging behavior.
- [ ] Confirm no telemetry or analytics dependency.
- [ ] Confirm no embedded browser or WebView.
- [ ] Confirm settings stay under `%LocalAppData%\WD-HUD`.
- [ ] Confirm firewall scripts target only the WD-HUD executable.
