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
| Read-only banner when locked to colleague | Done | Shows lock holder name |
| **Take case** / **Release lock** actions | Done | Case detail header |
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

| Action | Reception | Population | Police |
|--------|-----------|------------|--------|
| Create case | Yes | Yes | No |
| List / view cases | No | Yes | No |
| Edit intake & decisions | No | Lock holder only | No |
| Claim / release lock | No | Yes | No |
| Police queue / record result | No | No | Yes |
| Review dashboard | No | Yes | No |
| Audit timeline on case detail | No | Yes | No |

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

- **Unassigned** tile counts intake cases with no assignee and no lock.
- **Needs my attention** includes unassigned intakes (summary: “Unassigned — awaiting intake”), ranked below ready-for-decision cases.
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

### Demo role switching

- `?demoOfficer={guid}` persists the selected demo officer across navigation.
- Switching role from a page the new role cannot access redirects to the correct home route **without dropping the host port** (fix in `DemoOfficerPersistence.AppendToUri`).

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
| Demo officer URL | `src/.../Web/Auth/DemoOfficerPersistence.cs` |
| Layout / role switch | `src/.../Web/Components/Layout/MainLayout.razor` |
| Sidebar alignment CSS | `src/.../Web/wwwroot/app.css` |

---

## Demo script

1. **Reception intake** — Switch to Jean Martin → **New case** → create with default visit reason → see reference and handoff guidance → **Create another case**.
2. **Unassigned visibility** — Switch to Marie Dupont → **Review dashboard** → unassigned case appears in tile and **Needs my attention** → click **Unassigned** tile → case list filtered.
3. **Claim & lock** — Open the case → auto-assigned and locked → timeline shows opened + assigned.
4. **Colleague read-only** — Switch to Anne Leroy → case detail is read-only → **Take case** after Marie releases lock.
5. **Case list filters** — **My cases** shows only yours; **Unassigned** shows reception handoffs; search still works within the active filter.
6. **Case detail layout** — Officer decision aligns with Identity card; expand **Case history** at bottom for datagrid audit log.
7. **Role switch URL** — From `/registration/cases` switch to reception → lands on `/registration/new-case?demoOfficer=…` with port intact.

---

## Tests

| Suite | Coverage |
|-------|----------|
| `RegistrationCaseLockingTests` | Domain claim, release, ensure-editable |
| `RoleBoundariesAndCaseLockingTests` | Reception cannot list/view; claim + audit; lock enforcement; unassigned on review dashboard |

---

## Deferred

- Admin force-unlock
- Colleague “request release” notifications
- Additional case list filters (ready for decision, suspended) with dashboard deep links
- Friendly visit reason labels on case list / dashboard tables (currently enum names)
