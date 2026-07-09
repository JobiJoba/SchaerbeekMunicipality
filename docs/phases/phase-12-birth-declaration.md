# Phase 12 — Birth declaration

- **Status:** Complete
- **Completed:** July 2026
- **Goal:** Register a **newborn child** in the population register — a distinct procedure from first registration of an adult foreign citizen.
- **Maps to IDEA:** Reception visit reason “Birth declaration”; municipal act of recording a birth and assigning household / register links.

---

## Summary

When parents declare a birth at the municipality, officers record the **child**, link **one or two parents** already in the National Register (or open a follow-up registration case), attach the **medical declaration** (hospital / midwife), and confirm the child’s entry in the **population register**.

This is **not** the same as Phase 9’s `RecordBirthInformation` slice, which captures an **applicant’s** place/country of birth during **first registration**. Phase 12 introduces a new aggregate and feature folder for the **child-centric** workflow.

`VisitReason.BirthDeclaration` on reception intake opens a `BirthDeclarationCase`, not a `RegistrationCase`.

---

## Bounded context

| Context | Role |
|---------|------|
| **Reception** | Route citizen with reason `BirthDeclaration` → open birth declaration case |
| **BirthDeclaration** (new) | Own aggregate, checklist, decision, and confirmation |
| **Registration** (existing) | NR search to link parents; optional spawn of first-registration case for a parent not yet registered |
| **NationalRegister** (existing) | Assign stub NR number to the child on confirmation |

---

## Deliverables checklist

| Deliverable | Status | Notes |
|-------------|--------|-------|
| `BirthDeclarationCase` aggregate + checklist | Done | Statuses, parent links, medical doc, household |
| `BirthRegistered` domain event | Done | Stub tax + mutuality notifications |
| EF migration `Phase12BirthDeclaration` | Done | `birth_declaration_cases` + document FK |
| `IBirthDeclarationCaseRepository` | Done | List, get, add, save |
| API group `/api/birth-declarations/*` | Done | All intake + decision slices |
| Claim / release lock | Done | Reuses Phase 8.1 pattern |
| `BirthDeclarationCaseAuthorization` | Done | Reception create; population list/view/edit |
| Case list `/birth-declarations` | Done | Filters: All, My cases, Unassigned, Ready for confirmation |
| Case detail `/birth-declarations/{id}` | Done | Two-column layout; officer decision panel |
| Reception routing | Done | `NewCaseDialog` + `NewRegistrationCasePage` open birth case |
| Review dashboard integration | Done | Birth tiles + unified **Needs my attention** queue |
| Population officer landing | Done | `/` redirects to review dashboard |
| Domain tests | Done | Checklist gates, transitions, locking |
| Integration tests | Done | Happy path, reception routing, dashboard handoff |

---

## Domain

### Aggregate: `BirthDeclarationCase`

| Field / concept | Description |
|-----------------|-------------|
| `BirthDeclarationCaseId` | Strongly typed identifier |
| `Status` | `Intake` → `UnderReview` → `Confirmed` / `Rejected` / `Suspended` |
| `AssignedOfficerId` | Nullable until claimed (reuse locking pattern from Phase 8.1) |
| `Child` | `NewbornDetails` value object: given names, sex, date and time of birth, place of birth |
| `ParentLinks` | Collection linking to existing `PersonId` + role (`Mother`, `Father`, `CoParent`) |
| `MedicalDeclaration` | Reference to attached `AdministrativeDocument` (type `MedicalBirthDeclaration`) |
| `HouseholdLink` | Target household / address at declaration (parents’ domicile) |
| `Checklist` | Computed readiness flags (see below) |
| `ConfirmedAt` / `ChildNationalRegisterNumber` | Set on confirmation |

### Checklist

| Flag | Rule |
|------|------|
| `ChildDetailsRecorded` | Names, sex, date/time, place of birth present |
| `AtLeastOneParentLinked` | At least one parent found in NR and linked |
| `MedicalDeclarationAttached` | Hospital / midwife document on file |
| `HouseholdEstablished` | Child will be registered at a valid domicile (linked household or address) |
| `ReadyForConfirmation` | All of the above |

### Domain events

- `BirthRegistered` — triggers stub outbound notification (tax, mutuality) via existing notification log pattern from Phase 8.

---

## Slices

| Slice | Route (illustrative) | Status |
|-------|----------------------|--------|
| `OpenBirthDeclarationCase` | `POST /api/birth-declarations/cases` | Done |
| `ListBirthDeclarationCases` | `GET /api/birth-declarations/cases` | Done |
| `GetBirthDeclarationCase` | `GET /api/birth-declarations/cases/{id}` | Done |
| `ClaimBirthDeclarationCase` / `ReleaseCaseLock` | `POST …/claim`, `POST …/release-lock` | Done |
| `RecordChildDetails` | `POST …/child` | Done |
| `LinkParent` / `UnlinkParent` | `POST …/parents`, `DELETE …/parents/{personId}` | Done |
| `AttachDocument` / `RemoveDocument` | `POST …/documents`, `DELETE …/documents/{id}` | Done |
| `SetDeclarationHousehold` | `POST …/household` | Done |
| `GetBirthDeclarationChecklist` | `GET …/checklist` | Done |
| `ConfirmBirthDeclaration` | `POST …/confirm` | Done |
| `RejectBirthDeclaration` / `Suspend` / `Resume` | `POST …/reject`, `…/suspend`, `…/resume` | Done |

