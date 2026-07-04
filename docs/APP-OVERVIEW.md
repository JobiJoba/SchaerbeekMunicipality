# Application overview

Visual tour of the Schaerbeek Population Department back-office as of **Phase 2** (residence category & permits), with intake correction UI from **Phase 2.1** in progress.

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

Officers browse open first-registration procedures. The table shows visit reason, intake status, opened date, and whether identity has been established. Search and pagination are built in.

![Registration case list with search and status badges](./screenshots/01-registration-case-list.png)

**Capabilities:** `ListRegistrationCases`, case search, officer role switcher in the app bar.

---

## Open a new case

The **New case** button opens a dialog to choose a visit reason. The current officer is assigned automatically.

![Open registration case dialog](./screenshots/04-open-registration-case-dialog.png)

**Capabilities:** `OpenRegistrationCase` — visit reasons include first registration, EU citizen registration, and change of address.

---

## Record identity (intake step)

A freshly opened case starts on the identity step. The completeness checklist tracks progress across identity, legal residence, and address. Later steps stay locked until identity is recorded.

![Identity intake form on a new case](./screenshots/03-identity-intake-form.png)

**Capabilities:** `RecordIdentity` — given name, family name, date of birth, nationality; checklist flag `IdentityEstablished`.

---

## Legal residence — EU citizen

Once identity is saved, the legal residence section unlocks. EU citizens can be classified without a permit; the checklist marks legal residence as established when policy rules pass.

![Case detail — identity and EU residence category](./screenshots/02-registration-case-detail.png)

**Capabilities:** `SetResidenceCategory`, optional `RecordImmigrationDecision` stub, completeness checklist.

---

## Legal residence — non-EU worker with permit & documents

Non-EU categories require a residence permit. Officers can attach supporting documents (local file upload) and review what is already on file.

![Full case — non-EU worker, B card permit, attached document](./screenshots/06-full-case-with-permit-and-documents.png)

**Capabilities:**

| Area | Use cases |
|------|-----------|
| Identity | `RecordIdentity`, `CorrectIdentity` (edit) |
| Residence | `SetResidenceCategory`, `RecordResidencePermit`, `RecordImmigrationDecision` (all editable after save) |
| Documents | `AttachDocument`, `RemoveDocument` |
| Policies | `EuCitizenPolicy`, `NonEuWorkerPolicy`, `StudentPolicy` — checklist `LegalResidenceEstablished` |

### Document upload & attached files

Immigration decision stub, file upload by document type, and a list of attached documents with remove action.

![Document upload, immigration decision, and attached files](./screenshots/07-document-upload-and-attached-files.png)

---

## Navigation & shell

| Item | Route | Purpose |
|------|-------|---------|
| Home | `/home` | Dashboard and workflow overview |
| Registration cases | `/registration/cases` | Case list and detail wizard |
| Design system | `/design-system` | MudBlazor component showcase (Phase 3) |

The app bar shows a fake officer identity (**Marie Dupont**) and a role switcher for development (`PopulationOfficer`, etc.).

---

## What is not built yet

See [ROADMAP.md](./ROADMAP.md) for upcoming phases:

- **Phase 4** — address declaration and household
- **Phase 5+** — National Register search, police verification, final decision, certificates
- **Phase 3** — Schaerbeek-branded design system (current UI uses default MudBlazor theme)
