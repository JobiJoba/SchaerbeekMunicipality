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
| 8 | Certificates & outbound stubs | ✅ Complete |
| 8.1 | Role boundaries, case locking & officer UX | ✅ Complete |
| 9 | Exception scenarios | ✅ Complete |
| 10 | Azure deployment | ✅ Complete |
| 11 | Extended municipality (scope definition) | ✅ Complete |
| 12 | Birth declaration | ✅ Complete |
| 13 | Change of address | ✅ Complete |
| 14 | Passport / ID card request | ✅ Complete |
| 15 | Reporting dashboard | ✅ Complete |
| 16 | Person file (read model) | ✅ Complete |
| 17 | Post-registration amendments | ✅ Complete |
| 18 | Remaining exception scenarios | ✅ Complete |
| 19 | Life events & citizen services (scope) | ✅ Complete |
| 20 | Death declaration & radiation | Planned |
| 21 | Certificate desk (on-demand) | Planned |
| 22 | Inter-municipal move & emigration | Planned |
| 23 | Residence card production | Planned |
| 24 | Marriage / partnership declaration | Planned |
| 25 | Investigation workspace | Planned |
| 26 | Cross-workflow polish | Planned |

Delivery notes for completed phases: [phases/](./phases/). Second-horizon plan: [phase-19](./phases/phase-19-life-events-citizen-services.md).

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

## Phase 8 — Certificates & outbound stubs (IDEA Phases 21–23) ✅

**Status:** Complete — certificates, household composition, and outbound notification log implemented in `Features/Registration/`.

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

**Demo:** Reception opens case → population sees it as unassigned on dashboard → claims and edits → colleague read-only (no take/edit actions) until lock released → then colleague can take case.

**Tests:** `RegistrationCaseLockingTests`, `RoleBoundariesAndCaseLockingTests`.

---

## Phase 9 — Exception scenarios (IDEA “Major Exceptions”) ✅

**Goal:** Teach edge cases as explicit workflows.

**Status:** Complete (high + medium slices) — see [phases/phase-9-exception-scenarios.md](./phases/phase-9-exception-scenarios.md).

**Incremental slices:**

| Feature | Priority | Status |
|---------|----------|--------|
| Missing birth certificate → suspend | High | Done |
| Marriage not recognised | Medium | Done |
| Duplicate identity investigation flag | High | Done |
| Illegal stay → reject + refer | Medium | Done |
| Refugee / temporary protection path | Medium | Done |
| EU vs non-EU policy differences (deepen) | High | Done |
| Diplomat separate rules | Low | Deferred |
| Homeless reference address | Low | Deferred |

Each adds domain rules + UI branch + dedicated tests.

---

## Phase 10 — Azure deployment ✅

**Status:** Complete — see [phases/phase-10-azure-deployment.md](./phases/phase-10-azure-deployment.md).

**Goal:** Containerized Azure deployment with production hardening; local dev unchanged on Aspire + PostgreSQL.

**Deliverables:**

- [x] `Dockerfile` + GHCR publish workflow
- [x] Azure Container Apps — **SQLite ephemeral** (default production)
- [x] Azure Container Apps — **PostgreSQL optional** (`deploy/azure/postgres/`)
- [x] Snake case naming (PostgreSQL — existing)
- [x] OpenTelemetry via ServiceDefaults (existing)
- [x] `AddDbContextCheck` on `/health`
- [x] `/health` and `/alive` in Production (+ optional `HEALTH_CHECK_API_KEY`)
- [x] OpenAPI at `/openapi/v1.json` in Production
- [x] `MigrateAsync` for Npgsql in all environments

**Deferred:** full Testcontainers PG integration suite; `aspire publish` / ACR (GHCR + Bicep instead).

---

## Phase 11 — Extended municipality (scope definition) ✅

**Status:** Complete — see [phases/phase-11-extended-municipality.md](./phases/phase-11-extended-municipality.md).

**Goal:** Identify and sequence follow-on municipal workflows beyond first registration.

The original optional backlog is split into **Phases 12–18**. Multi-language UI (FR / NL) is **not planned**.