---

## Infrastructure

- EF configuration + migration: `birth_declaration_cases` table.
- `AdministrativeDocument` extended with nullable `BirthDeclarationCaseId` and `DocumentType.MedicalBirthDeclaration`.
- Reuse document storage and NR seed tables.
- `BirthRegisteredNotificationHandler` — stub outbound notifications.

---

## UI

| Page / component | Description |
|------------------|-------------|
| Reception routing | “Birth declaration” on new-case flow opens `BirthDeclarationCase`, not `RegistrationCase` |
| Case list | `/birth-declarations` — `AppDataTable`, status chips, assignee, quick filters (`All`, `My cases`, `Unassigned`, `Ready for confirmation`), search; query param `filter` (`mine`, `unassigned`, `ready`) |
| Review dashboard | Unassigned and ready-for-confirmation birth cases appear in **Needs my attention** alongside registration cases; stat tiles **Birth unassigned** and **Ready for confirmation** deep-link to filtered birth list |
| Population landing | `/` redirects population officers to `/registration/review-dashboard` (no separate Home nav item) |
| Case detail | Two-column layout: Child → Parents → Medical document → Household (left); Officer decision (right) |
| `ParentLinkDialog` | NR search to link parent |
| `MedicalDeclarationUpload` / `BirthDeclarationDocumentPanel` | MudFileUpload pattern (same as registration) |
| `BirthDeclarationDecisionSection` | Checklist + confirm / reject / suspend |

Design-system: no new primitives required; optional `AppPersonCard` deferred to Phase 16.

---

## Demo script

1. **Reception handoff** — Jean Martin → **New case** → Birth declaration → note case reference.
2. **Dashboard visibility** — Marie Dupont → lands on **Review dashboard** → case appears under **Birth unassigned** and **Needs my attention** (type Birth declaration).
3. **Intake** — Open case (auto-claim) → record child (Amélie Dupont, female, born yesterday at CHU Saint-Pierre).
4. **Parents** — Link father via NR search; attach medical PDF; set household address.
5. **Confirm** — Checklist green → confirm → child receives stub NR number; outbound notification log shows mutuality stub.

**Deferred demo path:** One parent not in NR → banner to open first-registration case for the other parent (spec only; banner not implemented).

---

## Tests

```bash
dotnet test --configuration Release --filter "Category!=PostgreSQL"
```

| Test | Type |
|------|------|
| Cannot confirm without medical declaration | Domain |
| Cannot confirm without parent link | Domain |
| Future date of birth rejected | Domain |
| Claim / lock enforcement | Domain |
| Open → record child → link parent → confirm happy path | Integration |
| Reception opens correct case type for `BirthDeclaration` visit reason | Integration |
| Unassigned birth declaration appears on review dashboard actionable queue | Integration |

---

## Out of scope

- Full legal paternity / recognition workflow (simplified to parent link only).
- Stillborn or late registration beyond demo window (single rejection rule is enough).
- FR / NL UI strings.
- Merging birth declaration into `RegistrationCase` aggregate (keeps contexts separate).
- Banner to spawn first-registration case when parent not in NR (carries forward).

---

## Carries forward

- Phase 16 person file: show confirmed children and birth declaration history on parent profile.
- Phase 15 reporting: count birth declarations per month.
- Phase 17: post-registration correction of child name (amendment), not intake edit.
- Banner for unregistered parent → open registration case.

---

## Key files

| Area | Path |
|------|------|
| Domain aggregate | `src/.../Domain/BirthDeclaration/BirthDeclarationCase.cs` |
| Repository | `src/.../Infrastructure/Persistence/Repositories/BirthDeclarationCaseRepository.cs` |
| API registration | `src/.../Web/Features/BirthDeclaration/BirthDeclarationEndpoints.cs` |
| Case list / detail | `src/.../Web/Features/BirthDeclaration/Pages/` |
| Review dashboard merge | `src/.../Web/Features/Registration/GetReviewDashboard/GetReviewDashboardHandler.cs` |
| Population landing redirect | `src/.../Web/Components/Pages/Home.razor` |
| Domain tests | `tests/.../Domain.Tests/BirthDeclaration/` |
| Integration tests | `tests/.../Integration.Tests/Features/BirthDeclaration/` |

---

## Related documents

- [ROADMAP.md](../ROADMAP.md) — Phase 12 entry
- [phase-8.1-role-boundaries-case-locking.md](./phase-8.1-role-boundaries-case-locking.md) — review dashboard queue + role switching
- [phase-9-exception-scenarios.md](./phase-9-exception-scenarios.md) — `RecordBirthInformation` on registration cases (different concern)
- [DOMAIN.md](../DOMAIN.md) — bounded contexts
