# Phase 1 — Case intake & identity

- **Status:** Complete
- **Completed:** July 2026
- **Goal:** Open a registration case and record identity with document upload.

---

## Summary

Phase 1 delivers the first vertical slice through all layers: officers can open an intake case, record citizen identity, and attach supporting documents. The domain model introduces the completeness checklist (`IdentityEstablished` first), `Person` and `AdministrativeDocument` aggregates, and guarded transitions via `InvalidRegistrationTransitionException`.

---

## Deliverables checklist

| Deliverable | Status | Notes |
|-------------|--------|-------|
| `OpenRegistrationCase` slice | Done | Handler, validator, endpoint, dialog |
| `ListRegistrationCases` / `GetRegistrationCase` | Done | List enhanced; detail DTO with checklist |
| `RecordIdentity` slice | Done | Creates `Person`, marks checklist |
| `AttachDocument` slice | Done | Local file storage + metadata |
| `RegistrationCase` checklist | Done | Owned type on case; `IdentityEstablished` |
| `Person` aggregate | Done | Name, DOB, nationality |
| `AdministrativeDocument` | Done | Type, path, hash, officer |
| Case list page | Done | New case button, identity column |
| Case detail page | Done | Checklist, identity form, documents |
| Document upload component | Done | `MudFileUpload` + handler |
| FluentValidation | Done | Open case + record identity + attach |
| Domain tests | Done | 4 tests — transitions + checklist |
| Integration tests | Done | Open + record identity + API |
| PostgreSQL migration job | Done | `ci-postgres.yml`, `Category=PostgreSQL` |
| EF migration `CaseIntakeAndIdentity` | Done | Extends `registration_cases`; adds tables |

---

## Domain changes

### `RegistrationCase`

- `Open(visitReason, assignedOfficer, openedAt)` replaces bare `Create()`.
- `RecordIdentity(identity)` creates a `Person`, links `PersonId`, sets `Checklist.IdentityEstablished`.
- `EnsureCanAttachDocuments()` guards document upload during `Intake` or `UnderReview`.
- Data recording does **not** advance status — case stays `Intake`.

### New types

| Type | Location |
|------|----------|
| `Person`, `PersonId`, `IdentityDetails` | `Domain/Identity/` |
| `AdministrativeDocument`, `DocumentType`, `IDocumentStorage` | `Domain/Documents/` |
| `VisitReason`, `OfficerId`, `RegistrationCaseChecklist` | `Domain/Registration/` |
| `InvalidRegistrationTransitionException` | `Domain/Registration/` |

---

## API routes

| Method | Route | Slice |
|--------|-------|-------|
| `GET` | `/api/registration/cases` | List |
| `POST` | `/api/registration/cases` | Open |
| `GET` | `/api/registration/cases/{id}` | Get detail |
| `POST` | `/api/registration/cases/{id}/identity` | Record identity |
| `POST` | `/api/registration/cases/{id}/documents` | Attach document (multipart) |

---

## UI pages

| Route | Page | Purpose |
|-------|------|---------|
| `/registration/cases` | `RegistrationCaseList.razor` | Grid + **New case** dialog |
| `/registration/cases/{id}` | `RegistrationCaseDetail.razor` | Checklist, identity step, uploads |

---

## Demo walkthrough

1. Start AppHost: `dotnet run --project src/SchaerbeekMunicipality.AppHost`
2. Open **Registration cases** → click **New case**
3. Select visit reason (e.g. First registration) → **Open case**
4. On the case detail page, fill identity (name, DOB, nationality) → **Record identity**
5. Upload a passport scan (PDF/JPG/PNG) via **Attach document**
6. Confirm checklist shows **Identity ✓** and document appears in the list

---

## Tests

| Project | Count | Coverage |
|---------|-------|----------|
| `Domain.Tests` | 4 | Open, record identity, duplicate identity, attach guard |
| `Integration.Tests` | 8 | Handlers, API, validators, health |
| `Integration.Tests` (PostgreSQL) | 1 | Migrations apply on Testcontainers |

```bash
dotnet test --configuration Release --filter "Category!=PostgreSQL"   # fast suite
dotnet test --configuration Release --filter "Category=PostgreSQL"    # needs Docker
```

---

## Verification commands

```bash
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release --filter "Category!=PostgreSQL"
```

Expected: **12 tests passing** in the fast suite (4 domain + 8 integration).

---

## Intentionally deferred to Phase 2

| Item | Phase |
|------|-------|
| `SetResidenceCategory` | 2 |
| Residence permit evidence | 2 |
| `LegalResidenceEstablished` checklist flag | 2 |
| Address declaration | 3 |

---

## Related documents

- [ROADMAP.md](../ROADMAP.md)
- [phase-0-foundation.md](./phase-0-foundation.md)
- [DOMAIN.md](../DOMAIN.md)
- [TESTING.md](../TESTING.md)
