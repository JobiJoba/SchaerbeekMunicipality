# Phase 26 — Cross-workflow polish & demo hardening

- **Status:** Planned
- **Goal:** Close **deferred UX and integration gaps** across Phases 12–18 without adding a large new bounded context — make demos coherent end-to-end.
- **Maps to:** Carries-forward notes from birth, COA, NR, person file, and Phase 18.

---

## Summary

After many vertical slices, several **small but high-leverage** items remain scattered in phase docs. Phase 26 batches them into one polish release so the municipality simulation feels continuous when officers jump between contexts.

This is intentionally a **hardening phase**: multiple thin slices, shared tests, seed/demo updates — not a new life-event aggregate.

---

## Slice backlog

| # | Item | Source | Effort |
|---|------|--------|--------|
| 26.1 | Birth: banner when parent not in NR → CTA to open first-registration case | Phase 12 deferred | S |
| 26.2 | Person file: parents / children from birth declarations + spouse links | Phase 12 / 16 carries | M |
| 26.3 | COA: reference address → domicile transition path | Phase 18 out-of-scope / Phase 13 | M |
| 26.4 | NR: unlink / re-link identity after wrong link (intake correction) | Phase 5 / 2.1 carries | M |
| 26.5 | Demo seeder: diplomat + reference-address showcase cases | Phase 18 | S |
| 26.6 | Review dashboard: ensure all case types (death/departure/… if built) remain consistent | Cross-cutting | S–M |
| 26.7 | Person file cases tab: pagination or “load more” | Phase 16 out-of-scope | S |
| 26.8 | APP-OVERVIEW / screenshots refresh for Phases 13–18 | Docs | S |

Pick a **minimum set** (26.1–26.5) for the demoable core; treat 26.6–26.8 as stretch inside the same phase window.

---

## Architecture notes

- Prefer application-level orchestration over new aggregates
- 26.3 may add one domain method on `ChangeOfAddressCase` or `RegistrationCase` history — keep rules explicit in domain tests
- 26.4 must respect Phase 2.1 editability guards and audit

---

## Demo (minimum)

1. Birth case with one unknown parent → banner → open registration for that parent → return to birth case.
2. Person file of a parent shows confirmed child from birth declaration.
3. Homeless reference-address resident later opens COA → converts to real Schaerbeek domicile with police.
4. Wrong NR link → unlink → link correct person → continue intake.

---

## Tests

- One integration test per selected slice (26.1–26.5)
- Regression: existing Phase 12/13/16/18 suites stay green
- Optional: expand E2E smoke for person-file family tab

---

## Out of scope

- New life-event types (those are Phases 20–24)
- FR / NL localization
- Large reporting redesign
- Accessibility axe-core CI gate (can be a later micro-phase)

---

## Dependencies

- Best run **after** Phase 20–22 if those land first (dashboard consistency), **or earlier** as a breather sprint between large features
- Phase 18 complete (reference address) for 26.3

---

## Related documents

- [phase-12-birth-declaration.md](./phase-12-birth-declaration.md) — carries forward
- [phase-13-change-of-address.md](./phase-13-change-of-address.md)
- [phase-16-person-file.md](./phase-16-person-file.md)
- [phase-18-remaining-exception-scenarios.md](./phase-18-remaining-exception-scenarios.md)
- [phase-19-life-events-citizen-services.md](./phase-19-life-events-citizen-services.md)
