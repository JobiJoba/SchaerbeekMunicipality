# Domain model

This document maps the Belgian municipal population workflow ([IDEA.md](../IDEA.md)) onto bounded contexts, aggregates, and implementation priorities. Terminology aligns with [GLOSSARY.md](./GLOSSARY.md).

## Context map

```
                    ┌─────────────┐
                    │  Reception  │
                    └──────┬──────┘
                           │ directs visit
                           ▼
┌──────────────┐    ┌──────────────────────────────────────────┐
│ National     │◄───│           Registration (core)             │
│ Register     │    │  RegistrationCase aggregate + workflow    │
└──────────────┘    └───────┬───────────────────────┬──────────┘
       ▲                    │                       │
       │                    │                       │
┌──────┴───────┐    ┌───────▼──────┐        ┌───────▼────────┐
│ Identity &   │    │ Address &    │        │ Immigration &  │
│ Civil Status │    │ Household    │        │ Residence      │
└──────────────┘    └───────┬──────┘        └────────────────┘
                            │
                    ┌───────▼──────┐
                    │   Police     │
                    │ Verification │
                    └───────┬──────┘
                            │
                    ┌───────▼──────┐        ┌────────────────┐
                    │  Documents   │        │ Certificates & │
                    │  (evidence)  │        │ Notifications  │
                    └──────────────┘        └────────────────┘
```

**Core context:** `Registration` — owns the case lifecycle and ties other contexts together.

**Supporting contexts:** provide capabilities invoked during the case; they do not silently mutate registration state without going through `RegistrationCase`.

---

## Bounded contexts

### 1. Reception

**Purpose:** Front desk intake — reason for visit, routing, appointment (optional).

**Key concepts:**

- `VisitReason` (enum): FirstRegistration, ChangeOfAddress, EUCitizenRegistration, BirthDeclaration, …
- `ReceptionTicket` (optional lightweight aggregate for queue simulation)

**Integration:** Opening a `RegistrationCase` when reason is first registration.

**Not in MVP:** appointment scheduling, queue display hardware.

---

### 2. Registration (core)

**Purpose:** Orchestrate the full first-registration procedure until the person is entered in a register or the case is rejected/suspended.

**Aggregate:** `RegistrationCase`

| Field / concept | Description |
|-----------------|-------------|
| `RegistrationCaseId` | Strongly typed identifier |
| `Status` | Lifecycle status — `Intake`, `AwaitingPoliceVerification`, … (see ARCHITECTURE.md) |
| `Checklist` | Computed completeness flags mirroring the four core decisions |
| `VisitReason` | Why the citizen came |
| `AssignedOfficerId` | Population officer |
| `PersonId` | Link to identity being established |
| `ResidenceCategory` | EU worker, student, refugee, … |
| `DeclaredAddress` | Address pending confirmation |
| `RegisterTarget` | Population / Foreigners / Waiting / Special register |
| `OpenedAt`, `ClosedAt` | Timestamps |
| `AuditTrail` | Collection of `CaseAuditEntry` |

**Domain invariants (examples):**

- Cannot move to `AwaitingPoliceVerification` unless `IdentityEstablished` and `AddressDeclared` are true.
- Cannot `Approve` while police result is negative or missing (unless policy exception flagged), or while any checklist flag is unresolved.
- Cannot `ConfirmRegistration` without `Approved` status.
- Data-recording methods (identity, address, civil status, …) never advance the status — they only update checklist flags.
- Duplicate open case for same person identity fingerprint → domain exception.

**Use cases (slices):**

| Slice | Maps to IDEA phase |
|-------|-------------------|
| `OpenRegistrationCase` | Phase 1–2 |
| `RecordIdentity` | Phase 2 |
| `SetResidenceCategory` | Phase 3 |
| `RecordResidencePermit` | Phase 4 |
| `DeclareAddress` | Phase 5–6 |
| `RecordHouseholdComposition` | Phase 7 |
| `RecordCivilStatus` | Phase 8 |
| `AttachDocument` | Phase 12 |
| `SearchNationalRegister` | Phase 13–14 |
| `RequestPoliceVerification` | Phase 15 |
| `RecordPoliceResult` | Phase 17 |
| `ReviewCase` / `ApproveCase` / `RejectCase` | Phase 18 |
| `ConfirmRegistration` | Phase 19–20 |

**Phase 12 (complete):** Newborn registration is a **separate** bounded context — aggregate `BirthDeclarationCase`, API `/api/birth-declarations/*`, not a variant of `RegistrationCase`. Reception visit reason `BirthDeclaration` opens a birth case. See [phase-12-birth-declaration.md](./phases/phase-12-birth-declaration.md).

---

### 3. Identity & civil status

**Purpose:** Establish who the person is and civil status facts.

**Aggregates:**

- `Person` — legal name, birth facts, sex, nationalities, previous names
- `CivilStatusRecord` — single, married, divorced, … with supporting marriage recognition status

**Value objects:**

- `FullName`, `BirthPlace`, `Nationality`, `PreviousName`

**Integration:** Registration case records identity snapshot; `Person` may exist before full registration (BIS scenario).

**Exceptions from IDEA:**

- Missing birth certificate → case `Suspended`, document requests
- Marriage not recognised → civil status temporarily `Single`

---

### 4. Immigration & residence

**Purpose:** Legal basis to stay in Belgium; permit types and validity.

**Concepts:**

- `ResidenceCategory` (enum + metadata)
- `ResidencePermit` — type (A/B/C/… card, Annex 15, …), validity, issuing authority
- `ImmigrationDecision` — reference to external decision (stub)

**Educational simplification:** Rule engine as explicit C# policy classes per category, not a full legal rules engine.

