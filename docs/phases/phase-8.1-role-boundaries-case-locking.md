# Phase 8.1 — Role boundaries, case locking & officer UX

- **Status:** Complete
- **Completed:** July 2026
- **Goal:** Enforce officer role boundaries and exclusive case edit locks, then polish the day-to-day workflows for reception, population officers, and the review dashboard.

---

## Summary

Phase 8.1 delivers two layers:

1. **Access control** — roles are enforced in handlers and routes; population officers claim cases before editing; an expanded audit timeline records who did what.
2. **Officer UX** — reception intake page, case detail layout, review dashboard queue, case list filters, and demo role switching behave the way officers expect in daily use.

No new domain aggregates. Changes are domain rules (claim/lock), application guards, Blazor pages, and SQLite-safe listing.

---

## Problem statement

| Area | Before | Officer impact |
|------|--------|----------------|
| **Roles** | Nav switcher only; any role could hit any API | Reception could browse cases; police could open full case detail |
| **Assignment** | `AssignedOfficerId` set at open to whoever clicked create | Reception-created cases appeared “owned” before population picked them up |
| **Editing** | No lock; two population officers could edit the same case | Conflicting intake changes with no clear owner |
| **Audit** | Timeline showed decisions only | No record of open, claim, release, or police handoff |
| **Reception intake** | Bare form; Create disabled until validation ran | “First registration” pre-selected but button stayed grey |
| **Review dashboard** | Actionable queue ignored unassigned intakes | Reception handoffs invisible until browsing the full list |
| **Case list** | Search only | No one-click “my cases” or “unassigned” views |
| **Case detail** | History timeline in sidebar; decision box misaligned | Cluttered sidebar; decision panel floated above Identity card |
| **Demo roles** | Role switch from forbidden page dropped URL port | `https://localhost/…` instead of `https://localhost:5155/…` |

---

## Deliverables checklist

### Domain & application — role boundaries & locking

| Deliverable | Status | Notes |
|-------------|--------|-------|
| Nullable `AssignedOfficerId` until claim | Done | Open case no longer auto-assigns |
| `LockedByOfficerId` + `LockedAt` | Done | Exclusive edit lock |
| `Claim`, `ReleaseLock`, `EnsureEditableBy` | Done | On `RegistrationCase` |
| `CaseClaimResult` enum | Done | `NewlyClaimed`, `AlreadyHeld`, `Reclaimed` |
| `ClaimRegistrationCase` slice | Done | `POST …/cases/{id}/claim` |
| `ReleaseCaseLock` slice | Done | `POST …/cases/{id}/release-lock` |
| `RegistrationCaseAuthorization` | Done | Per-role `EnsureCan*` methods |
| `RegistrationCaseGuard` | Done | Load + authorize in handlers |
| Audit: `CaseOpened`, `CaseAssigned`, `CaseLockReleased` | Done | Plus existing decision/police events |
| Migration `RoleBoundariesAndCaseLocking` | Done | Lock columns on `RegistrationCases` |

### UI — role boundaries

| Deliverable | Status | Notes |
|-------------|--------|-------|
| Reception: **New case** page only | Done | `/registration/new-case` |
| Population: case list + detail | Done | Auto-claim on detail open |
| Read-only banner when locked to colleague | Done | Shows lock holder name; no misleading “take case” hint |
| **Take case** / **Release lock** actions | Done | Header actions — visibility gated on `CanEdit` / `CanTakeCase` |
| Read-only intake: no edit forms or action buttons | Done | Registration + shared `AppEditableSection` / `CaseDocumentPanel` |
| Police: verifications queue only | Done | Redirected away from case list/detail |
| `RegistrationRoleGate` component | Done | Reusable route guard |
| Demo officer switcher with `?demoOfficer=` | Done | Persisted in URL |
| `DemoOfficerPersistence` port-safe redirects | Done | Relative paths stay relative on role switch |

### UI — reception new case page

| Deliverable | Status | Notes |
|-------------|--------|-------|
| Two-column layout (form + guidance) | Done | `AppCard`, `AppInfoBox`, workflow steps |
| `VisitReasonLabels` helper | Done | Human-readable reason + description |
| Create button enabled with default selection | Done | Validate on submit, not on `IsValid` bind |
| Success state keeps “What happens next?” | Done | Left column switches to confirmation card |
| **Create another case** action | Done | Resets form without navigation |

### UI — review dashboard & case list

