# Application overview

Visual tour of the Schaerbeek Population Department back-office as of **Phase 12** (birth declaration), including the shared **Municipal UI** layer and design-system additions (`AppDateField`, `AppCollapsibleSection`, `AppEditableSection`, `AppChecklist`, `AppFilePreview`).

Run the app locally:

```bash
dotnet run --project src/SchaerbeekMunicipality.AppHost
```

Open the Web app from the Aspire dashboard (typically `http://localhost:5155`).

---

## Home dashboard

Landing page summarising the four core registration decisions from [IDEA.md](../IDEA.md).

![Home dashboard — four core decisions and link to registration cases](./screenshots/05-home-dashboard.png)

---

## Registration case list

Officers browse open first-registration procedures. The table shows visit reason, intake status, opened date, and **intake progress** (`n/3` with icons for identity, legal residence, and address). Search and pagination are built in.

![Registration case list with search and status badges](./screenshots/01-registration-case-list.png)

**Capabilities:** `ListRegistrationCases`, case search, officer role switcher in the app bar.

---

## Open a new case

The **New case** button opens a dialog to choose a visit reason. The current officer is assigned automatically.

![Open registration case dialog](./screenshots/04-open-registration-case-dialog.png)

**Capabilities:** `OpenRegistrationCase` — visit reasons include first registration, EU citizen registration, and change of address.

---

## Record identity (intake step)

A freshly opened case starts on the identity step. The completeness checklist tracks progress across identity, legal residence, and address. Later steps stay visible; address and household require identity first.

![Identity intake form on a new case](./screenshots/03-identity-intake-form.png)

**Capabilities:** `RecordIdentity` — given name, family name, date of birth, nationality; checklist flag `IdentityEstablished`.

---

## Legal residence — EU citizen

Once identity is saved, the legal residence section unlocks. EU citizens can be classified without a permit; the checklist marks legal residence as established when policy rules pass.

![Case detail — identity and EU residence category](./screenshots/02-registration-case-detail.png)

**Capabilities:** `SetResidenceCategory`, optional `RecordImmigrationDecision` stub, completeness checklist.

---

## Legal residence — non-EU worker with permit

Non-EU categories require a residence permit. Officers can attach supporting documents and review what is already on file.

![Full case — identity, non-EU permit, address checklist](./screenshots/06-full-case-with-permit-and-documents.png)

**Capabilities:**

| Area | Use cases |
|------|-----------|
| Identity | `RecordIdentity`, `CorrectIdentity` (edit) |
| Residence | `SetResidenceCategory`, `RecordResidencePermit`, `RecordImmigrationDecision` (all editable after save) |
| Documents | `AttachDocument`, `RemoveDocument` |
| Policies | `EuCitizenPolicy`, `NonEuWorkerPolicy`, `StudentPolicy` — checklist `LegalResidenceEstablished` |

---

## Address declaration (1030 Schaerbeek)

First registration at this desk requires a domicile **within Schaerbeek**. The municipality is fixed to `1030 Schaerbeek`; officers pick a street from the local register, then enter number and box.

![Address declaration form — fixed municipality and street autocomplete](./screenshots/10-address-declaration-form.png)

**Capabilities:** `DeclareAddress` — sets checklist `AddressDeclared`; `RecordHousingSituation` (tenant, owner, …). Both support edit after save.

---

## Address, household & civil status (complete case)

A fully progressed case shows declared domicile, housing situation, household members, and civil status with marriage details when applicable.

![Declared address and housing situation](./screenshots/08-address-household-civil-status.png)

![Household members, civil status, and document upload](./screenshots/09-civil-status-and-household.png)

**Capabilities:**

| Area | Use cases |
|------|-----------|
| Address | `DeclareAddress`, `RecordHousingSituation` |
| Household | `RecordHouseholdComposition` — add/remove members (spouse, child, …) |
| Civil status | `RecordCivilStatus` — conditional marriage fields when married |
| Reference data | Schaerbeek street autocomplete (`GET /api/registration/streets?postalCode=1030`) |

---

## Document panel — upload, preview & download

The case detail page uses a **two-column layout**: collapsible intake sections on the left (`AppCollapsibleSection`), and a **sticky document panel** on the right (on wide screens). The panel is implemented once in `Municipal/Components/CaseDocumentPanel.razor` and reused by registration and birth declaration via thin feature wrappers.

Officers upload files, click a row to preview PDFs and images inline (`AppFilePreview`), or download.

