# Phase 6.1 — Collapsible intake sections

- **Status:** Complete
- **Completed:** July 2026
- **Goal:** Reduce scroll on the registration case detail page by collapsing completed intake categories and auto-expanding the next actionable step.

---

## Summary

Phase 6 added police verification to an already long case detail page. Officers had to scroll past every completed intake section to reach the current task.

Phase 6.1 is a **cross-cutting UX phase** between Phase 6 and Phase 7: intake categories become collapsible panels with header summaries, Identity stays always expanded, and the first incomplete step opens automatically on load/reload. No domain model or API changes.

---

## Problem statement

| Area | Before | Officer impact |
|------|--------|----------------|
| **Case detail length** | All intake sections fully expanded | Long scroll to reach the current step |
| **Progress at a glance** | Checklist chips only at top | Cannot scan section headers without opening each block |
| **Police section placement** | Above the two-column grid | Broke the intake flow; duplicated awaiting alert |

---

## Deliverables checklist

| Deliverable | Status | Notes |
|-------------|--------|-------|
| `AppCollapsibleSection` | Done | Design-system wrapper around `MudExpansionPanel`; fixed mode for Identity |
| `RegistrationIntakeStepSummaries` | Done | Summary text, completion checks, default expansion logic |
| Case detail accordion | Done | Identity fixed; residence → address → household → civil status → police |
| Step components refactored | Done | Body-only; parent owns collapsible chrome |
| Police awaiting alert | Done | Moved into `PoliceVerificationSection` body |
| Duplicate warning | Done | Inside Identity section (always visible) |
| Collapsible CSS (`app.css`) | Done | Panel borders, summary ellipsis |
| Unit tests | Done | `RegistrationIntakeStepSummariesTests` |

---

## UI behaviour

### Expansion rules

- **Identity** — always expanded (non-collapsible); summary in header.
- **Other steps** — collapsible; collapsed by default when complete.
- **Next step** — exactly one additional panel expanded: first incomplete step in order (legal residence → address → household → civil status → police verification).
- **Manual toggle** — officers can expand/collapse any collapsible section; state resets on `ReloadCase()` after save.
- **Checklist bar** — unchanged at top of page.
- **Document panel** — unchanged sticky right column.

### Header summaries

Each section header shows a one-line summary and a status chip (`Complete`, `Pending`, or `Awaiting` for pending police visits).

Examples:

- Identity: `Marie Leclerc · born 01/01/1975 · Belgian`
- Legal residence: `EU citizen` or `Non-EU worker · permit until 2027-06-01`
- Address: `Chaussée de Louvain 42, 1030 Schaerbeek · Owner`
- Police: `Awaiting result · visit 1` or `Confirmed · visit 1`

Police verification section appears only when a visit was sent or recorded.

---

## Demo walkthrough

1. Open a **new case** → Identity expanded; other sections collapsed with pending summaries.
2. Record identity → reload → **Legal residence** auto-expands.
3. Complete residence and address → **Household** auto-expands; completed steps show summaries when collapsed.
4. Expand a completed step → **Edit** still works (Phase 2.1 correction flow).
5. Send to police → **Police verification** panel appears and expands with awaiting summary.
6. Document panel remains sticky on wide screens throughout.

---

## Tests

```bash
dotnet test --configuration Release --filter "RegistrationIntakeStepSummariesTests"
```

Covers default expansion for fresh case, identity complete, address complete without household, and awaiting police.

---

## Carries forward

- Phase 7+ case detail pages should add new intake or review sections as collapsible panels using `AppCollapsibleSection`.
- Expansion state is not persisted across sessions (localStorage out of scope).

---

## Related documents

- [ROADMAP.md](../ROADMAP.md)
- [phase-6-police-verification-loop.md](./phase-6-police-verification-loop.md)
- [phase-4.1-document-preview-case-ux.md](./phase-4.1-document-preview-case-ux.md)
- [get-registration-case.md](../features/registration/get-registration-case.md)
- [06-page-templates.md](../design-system/06-page-templates.md)