```csharp
public interface IResidencePolicy
{
    ResidenceCategory Category { get; }
    ValidationResult ValidatePermitDocuments(IReadOnlyList<DocumentReference> docs);
}
```

**Integration:** `RegistrationCase.SetResidenceCategory(...)` validates against policy.

---

### 5. Address & household

**Purpose:** Declared domicile and who lives together.

**Value objects / entities:**

- `BelgianAddress` — street, number, box, postal code, municipality
- `HousingSituation` — owner, tenant, student housing, shelter, …
- `Household` — members linked to `PersonId`, roles (head, spouse, child)

**Integration:** Address declaration on case; household may reference persons already registered at same address (query).

---

### 6. Police verification

**Purpose:** Municipality requests local police to confirm physical residence.

**Aggregate:** `PoliceVerificationRequest`

| Field | Description |
|-------|-------------|
| `RequestId`, `RegistrationCaseId` | Links |
| `RequestedAt`, `CompletedAt` | Timing |
| `Result` | Confirmed, NotFound, MailboxOnly, RefusedAccess, Incomplete, … |
| `OfficerNotes` | Free text |
| `AttemptNumber` | Supports second visit |

**Integration:** Case transitions to `AwaitingPoliceVerification` on request; `RecordPoliceResult` updates both request and case.

**Educational UI:** Police clerk portal at `/registration/police-verifications`; case detail `PoliceVerificationSection` shows pending visit and completed history (card for one visit, table for multiple) including clerk notes.

**Implemented (Phase 6):** see [phase-6-police-verification-loop.md](./phases/phase-6-police-verification-loop.md).

---

### 7. Documents (evidence)

**Purpose:** Store metadata and files for identity, residence, and address proof.

**Aggregate:** `AdministrativeDocument`

- Type (passport, rental contract, marriage certificate, …)
- Storage path / hash
- Validity, language, legalisation status (simplified flags)
- Linked `RegistrationCaseId`

**Not in early phases:** OCR, automatic classification.

---

### 8. National Register (stub)

**Purpose:** Search for existing persons; BIS → NR conversion simulation.

**Implemented (Phase 5):** see [phase-5-national-register-search-bis.md](./phases/phase-5-national-register-search-bis.md).

**Operations:**

- `SearchNationalRegister` → scored matches (optional partial criteria: given name, family name, and/or birth date)
- `LinkExistingPerson` → attach stub record to case as `Person`
- `RecordIdentity` → create new person when no link (duplicate warning on read)
- `ConvertBisNumber` → stub NR assignment for BIS-only persons
- `AssignNationalRegisterNumber` → planned Phase 7 (official registration)

**Educational stub:** PostgreSQL/SQLite table `national_register_persons` seeded at startup; `NationalRegisterSearchScorer` ranks candidates (scores 40–100).

---

### 9. Certificates & notifications

**Purpose:** Documents issued to citizen; inform other administrations (stub).

**Concepts:**

- `CertificateRequest` — residence certificate, household composition, proof of address
- `OutboundNotification` — tax, social security, health insurance (log only)

**Triggered by:** `RegistrationConfirmed` domain event.

---

## The four core decisions (implementation mapping)

| Decision | Primary data | Checklist flag / gate |
|----------|--------------|-----------------------|
| Identity certain? | `Person`, attached ID documents | `IdentityEstablished` — officer confirms during `Intake` |
| Legal residence? | `ResidenceCategory`, permits | `LegalResidenceEstablished` — policy validation |
| Address genuine? | `DeclaredAddress`, police result | `AddressConfirmed` — positive police verification |
| Which register? | Policies on category + outcome | `Approve(registerTarget)` requires all flags resolved |

Officer **decision screen** aggregates checklist flags computed from domain state, not ad-hoc UI booleans.

---

## Reference data (not aggregates)

Seed tables / enums:

- `RegisterType`: PopulationRegister, ForeignersRegister, WaitingRegister, SpecialRegister
- `ResidencePermitType`: ACard, BCard, Annex15, …
- `DocumentType`: Passport, VisaD, RentalContract, …
- `Municipality`, `Street` (Schaerbeek subset for demos)
- `RejectionReason`, `SuspensionReason`

---

## Person file (read model — Phase 16)

A **PersonFile** read model consolidates everything IDEA lists under “Information maintained about the person.” Build this as a query/projection once multiple contexts exist — not as a writable mega-entity on day one. See [phases/phase-16-person-file.md](./phases/phase-16-person-file.md).

---

## Exception scenarios (backlog hooks)

Each IDEA exception becomes a testable slice or domain rule:

| Scenario | Behavior | Phase |
|----------|----------|-------|
| Duplicate identity | Block registration; open investigation flag | 9 |
| Illegal stay | Reject; refer to immigration (status reason) | 9 |
| EU citizen | Lighter document policy | 9 |
| Refugee | Additional federal decision prerequisite | 9 |
| Diplomat | Special Register; diplomatic card / passport; police waived | [18](./phases/phase-18-remaining-exception-scenarios.md) |
| Homeless | Municipality reference address when no fixed abode | [18](./phases/phase-18-remaining-exception-scenarios.md) |

See [ROADMAP.md](./ROADMAP.md) for delivery status.

---

## Ubiquitous language in code

Use Belgian/French-Dutch admin terms in **enum names and glossary**, English in **type and method names**:

- Code: `ResidenceCategory.EuCitizen`, `RegisterTarget.ForeignersRegister`
- UI: localized labels (FR/NL/EN later); start with English or bilingual FR/EN for Schaerbeek

---

## Related documents

- [ARCHITECTURE.md](./ARCHITECTURE.md) — slice layout and state machine
- [GLOSSARY.md](./GLOSSARY.md) — term definitions
- [ROADMAP.md](./ROADMAP.md) — phased delivery
