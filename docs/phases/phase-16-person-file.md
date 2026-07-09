# Phase 16 — Person file (read model)

- **Status:** Planned
- **Goal:** Consolidated **citizen profile** — one read-only view of identity, domicile, household, cases, and certificates for a person in the National Register.
- **Maps to IDEA:** “Information maintained about the person”; administrative file after registration.

---

## Summary

Build a **query-side read model** (not a writable mega-aggregate) that officers open from NR search, case lists, or direct lookup. The person file ties together data from Registration, BirthDeclaration, ChangeOfAddress, IdentityDocuments, and certificates.

[DOMAIN.md](../DOMAIN.md) already describes this as a later-phase projection; Phase 16 implements it.

---

## Read model: `PersonFile`

| Section | Data sources |
|---------|--------------|
| **Header** | Name, NR number, register type, civil status, photo stub |
| **Identity** | `Person` aggregate fields |
| **Domicile** | Current address + housing situation |
| **Household** | Members and relationships |
| **Cases** | Registration cases, birth declarations, COA cases, document requests (links) |
| **Certificates** | Issued residence / household certificates (Phase 8) |
| **History** | Audit entries across case types (timeline) |

---

## Slices

| Slice | Notes |
|-------|-------|
| `GetPersonFile` | By `PersonId` or NR number |
| `SearchPersonFile` | Entry from global search (reuse NR search ranking) |
| `ListPersonCases` | Paginated case history for tabs |

Handlers compose existing tables; optional dedicated `PersonFileQuery` class in Infrastructure.

---

## UI

Template from [design-system/06-page-templates.md](../design-system/06-page-templates.md) §10:

- Route: `/persons/{personId}` or `/register/persons/{nrNumber}`
- Header: `MudAvatar` initials, name, NR, `AppStatusChip` for register type
- Tabs: **Overview** | **Household** | **Addresses** | **Cases** | **Certificates**
- `AppPersonCard` / `AppPropertyGrid` — implement deferred UI kit components here
- Deep links from registration case detail (“View person file”)

---

## Infrastructure

- No new write tables for MVP.
- Optional view: SQL view `person_file_summary` if queries become unwieldy.
- Index: NR number on `Person` (likely exists).

---

## Demo

Search NR `85.12.31-123.45` → open person file → see registered EU citizen, current Avenue Rogier address, household with spouse and child linked from Phase 12 birth declaration, two closed registration cases, one residence certificate.

---

## Tests

- Integration: registered person from Phase 7 happy path → person file returns consistent identity + case link.
- Unknown NR returns 404.
- Person with no cases still returns identity header.

---

## Out of scope

- Editing from person file (edits stay in case workflows; Phase 17 for amendments).
- Merging all aggregates into one writable entity.
- FR / NL localization.

---

## Dependencies

- Richest demo after **Phases 12–14** (multiple case types to show in Cases tab).
- Cannot ship before **Phase 12** (birth declarations); **Phase 13+** recommended for COA metrics.

---

## Related documents

- [design-system/05-ui-kit.md](../design-system/05-ui-kit.md) — `AppPersonCard` trigger
- [phase-5-national-register-search-bis.md](./phase-5-national-register-search-bis.md) — search entry point
