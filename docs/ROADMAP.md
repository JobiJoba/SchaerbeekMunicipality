# Roadmap

Phased plan to grow the educational municipality from an empty repository to a rich simulation of Belgian population registration. Each phase delivers **demoable** functionality and **tests**.

Estimates are rough learning-project sizing, not calendar commitments.

---

## Phase 0 — Foundation ✅

**Status:** Complete — see [phases/phase-0-foundation.md](./phases/phase-0-foundation.md) for full delivery notes.

**Goal:** Runnable solution skeleton with architecture enforced by structure.

**Deliverables:** Solution scaffold, Aspire + PostgreSQL, ServiceDefaults health, Blazor/MudBlazor shell, registration API group, EF migration, `RegistrationCase` stub, fake officer auth, CI.

**Demo:** Run AppHost → Aspire dashboard shows Web + PostgreSQL healthy; empty case list; role switcher in nav.

**Tests:** Domain factory test + integration smoke test on `/health` (SQLite, no AppHost).

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

## Phase 3 — Design system & UI kit (Schaerbeek visual identity)

**Goal:** Reverse-engineer the design language of [www.1030.be](https://www.1030.be/fr) into an enterprise-grade MudBlazor design system so every subsequent phase builds pages from a consistent, branded UI kit instead of raw MudBlazor defaults.

**Plan:** See [phases/phase-3-design-system.md](./phases/phase-3-design-system.md). Full specification lives in [design-system/](./design-system/).

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

## Phase 4 — Address & household (IDEA Phases 5–8, 9–10)

**Goal:** Declare domicile and household; civil status.

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

## Phase 5 — National Register search & BIS (IDEA Phases 13–14)

**Goal:** Duplicate detection and BIS handling.

**Slices:**

- `SearchNationalRegister`
- `LinkExistingPerson` / `CreateNewPerson`
- `ConvertBisNumber` (simplified)

**Infrastructure:**

- `national_register_persons` seed table
- Search by name + birth date + fuzzy match

**UI:**

- Search dialog before creating person
- “Possible duplicate” warning banner on case

**Demo:** Search finds existing BIS record; officer links instead of duplicate create.

**Tests:**

- Duplicate link prevents second NR assignment
- Search returns scored matches

---

## Phase 6 — Police verification loop (IDEA Phases 15–17)

**Goal:** Async residence check with separate police role.

**Slices:**

- `RequestPoliceVerification`
- `ListPendingPoliceVerifications` (police clerk)
- `RecordPoliceResult`

**Domain:**

- `PoliceVerificationRequest` aggregate
- Case status `AwaitingPoliceVerification`

**UI:**

- Population officer: “Send to police” action
- Police clerk portal: pending list, result form (confirmed / not found / …)

**Demo:** Full wait loop — request → police confirms → case returns to review.

**Tests:**

- Cannot approve with negative police result
- Second visit (`AttemptNumber`) allowed when incomplete

---

## Phase 7 — Decision & registration (IDEA Phases 18–20)

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
