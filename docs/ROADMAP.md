# Roadmap

Phased plan to grow the educational municipality from an empty repository to a rich simulation of Belgian population registration. Each phase delivers **demoable** functionality and **tests**.

Estimates are rough learning-project sizing, not calendar commitments.

## Progress

| Phase | Title | Status |
|-------|-------|--------|
| 0 | Foundation | ✅ Complete |
| 0.1 | Tooling hygiene | ✅ Complete |
| 1 | Case intake & identity | ✅ Complete |
| 2 | Residence category & permits | ✅ Complete |
| 2.1 | Intake corrections | ✅ Complete |
| 3 | Design system & UI kit | ✅ Complete |
| 4 | Address & household | ✅ Complete |
| 4.1 | Document preview & case UX | ✅ Complete |
| 5 | National Register search & BIS | ✅ Complete |
| 6 | Police verification loop | ✅ Complete |
| 6.1 | Collapsible intake sections | ✅ Complete |
| 7 | Decision & registration | ✅ Complete |
| 8 | Certificates & outbound stubs | In progress |
| 8.1 | Role boundaries, case locking & officer UX | ✅ Complete |
| 9+ | Exception scenarios, … | Planned |

Delivery notes for completed phases: [phases/](./phases/).

---

## Phase 0 — Foundation ✅

**Status:** Complete — see [phases/phase-0-foundation.md](./phases/phase-0-foundation.md) for full delivery notes.

**Goal:** Runnable solution skeleton with architecture enforced by structure.

**Deliverables:** Solution scaffold, Aspire + PostgreSQL, ServiceDefaults health, Blazor/MudBlazor shell, registration API group, EF migration, `RegistrationCase` stub, fake officer auth, CI.

**Demo:** Run AppHost → Aspire dashboard shows Web + PostgreSQL healthy; empty case list; role switcher in nav.

**Tests:** Domain factory test + integration smoke test on `/health` (SQLite, no AppHost).

---

## Phase 0.1 — Tooling hygiene ✅

**Status:** Complete — see [phases/phase-0.1-tooling-hygiene.md](./phases/phase-0.1-tooling-hygiene.md).

**Goal:** Modernize solution and dependency management before business slices expand.

**Deliverables:** SLNX solution format, `Directory.Packages.props` central package management, legacy `.sln` removed.

**Demo:** `dotnet restore` / `dotnet build` unchanged for developers; all package versions live in one manifest.

**Tests:** Existing Phase 0 tests stay green (no behavior change).

---

## Phase 1 — Case intake & identity (IDEA Phases 1–2, 12 partial) ✅

**Status:** Complete — see [phases/phase-1-case-intake-identity.md](./phases/phase-1-case-intake-identity.md) for full delivery notes.

**Goal:** Open a registration case and record identity.

**Slices:**

- `OpenRegistrationCase`
- `ListRegistrationCases` / `GetRegistrationCase`
- `RecordIdentity`
- `AttachDocument` (metadata + local file upload)

**Domain:**

- `RegistrationCase` lifecycle: `Intake` status + completeness checklist (`IdentityEstablished` first)
- `Person` aggregate basics
- `AdministrativeDocument`

**UI:**

- Case list page
- New case dialog (visit reason, assign officer)
- Case detail — identity step (MudForm)
- Document upload component

**Demo:** Officer opens case, enters name/DOB/nationality, uploads passport scan.

**Tests:**

- Domain: invalid transitions throw; checklist flags computed correctly
- Integration: open case + record identity persists
- Validator: required fields
- Migrations: Testcontainers PostgreSQL applies all migrations (separate CI job, `Category=PostgreSQL` — starts here because this phase creates the first real migration)

---

## Phase 2 — Residence category & permits (IDEA Phases 3–4, 11) ✅

**Status:** Complete — see [phases/phase-2-residence-category-permits.md](./phases/phase-2-residence-category-permits.md).

**Goal:** Classify legal stay and attach permit evidence.

**Slices:**

- `SetResidenceCategory`
- `RecordResidencePermit`
- `RecordImmigrationDecision` (stub external decision)

**Domain:**

- `ResidenceCategory` enum + seed data
- `ResidencePermit` entity on case
- Policy classes: `EuCitizenPolicy`, `NonEuWorkerPolicy`, `StudentPolicy` (minimal rules)

**UI:**

- Residence step on case wizard
- Permit type picker with validity dates

**Demo:** Non-EU worker case with B card reference; EU citizen with lighter requirements.

**Tests:**

- Policy rejects missing permit for non-EU
- EU path skips certain document requirements

---

## Phase 2.1 — Intake corrections ✅

