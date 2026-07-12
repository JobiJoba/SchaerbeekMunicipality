# Phase 16 — Person file (read model)

- **Status:** Complete
- **Completed:** July 2026
- **Goal:** Consolidated **citizen profile** — one read-only view of identity, domicile, household, cases, and certificates for a person in the National Register.
- **Maps to IDEA:** “Information maintained about the person”; administrative file after registration.

---

## Summary

Delivered a **query-side read model** that Population officers open from person lookup (NR search), direct URL, or case detail deep links. Data is composed from Registration, BirthDeclaration, ChangeOfAddress, IdentityDocuments, and certificate tables — no new write aggregates.

---

## What was built

### Application slices

| Slice | Route / entry |
|-------|----------------|
| `GetPersonFile` | `GET /api/persons/{personId}`, `GET /api/persons/by-nr/{nrNumber}` |
| `SearchPersonFile` | `GET /api/persons/search` (NR ranking + `PersonId` enrichment) |
| `ListPersonCases` | `GET /api/persons/{personId}/cases` |

Authorization: **`PopulationOfficer` only** (`PersonFileAuthorization`).

### Query layer

- `IPersonFileQuery` in Domain; `PersonFileQuery` in Infrastructure composes household, address history, cross-workflow cases, and timeline events.
- `ICertificateRequestRepository.ListByPersonIdAsync` added for certificates tab.

### UI

| Page | Route |
|------|-------|
| Person lookup | `/persons/search` |
| Person file | `/persons/{personId}` |

- **`AppPersonCard`** and **`AppPropertyGrid`** (Wave 2 UI kit) implemented.
- Tabs: Overview (identity, domicile, recent history), Household, Addresses, Cases, Certificates.
- Nav item **Person lookup** for Population officers.
- **View person file** on registration, change-of-address, identity-document, and birth-declaration (linked parents) case pages.

---

## Demo

1. Run AppHost as Marie Dubois (Population officer).
2. Open **Person lookup** → search “Nguyen” → **Open person file** for Sofia (seeded via `PopulationRegisterSeeder` when demo workflow cases are enabled).
3. Browse tabs — domicile from register seed, cases from demo workflow seeder when `DemoData:SeedWorkflowCases` is true.
4. From a registration case with a linked person → **View person file**.

---

## Tests

`PersonFileTests` (integration):

- Registered person returns identity + registration case link (Phase 7 happy path).
- Unknown NR → `KeyNotFoundException` (404 via middleware).
- Seeded person with no cases returns header only.
- Non–Population-officer roles → `UnauthorizedAccessException`.
- `SearchPersonFile` enriches `PersonId` for seeded NR matches.
- HTTP smoke: `GET /api/persons/{id}`.

---

## Out of scope (unchanged)

- Editing from person file (Phase 17).
- Server-side pagination for cases tab.
- FR / NL localization.

---

## Related documents

- [design-system/05-ui-kit.md](../design-system/05-ui-kit.md) — `AppPersonCard`, `AppPropertyGrid`
- [phase-5-national-register-search-bis.md](./phase-5-national-register-search-bis.md) — search ranking reused by `SearchPersonFile`