![Document upload, immigration decision, and attached files](./screenshots/07-document-upload-and-attached-files.png)

**Capabilities:**

| Area | Use cases |
|------|-----------|
| Upload | `AttachDocument` — upload form passed as `UploadContent` slot |
| Preview | `DownloadDocument` — stream via `GET …/documents/{id}`; `AppFilePreview` for PDF/image |
| Remove | `RemoveDocument` — per-row delete with confirmation |
| Shared UI | `CaseDocumentPanel`, `CaseDocumentItem` — see [MUNICIPAL-UI.md](./MUNICIPAL-UI.md) |

---

## Officer decision & case locking

Population officers review cases against the **four core questions** checklist (`OfficerDecisionChecklist` → `AppChecklist`). Approve, reject, suspend, and confirm registration from the decision section — only when `CanEdit` is true (lock holder).

Case locking (Phase 8.1) prevents two officers editing the same case:

- **`CaseLockBar`** — read-only warning when another officer holds the lock (review only; no “take case” hint).
- **`CaseLockActions`** — header buttons on birth declaration, change of address, and identity document requests:
  - **Take case** when `!CanEdit && !IsReadOnlyDueToLock` (available to claim — not while locked to a colleague).
  - **Release lock** when `CanEdit` (lock holder only).

Registration case detail uses the same rules inline. When read-only, intake steps show summaries or “Not recorded yet.” placeholders — no forms, uploads, or NR-search actions. See [phase-8.1](./phases/phase-8.1-role-boundaries-case-locking.md#read-only-case-detail-lock-held-by-colleague).

**Capabilities:** `GetCaseReviewChecklist`, `ApproveCase`, `RejectCase`, `SuspendCase`, `ConfirmRegistration`, `ClaimRegistrationCase`, `ReleaseCaseLock` (and equivalent claim/release slices per workflow).

---

## Birth declaration

Separate workflow for newborn registration — child details, parent NR link (`NationalRegisterSearchForm`), medical document, household domicile, officer decision, confirm.

Route: `/birth-declarations` → `/birth-declarations/{id}`.

**Capabilities:** `BirthDeclarationCase` aggregate, `RecordChildDetails`, `LinkParent`, `AttachBirthDocument`, `SetDeclarationHousehold`, `ConfirmBirthDeclaration`. See [phase-12-birth-declaration.md](./phases/phase-12-birth-declaration.md).

---

## Shared Municipal UI layer

Cross-feature components extracted from Registration, Birth declaration, and Change of address live in `Web/Municipal/`. Feature slices pass handlers and DTOs via parameters — no slice-to-slice Razor dependencies.

| Component | Used for |
|-----------|----------|
| `BelgianAddressFields` | Domicile input (street autocomplete, number, box) |
| `NationalRegisterSearchForm` | NR/BIS person search in dialogs |
| `CaseDocumentPanel` | Document list + inline preview |
| `OfficerDecisionChecklist` | Four core questions display |
| `CaseLockBar` / `CaseLockActions` | Officer case locking UX |

Full catalogue: [MUNICIPAL-UI.md](./MUNICIPAL-UI.md).

Form validation uses shared `BelgianAddressRules` and `FluentMudValidation.ToMudValidateValue` to wire slice validators into MudForm inline errors. Date fields use `AppDateField` (`DateOnly` + FluentValidation).

---

## Navigation & shell

| Item | Route | Purpose |
|------|-------|---------|
| Review dashboard | `/registration/review-dashboard` | Population officer landing page and actionable queue |
| Registration cases | `/registration/cases` | First-registration case list and detail |
| Birth declarations | `/birth-declarations` | Newborn registration cases (Phase 12) |
| Change of address | `/change-of-address` | Intra-municipal moves (Phase 13 — in progress) |
| Police verifications | `/registration/police-verifications` | Police clerk pending queue (Phase 6) |
| Outbound notifications | `/administration/outbound-notifications` | Stub notification log (Phase 8) |
| Design system | `/design-system` | Schaerbeek UI kit showcase (Development only) |

Reception officers see **Home** (`/`) and **New case**. Population officers are redirected from `/` to the review dashboard.

The app bar shows a fake officer identity (**Marie Dupont**) and a role switcher for development (`PopulationOfficer`, etc.).

---

## What is not built yet

See [ROADMAP.md](./ROADMAP.md) for upcoming phases **14–18** (passport/ID request, reporting, person file, amendments, optional exception depth). Phase 13 (change of address) is underway.
