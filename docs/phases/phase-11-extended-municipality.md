# Phase 11 — Extended municipality

- **Status:** Complete (scope definition)
- **Completed:** July 2026
- **Goal:** Identify and sequence follow-on municipal workflows beyond first registration.

---

## Summary

Phase 11 was an **umbrella planning phase** — not a single delivery slice. It captured ideas for growing the simulation past the first-registration loop (Phases 1–10). That backlog is now **split into concrete phases 12–18** with dedicated planning documents.

**Explicitly out of scope** for this roadmap line (product decision): multi-language UI (FR / NL). The design system remains string-free so localization could be added later without refactoring components, but no phase is scheduled for it.

---

## Original ideas → new phases

| Former Phase 11 idea | New phase | Document |
|----------------------|-----------|----------|
| Birth declaration | **12** | [phase-12-birth-declaration.md](./phase-12-birth-declaration.md) |
| Change of address within municipality | **13** | [phase-13-change-of-address.md](./phase-13-change-of-address.md) |
| Passport / ID card request (simplified) | **14** | [phase-14-passport-id-request.md](./phase-14-passport-id-request.md) |
| Reporting dashboard | **15** | [phase-15-reporting-dashboard.md](./phase-15-reporting-dashboard.md) |
| Person file read model | **16** | [phase-16-person-file.md](./phase-16-person-file.md) |
| Post-registration amendments | **17** | [phase-17-post-registration-amendments.md](./phase-17-post-registration-amendments.md) |
| Deferred exception scenarios (diplomat, homeless) | **18** | [phase-18-remaining-exception-scenarios.md](./phase-18-remaining-exception-scenarios.md) |
| FR / NL localization | — | Not planned |

---

## Suggested implementation order

1. **Phase 12** — Birth declaration (new bounded context; reception already exposes `VisitReason.BirthDeclaration`)
2. **Phase 13** — Change of address (second high-volume municipal procedure)
3. **Phase 14** — Passport / ID request (citizen-facing output workflow)
4. **Phase 15** — Reporting dashboard (cross-cutting metrics; benefits from more case types)
5. **Phase 16** — Person file (read model; richer once birth + COA cases exist)
6. **Phase 17** — Post-registration amendments (depends on registered persons in NR)
7. **Phase 18** — Remaining exception scenarios (optional depth on first-registration edge cases)

Phases 15 and 16 can swap if a consolidated citizen view is needed before analytics.

---

## Carries forward

- New features live under **`Web/Features/{Context}/`** — not inside `Registration/` (see [ARCHITECTURE.md](../ARCHITECTURE.md)).
- Phase 10 deployment model unchanged: one container image, SQLite or PostgreSQL profile.
- Slice checklist in [ROADMAP.md](../ROADMAP.md) applies to every phase 12+ slice.

---

## Related documents

- [ROADMAP.md](../ROADMAP.md) — phase entries 12–18
- [DOMAIN.md](../DOMAIN.md) — bounded contexts and person file note