A further backlog (life events & citizen services) is defined in **Phase 19** and sequenced as **Phases 20–26**.

---

## Phase 12 — Birth declaration ✅

**Status:** Complete — see [phases/phase-12-birth-declaration.md](./phases/phase-12-birth-declaration.md).

**Goal:** Register a newborn child — new `BirthDeclaration` bounded context, separate from first-registration `RegistrationCase`.

**Deliverables:**

- `BirthDeclarationCase` aggregate, checklist, claim/lock, confirm/reject/suspend
- EF migration `Phase12BirthDeclaration`; API `/api/birth-declarations/*`
- Blazor pages `/birth-declarations` (list + detail) with intake sections and decision panel
- Reception routing: visit reason Birth declaration opens birth case
- Review dashboard: birth unassigned + ready-for-confirmation tiles; unified actionable queue
- Population officer landing: `/` → review dashboard

**Demo:** Reception opens birth case → population officer sees it on dashboard → records child, links parent, attaches medical form → confirms → stub NR + notification log.

**Tests:** `BirthDeclarationCaseTests`, `BirthDeclarationTests`, dashboard handoff in `RoleBoundariesAndCaseLockingTests`.

---

## Phase 13 — Change of address ✅

**Status:** Complete — see [phases/phase-13-change-of-address.md](./phases/phase-13-change-of-address.md).

**Goal:** Registered resident moves within the municipality — `ChangeOfAddressCase` aggregate, new address, optional police verification, domicile update.

**Demo:** NR lookup → new Schaerbeek address → police confirms → person domicile updated.

**Tests:** Domain and integration tests cover the full workflow; E2E covers NR search → open case.

---

## Phase 14 — Passport / ID card request ✅

**Status:** Complete — see [phases/phase-14-passport-id-request.md](./phases/phase-14-passport-id-request.md).

**Goal:** Simplified passport / eID request workflow for registered persons — photo, fee stub, production status, issue.

**Delivered:**

- `DocumentRequestCase` aggregate with linear status workflow (`Submitted` → `InProduction` → `ReadyForCollection` → `Issued` / `Cancelled`)
- EF migration `Phase14PassportIdRequest`; API `/api/identity-documents/*`
- Blazor pages `/identity-documents/requests` (list + detail) with photo upload, fee stub, status actions, case locking
- Reception routing: `VisitReason.PassportRenewal` → identity documents flow
- `DocumentType.ApplicantPhoto` + `AdministrativeDocument` extension for document requests

**Demo:** Registered citizen requests eID renewal → photo attached → fee paid → advanced through production → issued with stub document number `BE-YYYY-XXXX`.

**Tests:** `DocumentRequestCaseTests`, `DocumentRequestTests` (open guards, issue without photo, happy path); E2E `DocumentRequestE2ETests` (NR search → open case).

---

## Phase 15 — Reporting dashboard ✅

**Status:** Complete — see [phases/phase-15-reporting-dashboard.md](./phases/phase-15-reporting-dashboard.md).

**Goal:** Operational analytics — registrations per month, birth declarations, average police wait time, outcome breakdown.

**Demo:** Switch to Sophie Lambert (Back office) → Reports page with KPI tiles and monthly volume chart. Population officers see the same page alongside their operational nav.

---

## Phase 16 — Person file (read model) ✅

**Status:** Complete — see [phases/phase-16-person-file.md](./phases/phase-16-person-file.md).

**Goal:** Consolidated citizen profile — identity, household, addresses, cases, certificates in one read-only view.

**Demo:** Person lookup → search Sofia Nguyen → open person file with tabs; or use **View person file** from any linked case detail page.

---

## Phase 17 — Post-registration amendments ✅

**Status:** Complete — see [phases/phase-17-post-registration-amendments.md](./phases/phase-17-post-registration-amendments.md).

**Goal:** Correct register data after registration (name, civil status, nationality) with approval workflow and audit trail.

**Demo:** Legal name change with supporting judgment → approve → apply → person file updated.

---

## Phase 18 — Remaining exception scenarios ✅