| Deliverable | Status | Notes |
|-------------|--------|-------|
| Unassigned cases in **Needs my attention** | Done | Intake, no assignee, no lock |
| **Unassigned** statistic tile | Done | Accent when count > 0 |
| Case list quick filters | Done | All / My cases / Unassigned |
| `?filter=mine` / `?filter=unassigned` | Done | Deep links from dashboard tiles |
| Filter-specific empty states | Done | |
| SQLite-safe `ListAsync` ordering | Done | Order by `OpenedAt` in memory |

### UI — case detail layout

| Deliverable | Status | Notes |
|-------------|--------|-------|
| Case history → collapsible at bottom | Done | After Civil Status; collapsed by default |
| Case history → `AppDataTable` | Done | When / Action / Officer / Details |
| Officer decision aligned with Identity card | Done | Invisible header spacer in sidebar (`app.css`) |

---

## Role matrix

| Action | Reception | Population | Back office | Police |
|--------|-----------|------------|-------------|--------|
| Create case | Yes | Yes | No | No |
| List / view cases | No | Yes | No | No |
| Edit intake & decisions | No | Lock holder only | No | No |
| Claim / release lock | No | Yes | No | No |
| Police queue / record result | No | No | No | Yes |
| Review dashboard | No | Yes | No | No |
| Reports (analytics) | No | Yes | Yes | No |
| Audit timeline on case detail | No | Yes | No | No |

---

## API routes (new)

| Method | Route | Slice |
|--------|-------|-------|
| `POST` | `/api/registration/cases/{id}/claim` | ClaimRegistrationCase |
| `POST` | `/api/registration/cases/{id}/release-lock` | ReleaseCaseLock |

Existing list/get handlers call `RegistrationCaseAuthorization` and `RegistrationCaseGuard`.

---

## UI behaviour

### Reception — new case (`/registration/new-case`)

- Population officers are redirected to the case list (they use the dialog from there if needed).
- Visit reason select with friendly labels; description updates as selection changes.
- **Create case** validates on click; default **First registration** is submittable immediately.
- After create: confirmation card with reference + **Create another case**; right column still shows workflow guidance.

### Population — review dashboard

- Population officers land on **Review dashboard** (`/` redirects; no separate Home nav item).
- **Unassigned** tile counts intake registration cases with no assignee and no lock (links to `/registration/cases?filter=unassigned`).
- **Birth unassigned** and **Ready for confirmation** tiles count birth declaration workload (links to `/birth-declarations?filter=unassigned` and `?filter=ready`).
- **Needs my attention** includes unassigned intakes from **both** registration and birth declaration (summary: “Unassigned — awaiting intake”), plus ready-for-decision/confirmation and suspended cases; table has a **Type** column (`Registration` / `Birth declaration`) and rows navigate to the correct detail page.
- Tile links: **My open cases** → `/registration/cases?filter=mine`, **Unassigned** → `/registration/cases?filter=unassigned`.

### Population — case list (`/registration/cases`)

- Quick filter buttons above the table: **All cases**, **My cases**, **Unassigned**.
- Active filter uses filled primary styling; combines with text search.
- Query parameter `filter` pre-selects: `mine`, `my`, `my-cases`, `unassigned`.

**Filter rules:**

| Filter | Rule |
|--------|------|
| My cases | `LockedByOfficerId` or `AssignedOfficerId` equals current officer |
| Unassigned | Both assignee and lock are null |

### Population — case detail

- Opening detail auto-claims for the current population officer (when unassigned).
- Sidebar: Officer decision panel top-aligned with the Identity **card** (not the section title).
- Case history at the bottom of the main column: collapsible, closed by default, datagrid instead of timeline.

### Read-only case detail (lock held by colleague)

When `IsReadOnlyDueToLock` is true (`CanEdit` false, case locked to another officer), the UI is **review-only** — no action that would mutate the case is shown or clickable.

| UI area | Lock holder (`CanEdit`) | Read-only (`IsReadOnlyDueToLock`) |
|---------|-------------------------|-------------------------------------|
| Header **Take case** | Hidden | Hidden — cannot claim while another officer holds the lock |
| Header **Release lock** | Shown | Hidden |
| Header **Send to police** (registration) | When checklist allows | Hidden |
| Intake forms (identity, residence, address, …) | Shown for incomplete or editable steps | Read-only summary or “Not recorded yet.” — no save/upload/NR-search buttons |
| Officer decision (approve / reject / …) | Shown when status allows | Hidden (`Case.CanEdit` gate) |
| Document upload | Shown | Hidden (`CaseDocumentPanel` respects `CanEdit`) |

