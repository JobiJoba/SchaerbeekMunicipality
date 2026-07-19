# Phase 19 — Life events & citizen services (scope definition)

- **Status:** Complete (scope definition)
- **Completed:** July 2026
- **Goal:** Identify and sequence the **second horizon** of municipal workflows after Phases 12–18 — life-cycle events, citizen-facing services, and operational depth beyond first registration and the first extended backlog.

---

## Summary

Phases 0–10 built the **first-registration simulation**. Phases 11–18 added **extended municipality** procedures (birth, change of address, passport/eID, reporting, person file, amendments, remaining exceptions).

Phase 19 is an **umbrella planning phase** — not a delivery slice. The backlog below is split into **Phases 20–26** with dedicated planning documents.

**Still not planned:** multi-language UI (FR / NL). Design-system components remain string-free.

---

## What we have today (baseline)

| Capability area | Status | Notes |
|-----------------|--------|-------|
| First registration (four core decisions) | ✅ | Full intake → police → approve → confirm → certificates |
| Roles, locking, review dashboard | ✅ | Reception / Population / Police / Back office |
| Exception paths (Phase 9 + 18) | ✅ | Suspend, illegal stay, refugee, diplomat, reference address, … |
| Birth declaration | ✅ | Separate `BirthDeclarationCase` context |
| Change of address (intra-municipal) | ✅ | `ChangeOfAddressCase` + optional police |
| Passport / eID request | ✅ | `DocumentRequestCase` linear production workflow |
| Reporting KPIs | ✅ | Back-office + population read model |
| Person file | ✅ | Consolidated read model + deep links from cases |
| Post-registration amendments | ✅ | Name / civil status / nationality with approval |
| Azure deploy | ✅ | SQLite default; optional PostgreSQL |

**Visit reasons wired today:** `FirstRegistration`, `EuCitizenRegistration`, `ChangeOfAddress`, `BirthDeclaration`, `PassportRenewal`.

---

## Gaps that drive Phases 20–26

| Gap | Why it matters educationally | New phase |
|-----|------------------------------|-----------|
| No end-of-life / leave-register path | Birth exists; death/radiation completes the person lifecycle | **20** |
| Certificates only after registration case | Real desks issue extracts on demand for any resident | **21** |
| No move **out** of Schaerbeek / abroad | Phase 13 is intra-municipal only | **22** |
| No residence **card** production after approve | IDEA Phase 21; passport/eID is a different product | **23** |
| Marriage recorded only as intake field | Civil registry event deserves its own procedure | **24** |
| Duplicate identity is a flag, not a workflow | Phase 9 opens investigation; officers need a queue | **25** |
| Deferred polish across contexts | Parent-not-in-NR banner, reference→domicile, person-file family links | **26** |

---

## Original ideas → new phases

| Idea | New phase | Document |
|------|-----------|----------|
| Death declaration & radiation (deregistration) | **20** | [phase-20-death-declaration-radiation.md](./phase-20-death-declaration-radiation.md) |
| On-demand certificate desk | **21** | [phase-21-certificate-desk.md](./phase-21-certificate-desk.md) |
| Inter-municipal move & emigration | **22** | [phase-22-inter-municipal-move-emigration.md](./phase-22-inter-municipal-move-emigration.md) |
| Residence card production | **23** | [phase-23-residence-card-production.md](./phase-23-residence-card-production.md) |
| Marriage / partnership declaration | **24** | [phase-24-marriage-partnership-declaration.md](./phase-24-marriage-partnership-declaration.md) |
| Investigation workspace (duplicates & flags) | **25** | [phase-25-investigation-workspace.md](./phase-25-investigation-workspace.md) |
| Cross-workflow polish & demo hardening | **26** | [phase-26-cross-workflow-polish.md](./phase-26-cross-workflow-polish.md) |
| FR / NL localization | — | Not planned |

---

## Suggested implementation order

1. **Phase 20** — Death declaration & radiation (symmetric to birth; high teaching value)
2. **Phase 21** — Certificate desk (high citizen volume; reuses Phase 8 PDF stubs)
3. **Phase 22** — Inter-municipal move & emigration (extends Phase 13 geography)
4. **Phase 23** — Residence card production (closes IDEA gap after first registration)
5. **Phase 24** — Marriage / partnership declaration (civil-status life event)
6. **Phase 25** — Investigation workspace (operational depth on Phase 9 flags)
7. **Phase 26** — Cross-workflow polish (deferred UX and person-file enrichment)

Phases 21 and 23 can swap if post-registration outputs are prioritised over the public certificate desk. Phase 26 can be pulled earlier as a short hardening sprint between larger features.

---

## Explicitly deferred beyond 26

These remain interesting but are **not** scheduled:

- Appointment scheduling / physical queue hardware (`ReceptionTicket`)
- Real FPS / IBZ / eID integrations (stay stubbed)
- Full legalisation / apostille pipeline as a separate product
- Waiting-register long-running case management as its own aggregate
- OCR / automatic document classification
- Dark-mode user toggle (palette already ships)

---

## Carries forward

- New features live under **`Web/Features/{Context}/`** — not inside `Registration/` (see [ARCHITECTURE.md](../ARCHITECTURE.md)).
- Slice checklist in [ROADMAP.md](../ROADMAP.md) applies to every phase 20+ slice.
- Phase 10 deployment model unchanged: one container image, SQLite or PostgreSQL profile.
- Person file (Phase 16) remains the **read hub**; new case types should deep-link into it.

---

## Related documents

- [ROADMAP.md](../ROADMAP.md) — phase entries 19–26
- [phase-11-extended-municipality.md](./phase-11-extended-municipality.md) — first extended backlog (12–18)
- [DOMAIN.md](../DOMAIN.md) — bounded contexts
- [IDEA.md](../../IDEA.md) — original process narrative