**Status:** Complete — see [phases/phase-2.1-intake-corrections.md](./phases/phase-2.1-intake-corrections.md).

**Goal:** Let officers correct any intake step after it has been saved — identity, legal residence (category, permit, decision), and documents — without abandoning the case or losing later progress.

**Why now:** Phases 1–2 only support first-time recording; the UI locks sections after save. [ADR-0004](./adr/0004-checklist-over-linear-state-machine.md) and [ARCHITECTURE.md](./ARCHITECTURE.md) already assume flexible intake with corrections during `Intake` and `UnderReview`.

**Slices:**

- `CorrectIdentity` (new domain method + handler + endpoint)
- Legal residence correction — expose edit UI for category, permit, and immigration decision (permit/decision handlers largely upsert already)
- `RemoveDocument` (minimum viable document correction)

**Domain:**

- `Person.Update(IdentityDetails)`; `RegistrationCase.CorrectIdentity(...)`
- Shared `EnsureIntakeDataEditable()` guard (`Intake` + `UnderReview`)
- Post-correction checklist re-evaluation (e.g. identity/nationality change → re-run residence policy)
- Cascade rules when category and permit disagree

**UI:**

- Edit button + pre-filled form on every saved intake section (design-system edit form pattern)
- Policy warning when correction leaves checklist invalid until a related step is fixed

**Demo:** Record identity → start residence → fix wrong name; change non-EU category → update permit; remove and re-attach wrong document.

**Tests:**

- Correct identity with residence already set; nationality change clears or restores `LegalResidenceEstablished`
- Category change with incompatible permit → policy failure until permit corrected
- Correction blocked after approval
- Document remove persists and updates storage

**Carries forward:** Phase 4+ intake slices must ship record **and** correction in the same phase (see phase doc).

---

## Phase 3 — Design system & UI kit (Schaerbeek visual identity) ✅

**Status:** Complete — see [phases/phase-3-design-system.md](./phases/phase-3-design-system.md). Full specification lives in [design-system/](./design-system/).

