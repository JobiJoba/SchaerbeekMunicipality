# Phase 2.1 — Intake corrections

- **Status:** Complete
- **Goal:** Allow officers to correct any intake step after it has been saved — identity, legal residence, and documents — without reopening the case or losing later progress.

---

## Summary

Phases 1–2 deliver **first-time recording** only. Once identity or legal residence is saved, the case detail UI switches to read-only summaries with no way to fix mistakes (e.g. wrong name after starting residence, wrong permit type after category change).

[ADR-0004](../adr/0004-checklist-over-linear-state-machine.md) and [ARCHITECTURE.md](../ARCHITECTURE.md) already assume flexible, non-linear intake and state that **corrections are allowed during `Intake` (and later `UnderReview`)**. Phase 2.1 closes the gap between that intent and the current implementation.

This is a **cross-cutting** phase: one shared correction policy applied consistently across every intake slice built so far, and a template that Phase 4+ slices must follow from day one.

---

## Problem statement

| Step | Backend today | UI today | Officer impact |
|------|---------------|----------|----------------|
| **Identity** | `RecordIdentity` throws if `PersonId` already set; `Person` has no `Update` | Read-only card after save | Cannot fix name, DOB, or nationality |
| **Residence category** | `SetResidenceCategory` overwrites category | Form hidden once set | Cannot change EU → non-EU (or vice versa) |
| **Residence permit** | Handler upserts via `ResidencePermit.Update()` | Read-only card after save | Backend works; UI blocks correction |
| **Immigration decision** | `RecordImmigrationDecision` overwrites reference | Read-only card after save | Backend works; UI blocks correction |
| **Documents** | Attach only | List only; no remove/replace | Wrong scan cannot be replaced |

Officers currently must work around locked data or abandon the case. That contradicts real municipal intake, where facts are clarified as the conversation progresses.

---

## Design decisions

### 1. Record-or-correct convention

Each intake slice supports **both** first record and correction:

| Approach | When to use | Example |
|----------|-------------|---------|
| **Upsert handler** | Same payload shape for create and update | `RecordResidencePermit` (already upserts) |
| **Explicit `Correct*` domain method** | First record and correction have different invariants | `RecordIdentity` vs `CorrectIdentity` |

**Rule:** Pick one pattern per slice and document it. Prefer **explicit `Correct*`** when the first-record guard (`PersonId is null`) should stay strict; prefer **upsert** when create and update share the same validation.

### 2. When corrections are allowed

| Case status | Corrections |
|-------------|-------------|
| `Intake` | Allowed (primary use case) |
| `UnderReview` | Allowed (per ARCHITECTURE.md) |
| `AwaitingPoliceVerification` | Deferred — revisit in Phase 6 (police loop may need invalidation rules) |
| `Approved`, `Registered`, `Rejected` | Blocked — terminal or post-decision; future amendment workflow out of scope |

Domain guard: shared helper e.g. `EnsureIntakeDataEditable()` on `RegistrationCase`.

### 3. Checklist re-evaluation after every correction

Data corrections must **never** leave stale checklist flags. After any correction handler saves:

1. Re-run relevant evaluators (`RegistrationResidenceEvaluator` today; address/household evaluators in later phases).
2. Update checklist flags on the aggregate (`ApplyResidencePolicyResult`, etc.).
3. Return policy/checklist state in the response so the UI can show warnings.

### 4. UI: edit form on every section card