**Status:** Complete — see [phases/phase-18-remaining-exception-scenarios.md](./phases/phase-18-remaining-exception-scenarios.md).

**Goal:** Phase 9 deferred slices — diplomat register rules and homeless reference address.

**Demo:** Diplomat path to special register (police waived); reference address satisfies domicile checklist; police still applies to reference site.

---

## Phase 19 — Life events & citizen services (scope definition) ✅

**Status:** Complete — see [phases/phase-19-life-events-citizen-services.md](./phases/phase-19-life-events-citizen-services.md).

**Goal:** Identify and sequence the **second horizon** after Phases 12–18 — death/radiation, certificate desk, inter-municipal departure, residence cards, marriage declaration, investigations, and cross-workflow polish.

The backlog is split into **Phases 20–26**. Multi-language UI (FR / NL) remains **not planned**.

---

## Phase 20 — Death declaration & radiation

**Status:** Planned — see [phases/phase-20-death-declaration-radiation.md](./phases/phase-20-death-declaration-radiation.md).

**Goal:** Lifecycle counterpart to birth — record death, radiate person from the active register, update household, stub outbound notifications.

**Demo:** NR lookup → death case → confirm → person file shows deceased; household membership removed.

**Tests:** Domain guards + integration confirm path; review dashboard includes death cases.

---

## Phase 21 — Certificate desk (on-demand)

**Status:** Planned — see [phases/phase-21-certificate-desk.md](./phases/phase-21-certificate-desk.md).

**Goal:** Issue residence / household / register-extract certificates from current person state — not only after a registration case closes.

**Demo:** Person file → issue household composition PDF → issuance appears in history and desk list.

**Tests:** Block unregistered persons; composition reflects post-COA household.

---

## Phase 22 — Inter-municipal move & emigration

**Status:** Planned — see [phases/phase-22-inter-municipal-move-emigration.md](./phases/phase-22-inter-municipal-move-emigration.md).

**Goal:** Residents leaving Schaerbeek — transfer to another Belgian commune or emigrate abroad (Phase 13 follow-up).

**Demo:** Move to Ixelles → domicile cleared + destination notification stub; emigrate → status blocks new residence certificates.

**Tests:** Regression on intra-municipal COA; departure clears domicile.

---

## Phase 23 — Residence card production

**Status:** Planned — see [phases/phase-23-residence-card-production.md](./phases/phase-23-residence-card-production.md).

**Goal:** IDEA Phase 21 — order / produce / collect residence cards (distinct from passport/eID in Phase 14).

**Demo:** Non-EU registration complete → residence card request → production → issued stub with PIN/PUK note.

**Tests:** Issue requires ready-for-collection; card type suggestion from permit/category.

---

## Phase 24 — Marriage / partnership declaration

**Status:** Planned — see [phases/phase-24-marriage-partnership-declaration.md](./phases/phase-24-marriage-partnership-declaration.md).

**Goal:** First-class civil-status life event for two parties, including foreign-act recognition (Phase 9).

**Demo:** Two registered residents marry → both person files updated; foreign certificate → suspend until recognised.

**Tests:** Confirm updates both parties; not-recognised path leaves status unchanged.

---

## Phase 25 — Investigation workspace

**Status:** Planned — see [phases/phase-25-investigation-workspace.md](./phases/phase-25-investigation-workspace.md).

**Goal:** Actionable queue for Phase 9 duplicate-identity flags — compare, link, dismiss, or escalate.

**Demo:** Duplicate warning → open investigation → link existing NR person → registration unblocked.

**Tests:** Approve blocked while investigation open; dismiss requires reason.

---

## Phase 26 — Cross-workflow polish & demo hardening

**Status:** Planned — see [phases/phase-26-cross-workflow-polish.md](./phases/phase-26-cross-workflow-polish.md).

**Goal:** Deferred UX across contexts — unregistered parent banner, person-file family links, reference→domicile COA, NR unlink/re-link, Phase 18 demo seeds.

**Demo:** Birth parent CTA → registration; reference-address resident converts to real domicile via COA; wrong NR link corrected.

**Tests:** One integration test per selected polish slice; existing suites stay green.

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