**Goal:** Reverse-engineer the design language of [www.1030.be](https://www.1030.be/fr) into an enterprise-grade MudBlazor design system so every subsequent phase builds pages from a consistent, branded UI kit instead of raw MudBlazor defaults.

**Slices (infrastructure-style, no domain changes):**

- `SchaerbeekTheme` — production `MudTheme` (palette, typography, layout, shadows) replacing the inline theme in `MainLayout`
- Design tokens (`SchaerbeekColors`, `SchaerbeekSpacing`, `SchaerbeekElevation`, `SchaerbeekMotion`, `SchaerbeekLayout`)
- `DesignSystem/` component library — `App*` wrapper components (`AppPage`, `AppPageHeader`, `AppCard`, `AppDataTable`, `AppStatusChip`, `AppEmptyState`, `AppConfirmDialog`, …)
- Rebranded `MainLayout` (app bar, drawer, footer) following the 1030.be identity
- Minimal `app.css` rewrite (fonts, focus ring, print rules only)
- Retrofit existing Phase 1 pages (case list, case detail, dialogs) onto the UI kit

**Docs:**

- Design system guide: tokens, theme, accessibility, component catalogue, page templates, developer guidelines — [design-system/](./design-system/)

**Demo:** Case list and case detail pages restyled in Schaerbeek identity; a style-guide page (`/design-system`) showcasing every wrapper component.

**Tests:**

- bUnit: wrapper components render expected MudBlazor structure and parameters
- Contrast assertions on token pairs (WCAG AA) as plain unit tests
- Existing integration tests stay green (no behavior change)

---

## Phase 4 — Address & household (IDEA Phases 5–8, 9–10) ✅

**Status:** Complete — see [phases/phase-4-address-household.md](./phases/phase-4-address-household.md).

**Goal:** Declare domicile and household; civil status.

**Note:** Ship each slice with a **correction path** in the same phase (record + edit UI). See [Phase 2.1](./phases/phase-2.1-intake-corrections.md) for the established pattern.

**Slices:**

- `DeclareAddress`
- `RecordHousingSituation`
- `RecordHouseholdComposition`
- `RecordCivilStatus`

**Domain:**

- `BelgianAddress` value object (validation)
- `Household` aggregate
- Seed Schaerbeek streets / postal codes

**UI:**

- Address form with municipality autocomplete
- Household member list (add/remove)
- Civil status with conditional marriage fields

**Demo:** Complete address + tenant + spouse + child link.

**Tests:**

- Invalid postal code rejected
- Cannot declare address without identity recorded

---

## Phase 4.1 — Document preview & case UX ✅

**Status:** Complete — see [phases/phase-4.1-document-preview-case-ux.md](./phases/phase-4.1-document-preview-case-ux.md).

**Goal:** Preview and download attached documents inline; improve case list and detail layout for day-to-day intake work.

**Why now:** Phase 4 lengthened the case detail page; officers need evidence visible while filling address and household steps. The case list still only showed identity progress.

**Slices:**

- `DownloadDocument` (read stream from `IDocumentStorage`)

**Infrastructure:**

- `IDocumentStorage.OpenReadAsync`
- `GET /api/registration/cases/{id}/documents/{documentId}` with range support for PDFs

**UI:**

- `RegistrationCaseDocumentPanel` — sticky right column on case detail (upload, list, inline preview, fullscreen dialog, download)
- `RegistrationCaseChecklistProgress` — `n/3` + icons on case list (identity, legal residence, address)
- Case detail two-column layout (`MaxWidth.False`, intake left / documents right)

**Demo:** Open a case with a PDF scan → click to preview in the sidebar → expand fullscreen → download; case list shows `2/3` progress.

**Tests:** Existing suite stays green (77 tests); no new automated tests in this phase.

---

## Phase 5 — National Register search & BIS (IDEA Phases 13–14) ✅

**Status:** Complete — see [phases/phase-5-national-register-search-bis.md](./phases/phase-5-national-register-search-bis.md).

**Goal:** Duplicate detection and BIS handling.

**Slices:**

- `SearchNationalRegister`
- `LinkExistingPerson` / `RecordIdentity` (create new)
- `ConvertBisNumber` (simplified)

**Infrastructure:**

- `national_register_persons` seed table
- Search by name + birth date + fuzzy match
- **Partial search:** optional criteria (any of given name, family name, birth date)

**UI:**

- Search dialog before creating person (multiple ranked results; link per row)
- “Possible duplicate” warning banner on case

**Demo:** Search `Marie` → link BIS record; or record Amélie Bernard manually → duplicate warning. Convert BIS to stub NR.

**Tests:**

- Duplicate link prevents second NR assignment
- Search returns scored matches (including partial single-field queries)

---

## Phase 6 — Police verification loop (IDEA Phases 15–17) ✅

**Status:** Complete — see [phases/phase-6-police-verification-loop.md](./phases/phase-6-police-verification-loop.md).

**Goal:** Async residence check with separate police role.

**Slices:**

- `RequestPoliceVerification`
- `ListPendingPoliceVerifications` (police clerk)
- `RecordPoliceResult`

**Domain:**

- `PoliceVerificationRequest` aggregate
- Case status `AwaitingPoliceVerification`

**UI:**

- Population officer: “Send to police” action on case detail
- `PoliceVerificationSection` — pending visit + completed history (card or table)
- Police clerk portal: pending list, result form with optional notes
- Nav badge with pending count (police clerk role)

**Demo:** Full wait loop — request → police confirms with notes → case shows outcome on detail → second visit switches history to grid.

**Tests:**

- Cannot approve with negative police result (Phase 7)
- Second visit (`AttemptNumber`) allowed when incomplete
- `GetRegistrationCase` returns history with officer notes

---

## Phase 6.1 — Collapsible intake sections ✅

**Status:** Complete — see [phases/phase-6.1-collapsible-intake-sections.md](./phases/phase-6.1-collapsible-intake-sections.md).

**Goal:** Reduce scroll on the case detail page; collapse completed intake categories and auto-expand the next step.

**Why now:** Phase 6 lengthened the case detail page further; officers need to reach the current task without scrolling past every completed section.

**UI:**

- `AppCollapsibleSection` — design-system collapsible panel with header summary and status chip
- `RegistrationIntakeStepSummaries` — summary text, completion checks, default expansion
- Case detail accordion: Identity always expanded; legal residence → address → household → civil status → police verification
- Police awaiting alert moved into police section body

**Demo:** Open new case → only Identity expanded → record identity → Legal residence auto-expands → completed steps collapse with summaries in headers.

**Tests:** `RegistrationIntakeStepSummariesTests` (default expansion matrix).

---

## Phase 7 — Decision & registration (IDEA Phases 18–20) ✅

**Status:** Complete — see [phases/phase-7-decision-registration.md](./phases/phase-7-decision-registration.md).

**Goal:** Officer decision and official registration in correct register.

**Slices:**

- `GetCaseReviewChecklist` (computed readiness)
- `ApproveCase` / `RejectCase` / `SuspendCase`
- `ConfirmRegistration`
- `AssignNationalRegisterNumber`

**Domain:**

- `RegisterTarget` selection rules
- `RegistrationConfirmed` domain event
- Terminal states `Registered` and `Rejected`; `Suspended` as resumable pause

**UI:**

- Review dashboard with four core questions checklist
- Approve dialog (register type selection)
- Rejection/suspension reason

**Demo:** End-to-end first registration for EU citizen and non-EU worker.

**Tests:**

- Full happy-path integration test (open → register)
- Rejection path preserves audit trail

---

## Phase 8 — Certificates & outbound stubs (IDEA Phases 21–23)

**Goal:** Citizen-facing outputs and administration notifications.

**Slices:**

- `IssueResidenceCertificate`
- `IssueHouseholdComposition`
- `ListOutboundNotifications`

**Infrastructure:**

- PDF generation stub (simple HTML → PDF or printable Razor page)
- Notification log table

**UI:**

- “Print certificate” on completed cases
- Admin view of simulated outbound messages

**Demo:** Registration complete → certificate PDF → log shows “Notify tax administration.”

---

## Phase 8.1 — Role boundaries, case locking & officer UX ✅

**Status:** Complete — see [phases/phase-8.1-role-boundaries-case-locking.md](./phases/phase-8.1-role-boundaries-case-locking.md).

**Goal:** Enforce officer role boundaries and case edit locks; polish reception intake, review dashboard queue, case list filters, and case detail layout.

**Deliverables:**

- Claim / release lock on `RegistrationCase`; nullable assignee until claim
- `RegistrationCaseAuthorization` + guards on all registration handlers
- Reception **New case** page; population case list/detail with lock UX
- Unassigned intakes on review dashboard and case list quick filters
- Case history datagrid (collapsible); officer decision aligned with Identity card
- Demo officer URL persistence (`?demoOfficer=`) with port-safe role switching

**Demo:** Reception opens case → population sees it as unassigned on dashboard → claims and edits → colleague read-only until release.

**Tests:** `RegistrationCaseLockingTests`, `RoleBoundariesAndCaseLockingTests`.

---

## Phase 9 — Exception scenarios (IDEA “Major Exceptions”)

**Goal:** Teach edge cases as explicit workflows.

**Incremental slices:**

| Feature | Priority |
|---------|----------|
| Missing birth certificate → suspend | High |
| Marriage not recognised | Medium |
| Duplicate identity investigation flag | High |
| Illegal stay → reject + refer | Medium |
| Refugee / temporary protection path | Medium |
| Diplomat separate rules | Low |
| Homeless reference address | Low |
| EU vs non-EU policy differences (deepen) | High |

Each adds domain rules + UI branch + dedicated tests.

---

## Phase 10 — PostgreSQL hardening & Aspire deployment

**Goal:** Production habits and deployable manifests.

- [ ] PostgreSQL provider validated under Aspire (persistent volume, migrations on startup)
- [ ] Expand Testcontainers PostgreSQL coverage (full integration suite against PG, not just migrations)
- [ ] Snake case naming
- [ ] Structured logging via OpenTelemetry (ServiceDefaults)
- [ ] Database health check (`AddDbContextCheck`) wired into the existing `/health` pipeline
- [ ] Expose `/health` and `/alive` outside Development (ServiceDefaults maps them dev-only by default) with response caching and access restrictions
- [ ] `aspire publish` manifest for Azure Container Apps or Docker
- [ ] OpenAPI document published

---

## Phase 11 — Extended municipality (optional)

Ideas beyond first registration:

- Change of address within municipality
- Birth declaration
- Passport / ID card request workflow (simplified)
- Multi-language UI (FR / NL)
- Reporting dashboard (cases per month, average police wait time)
- Read model: consolidated **Person file** view

---

## Slice checklist template

When implementing any slice, complete:

```
[ ] Domain method(s) with tests
[ ] Handler + validator
[ ] Endpoint (if HTTP exposed)
[ ] EF configuration + migration
[ ] Blazor page or component
[ ] Integration test
[ ] Seed data if reference data needed
[ ] Glossary update if new terms
[ ] Correction path (domain + handler + UI edit) if slice records intake data — see Phase 2.1
```

---

## Suggested first sprint (after Phase 0)

1. Phase 0 foundation
2. Phase 1 through `RecordIdentity` only
3. One end-to-end Blazor demo with two test cases

That gives a tangible vertical slice through all layers before expanding horizontally.

---

## Related documents

- [phases/](./phases/) — detailed delivery notes per completed phase
- [design-system/](./design-system/README.md) — Schaerbeek design system & UI kit specification (Phase 3)
- [DOMAIN.md](./DOMAIN.md) — context and aggregate reference
- [ARCHITECTURE.md](./ARCHITECTURE.md) — slice conventions
- [TESTING.md](./TESTING.md) — test types per phase