**Take case** visibility (shared `CaseLockActions` on birth declaration, change of address, identity document requests; inline on registration case detail):

```text
CanTakeCase = !CanEdit && !IsReadOnlyDueToLock
```

So **Take case** appears only when the case is available to claim (unassigned, or lock released) — not while a colleague holds the lock.

**Release lock** appears only when `CanEdit` is true (current officer holds the lock).

`CaseLockBar` warns that the file is read-only until the lock holder releases; it does not suggest taking the case from a locked colleague.

Incomplete intake steps use `AppEditableSection` and step components (`ResidenceStep`, `AddressStep`, …) to render static placeholders instead of empty edit forms when `CanEdit` is false.

### Demo role switching

- `?demoOfficer={guid}` persists the selected demo officer across navigation.
- Switching role from a page the new role cannot access redirects to the correct home route **without dropping the host port** (fix in `DemoOfficerPersistence.AppendToUri`). Population officers land on **Review dashboard** (`/registration/review-dashboard`); reception on **New case**; police on **Police verifications**.

---

## Key files

| Area | Path |
|------|------|
| Domain locking | `src/.../Domain/Registration/RegistrationCase.cs` |
| Authorization | `src/.../Web/Features/Registration/RegistrationCaseAuthorization.cs` |
| Claim / release | `src/.../Web/Features/Registration/ClaimRegistrationCase/`, `ReleaseCaseLock/` |
| Review dashboard | `src/.../Web/Features/Registration/GetReviewDashboard/` |
| Case list filters | `src/.../Web/Features/Registration/Pages/RegistrationCaseList.razor` |
| New case page | `src/.../Web/Features/Registration/Pages/NewRegistrationCasePage.razor` |
| Visit reason labels | `src/.../Web/Features/Registration/VisitReasonLabels.cs` |
| Case detail layout | `src/.../Web/Features/Registration/Pages/RegistrationCaseDetail.razor` |
| Shared lock chrome | `src/.../Web/Municipal/Components/CaseLockActions.razor`, `CaseLockBar.razor` |
| Shared read-only intake | `src/.../Web/DesignSystem/Components/Layout/AppEditableSection.razor` |
| Document upload gate | `src/.../Web/Municipal/Components/CaseDocumentPanel.razor` |
| Demo officer URL | `src/.../Web/Auth/DemoOfficerPersistence.cs` |
| Layout / role switch | `src/.../Web/Components/Layout/MainLayout.razor` |
| Sidebar alignment CSS | `src/.../Web/wwwroot/app.css` |

---

## Demo script

1. **Reception intake** — Switch to Jean Martin → **New case** → create with default visit reason → see reference and handoff guidance → **Create another case**.
2. **Unassigned visibility** — Switch to Marie Dupont → **Review dashboard** → unassigned registration case appears in tile and **Needs my attention** → click **Unassigned** tile → case list filtered.
3. **Birth declaration handoff** — Reception opens birth declaration → population officer sees it under **Birth unassigned** and in **Needs my attention** (type Birth declaration) → row opens `/birth-declarations/{id}`.
4. **Claim & lock** — Open the case → auto-assigned and locked → timeline shows opened + assigned.
5. **Colleague read-only** — Switch to Anne Leroy → case detail is read-only → no **Take case**, **Release lock**, or intake edit buttons → Marie releases lock → Anne can **Take case** and edit.
6. **Case list filters** — **My cases** shows only yours; **Unassigned** shows reception handoffs; search still works within the active filter.
7. **Case detail layout** — Officer decision aligns with Identity card; expand **Case history** at bottom for datagrid audit log.
8. **Role switch URL** — From `/registration/cases` switch to reception → lands on `/registration/new-case?demoOfficer=…` with port intact.

---

## Tests

| Suite | Coverage |
|-------|----------|
| `RegistrationCaseLockingTests` | Domain claim, release, ensure-editable |
| `RoleBoundariesAndCaseLockingTests` | Reception cannot list/view; claim + audit; lock enforcement; unassigned registration and birth declarations on review dashboard |

---

## Deferred

- Admin force-unlock
- Colleague “request release” notifications
- Additional case list filters (ready for decision, suspended) with dashboard deep links
- Friendly visit reason labels on case list / dashboard tables (currently enum names)