Follow [design-system edit form](../design-system/06-page-templates.md#4-edit-form):

- Saved state → summary + **Edit** button (icon + label, accessible).
- Edit mode → pre-populated form, `AppSaveCancelBar` enabled only when dirty.
- Cancel → discard local edits, return to summary (optional `AppConfirmDialog` if dirty).
- Save → correction/upsert handler → `ReloadCase()` → refresh checklist chips.

No linear wizard lock-in: all sections remain visible on the case detail page (consistent with ADR-0004).

### 5. Cascade rules

Corrections can invalidate related data. Phase 2.1 must define and implement:

| Correction | Side effects |
|------------|--------------|
| **Identity** (nationality change) | Re-run residence policy; may clear `LegalResidenceEstablished` |
| **Residence category** change | Re-run policy; if new category does not require a permit, permit may become irrelevant (keep row but ignore in policy, or warn officer to review permit) |
| **Residence category** change with incompatible permit type | Policy fails until permit is corrected; show warning in UI |
| **Permit** correction | Re-run policy only |
| **Immigration decision** correction | Re-run policy only |
| **Document** remove/replace | Re-run document-dependent policies when applicable |

Phase 2.1 **does not** implement police or address invalidation (Phases 4 and 6).

### 6. Audit trail (minimal)

Full `CaseAuditEntry` collection is Phase 7 scope. Phase 2.1 **should** log corrections at the application layer (structured log: case id, slice, officer id, timestamp) so integration tests can assert correction occurred. Persistent audit table is optional stretch.

---

## Deliverables checklist

| # | Deliverable | Status | Notes |
|---|-------------|--------|-------|
| 2.1.1 | Domain: `Person.Update(IdentityDetails)` | Done | Trim/validate like `Create` |
| 2.1.2 | Domain: `RegistrationCase.CorrectIdentity(Person, …)` | Done | Requires `PersonId`; stays `Intake`/`UnderReview` |
| 2.1.3 | Slice: `CorrectIdentity` | Done | Handler, validator, endpoint, tests |
| 2.1.4 | UI: Identity edit on case detail | Done | Edit button + pre-filled form |
| 2.1.5 | Domain: `ImmigrationDecisionReference.Update(…)` or overwrite via case method | Done | Overwrite via `RecordImmigrationDecision` |
| 2.1.6 | Confirm/document upsert: residence category | Done | Already overwrites; tests + UI edit |
| 2.1.7 | Confirm/document upsert: residence permit | Done | Backend done; UI edit |
| 2.1.8 | Confirm/document upsert: immigration decision | Done | Upsert via handler + UI edit |
| 2.1.9 | Slice: `RemoveDocument` (or `ReplaceDocument`) | Done | Hard-delete + storage cleanup |
| 2.1.10 | UI: Document remove/replace | Done | Per-row remove with confirm dialog |
| 2.1.11 | Shared: `EnsureIntakeDataEditable()` guard | Done | Used by all correction paths |
| 2.1.12 | Shared: post-correction re-evaluation | Done | Identity + document removal trigger residence re-eval |
| 2.1.13 | Feature docs for correction slices | Done | Under `docs/features/registration/` |
| 2.1.14 | Domain + integration tests | Done | Per slice + cascade scenarios |

---

## Slices to implement

### `CorrectIdentity`

| | |
|---|---|
| **Route** | `PUT /api/registration/cases/{id}/identity` (or `PATCH`) |
| **Blazor** | `RegistrationCaseDetail.razor` — identity section edit mode |
| **Request** | Same fields as `RecordIdentityRequest` |

**Domain:**

```csharp
public void CorrectIdentity(Person person, IdentityDetails identity)
{
    EnsureIntakeDataEditable(nameof(CorrectIdentity));
    if (PersonId is null || person.Id != PersonId)
        throw …;
    person.Update(identity);
    // IdentityEstablished remains true
}
```

**Handler:** Load case + person by `PersonId`, call domain, re-run `RegistrationResidenceEvaluator`, save.

**Tests:**

- Correct name on case with residence category set → person updated, checklist recomputed.
- Correct nationality EU ↔ non-EU → `LegalResidenceEstablished` may flip.
- Block correction when status is `Approved`.

---

### Legal residence — category, permit, decision

These steps mostly need **UI edit mode** and **tests documenting upsert behaviour**. Backend gaps:

| Slice | Backend work | UI work |
|-------|--------------|---------|
| `SetResidenceCategory` | Add cascade test when category changes with existing permit; optional warning in response | Edit button → category select pre-filled |
| `RecordResidencePermit` | None (upsert exists) | Edit button → permit form pre-filled |
| `RecordImmigrationDecision` | Add `Update` on reference or explicit correct method | Edit button → decision form pre-filled |

**Category change UX:** After save, if policy fails because permit type mismatches new category, show persistent warning until permit is corrected.

---

### `RemoveDocument` (minimum viable document correction)

| | |
|---|---|
| **Route** | `DELETE /api/registration/cases/{id}/documents/{documentId}` |
| **Blazor** | Remove action on each row in attached documents list |

**Domain:** `RegistrationCase.EnsureCanAttachDocuments()` already gates intake; extend with remove permission under same statuses.

**Infrastructure:** Delete DB row + remove file from `IDocumentStorage` (or orphan policy documented).

**Optional stretch:** `ReplaceDocument` as delete + attach in one transaction.

---

## API routes (after Phase 2.1)

| Method | Route | Slice | New? |
|--------|-------|-------|------|
| `POST` | `/api/registration/cases/{id}/identity` | Record identity (first time) | Existing |
| `PUT` | `/api/registration/cases/{id}/identity` | Correct identity | **New** |
| `POST` | `/api/registration/cases/{id}/residence-category` | Set / correct category | Existing (behaviour documented) |
| `POST` | `/api/registration/cases/{id}/residence-permit` | Record / correct permit | Existing (behaviour documented) |
| `POST` | `/api/registration/cases/{id}/immigration-decision` | Record / correct decision | Existing (behaviour documented) |
| `POST` | `/api/registration/cases/{id}/documents` | Attach document | Existing |
| `DELETE` | `/api/registration/cases/{id}/documents/{documentId}` | Remove document | **New** |

---

## UI changes

### `RegistrationCaseDetail.razor`

- Identity: toggle `_editingIdentity`; pre-fill from `_case.Person` on Edit.
- Keep create form for `_case.Person is null`; use edit form when correcting.

### `ResidenceStep.razor`

- Add `_editingCategory`, `_editingPermit`, `_editingDecision` flags.
- Each saved card gets an Edit action in the card header or action bar.
- Show policy warning banner when category/permit combination is invalid.

### Document list

- Add remove (with confirm dialog) per `AppConfirmDialog` pattern.

---

## Demo walkthrough

1. Open case, record identity as **"Jon" Vermeulen**, nationality **Belgian**.
2. Set residence category **EU citizen** → legal residence ✓.
3. Click **Edit** on identity → change given name to **"Jean"** → save → summary updates; checklist unchanged.
4. Open another case: record identity, set **Non-EU worker**, record **B card**.
5. Click **Edit** on residence category → change to **Student** → save → warning: permit type may not match → edit permit to **Annex 15** → legal residence ✓.
6. Attach wrong passport scan → **Remove** document → attach correct file.

---

## Tests

| Project | Scenarios |
|---------|-----------|
| `Domain.Tests` | `Person.Update`; `CorrectIdentity` guards; category change + policy cascade |
| `Integration.Tests` | PUT identity; category correction with stale permit; permit UI path via API; DELETE document; blocked when wrong status |

```bash
dotnet test --configuration Release --filter "Category!=PostgreSQL"
```

Target: all existing tests stay green; add ~10–15 new tests for correction paths.

---

## Out of scope

- Persistent `CaseAuditEntry` table (Phase 7).
- Corrections during `AwaitingPoliceVerification` and police-result invalidation (Phase 6).
- Address / household / civil status correction (Phase 4 — **must ship as upsert from day one**).
- National Register link/unlink after identity correction (Phase 5).
- Formal amendment workflow post-registration (Phase 17).

---

## Carries forward to later phases

Every new intake slice from Phase 4 onward **must**:

1. Support correction in the same phase as first record (not deferred).
2. Use the shared `EnsureIntakeDataEditable()` guard.
3. Re-run checklist evaluators after correction.
4. Ship UI with Edit on the section card (or wizard step edit per design system).
5. Include at least one integration test: record → correct → assert persisted state.

Add a checklist item to the [slice template](../ROADMAP.md#slice-checklist-template) in ROADMAP when Phase 2.1 is merged:

```
[ ] Correction path (domain + handler + UI edit) if slice records intake data
```

---

## Related documents

- [ROADMAP.md](../ROADMAP.md) — Phase 2.1 entry
- [phase-2-residence-category-permits.md](./phase-2-residence-category-permits.md) — prerequisite
- [ADR-0004](../adr/0004-checklist-over-linear-state-machine.md) — flexible intake rationale
- [ARCHITECTURE.md](../ARCHITECTURE.md) — corrections during Intake / UnderReview
- [design-system/06-page-templates.md](../design-system/06-page-templates.md) — edit form pattern
- [features/registration/README.md](../features/registration/README.md) — slice index (updated when slices land)
