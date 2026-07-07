# Phase 12 — Birth declaration

- **Status:** Planned
- **Goal:** Register a **newborn child** in the population register — a distinct procedure from first registration of an adult foreign citizen.
- **Maps to IDEA:** Reception visit reason “Birth declaration”; municipal act of recording a birth and assigning household / register links.

---

## Summary

When parents declare a birth at the municipality, officers record the **child**, link **one or two parents** already in the National Register (or open a follow-up registration case), attach the **medical declaration** (hospital / midwife), and confirm the child’s entry in the **population register**.

This is **not** the same as Phase 9’s `RecordBirthInformation` slice, which captures an **applicant’s** place/country of birth during **first registration**. Phase 12 introduces a new aggregate and feature folder for the **child-centric** workflow.

`VisitReason.BirthDeclaration` already exists on `RegistrationCase` for reception routing; Phase 12 adds the real procedure behind that reason.

---

## Bounded context

| Context | Role |
|---------|------|
| **Reception** | Route citizen with reason `BirthDeclaration` → open birth declaration case |
| **BirthDeclaration** (new) | Own aggregate, checklist, decision, and confirmation |
| **Registration** (existing) | NR search to link parents; optional spawn of first-registration case for a parent not yet registered |
| **NationalRegister** (existing) | Assign stub NR number to the child on confirmation |

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

### Checklist (illustrative)

| Flag | Rule |
|------|------|
| `ChildDetailsRecorded` | Names, sex, date/time, place of birth present |
| `AtLeastOneParentLinked` | At least one parent found in NR and linked |
| `MedicalDeclarationAttached` | Hospital / midwife document on file |
| `HouseholdEstablished` | Child will be registered at a valid domicile (linked household or address) |
| `ReadyForConfirmation` | All of the above |

### Domain methods (illustrative)

- `BirthDeclarationCase.Open(...)` — reception opens case
- `RecordChildDetails(NewbornDetails)`
- `LinkParent(PersonId, ParentRole)`
- `UnlinkParent(PersonId)`
- `AttachMedicalDeclaration(documentId)` / `RemoveDocument`
- `SetHousehold(HouseholdId)` or `DeclareDomicile(BelgianAddress)` (simplified: link parents’ current address)
- `ConfirmDeclaration()` — assigns child NR stub, emits `BirthRegistered` domain event
- `Reject(reason)` / `Suspend(reason)` / `Resume()`

### Policies

- Child date of birth must not be in the future; reasonable window (e.g. declaration within 15 days — configurable constant for demo).
- Linked parent must exist in `national_register_persons` or registered `Person` aggregate.
- Cannot confirm without medical declaration (Belgian practice).

### Domain events

- `BirthRegistered` — triggers stub outbound notification (tax, mutuality) via existing notification log pattern from Phase 8.

---

## Slices

| Slice | Priority | Notes |
|-------|----------|-------|
| `OpenBirthDeclarationCase` | High | Reception or population officer; sets visit metadata |
| `ListBirthDeclarationCases` | High | Case list with status filters |
| `GetBirthDeclarationCase` | High | Detail read model |
| `RecordChildDetails` | High | Child identity step |
| `LinkParent` / `UnlinkParent` | High | NR search dialog reuse from Phase 5 |
| `AttachDocument` / `RemoveDocument` | High | Medical declaration; reuse `IDocumentStorage` |
| `SetDeclarationHousehold` | High | Link to parents’ household or declare address |
| `GetBirthDeclarationChecklist` | Medium | Review readiness |
| `ConfirmBirthDeclaration` | High | Terminal success; NR assignment for child |
| `RejectBirthDeclaration` / `Suspend` / `Resume` | Medium | Exception paths |
| `ClaimBirthDeclarationCase` / `Release` | Medium | Reuse Phase 8.1 locking pattern |

---

## Infrastructure

- EF configuration + migration: `birth_declaration_cases` table (+ child details columns or owned type).
- Reuse document storage and NR seed tables.
- API group: `/api/birth-declarations/...` (separate from `/api/registration/...`).

---

## UI

| Page / component | Description |
|------------------|-------------|
| Reception routing | “Birth declaration” on new-case flow opens `BirthDeclarationCase`, not `RegistrationCase` |
| Case list | `/birth-declarations` — `AppDataTable`, status chips, assignee |
| Case detail | Collapsible sections: Child → Parents → Medical document → Household → Decision |
| `ParentLinkSection` | NR search dialog (reuse `NationalRegisterSearchDialog` patterns) |
| `ChildDetailsStep` | Names, sex, date/time picker, place of birth |
| Decision panel | Checklist + confirm / reject (mirror `CaseDecisionSection` layout) |

Design-system: no new primitives required; optional `AppPersonCard` deferred to Phase 16.

---

## Demo

1. Reception opens birth declaration for parents already registered in Schaerbeek.
2. Officer records child (Amélie Dupont, female, born yesterday at CHU Saint-Pierre).
3. Links mother via NR search; attaches medical PDF.
4. Confirms → child receives stub NR number; notification log shows “Notify mutuality.”

**Secondary demo:** One parent not in NR → link registered parent + banner to open first-registration case for the other parent (link only, no full implementation of parent registration in this phase).

---

## Tests

| Test | Type |
|------|------|
| Cannot confirm without medical declaration | Domain |
| Cannot confirm without parent link | Domain |
| Future date of birth rejected | Domain |
| Open → record child → link parent → confirm happy path | Integration |
| NR search link prevents duplicate child assignment | Integration |
| Reception opens correct case type for `BirthDeclaration` visit reason | Integration |

---

## Out of scope

- Full legal paternity / recognition workflow (simplified to parent link only).
- Stillborn or late registration beyond demo window (single rejection rule is enough).
- FR / NL UI strings.
- Merging birth declaration into `RegistrationCase` aggregate (keeps contexts separate).

---

## Carries forward

- Phase 16 person file: show confirmed children and birth declaration history on parent profile.
- Phase 15 reporting: count birth declarations per month.
- Phase 17: post-registration correction of child name (amendment), not intake edit.

---

## Related documents

- [ROADMAP.md](../ROADMAP.md) — Phase 12 entry
- [phase-9-exception-scenarios.md](./phase-9-exception-scenarios.md) — `RecordBirthInformation` on registration cases (different concern)
- [DOMAIN.md](../DOMAIN.md) — bounded contexts
