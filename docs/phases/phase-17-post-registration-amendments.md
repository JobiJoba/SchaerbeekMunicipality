# Phase 17 — Post-registration amendments

- **Status:** Complete
- **Completed:** July 2026
- **Goal:** Formal workflow to **correct register data after registration** — name changes, civil status updates, nationality corrections — with audit trail and approval gate.
- **Maps to IDEA:** Ongoing maintenance of population register entries (simplified).

---

## Summary

Delivered a new **`RegisterAmendmentCase`** bounded context separate from intake corrections. Officers open amendment cases from the person file or amendment queue, propose typed changes with supporting documents, submit for review, approve, and apply updates to the golden `Person` record.

---

## What was built

### Domain

- Aggregate `RegisterAmendmentCase` with statuses `Draft` → `UnderReview` → `Approved` → `Applied` / `Rejected`
- Amendment types: `IdentityCorrection`, `CivilStatusUpdate`, `NationalityChange`
- Concurrent open cases blocked per person via `HasOpenCaseForPersonAsync`
- `AdministrativeDocument` extended with `RegisterAmendmentCaseId` FK

### Application slices

| Slice | Route |
|-------|-------|
| `OpenRegisterAmendmentCase` | `POST /api/register-amendments/cases` |
| `ListRegisterAmendmentCases` | `GET /api/register-amendments/cases` |
| `GetRegisterAmendmentCase` | `GET /api/register-amendments/cases/{id}` |
| `RecordProposedAmendment` | `PUT /api/register-amendments/cases/{id}/proposed-changes` |
| `AttachRegisterAmendmentDocument` / `RemoveRegisterAmendmentDocument` | `POST` / `DELETE …/documents` |
| `SubmitRegisterAmendmentForReview` | `POST …/submit` |
| `ApproveRegisterAmendment` / `RejectRegisterAmendment` | `POST …/approve` / `…/reject` |
| `ApplyRegisterAmendment` | `POST …/apply` |

Authorization: **PopulationOfficer** for all operations; approve/reject/apply require `CanApproveRegistration`.

### Person file integration

- `PersonFileQuery` lists amendment cases on the Cases tab and projects amendment lifecycle events into history
- **Request amendment** button on person file (registered persons with NR number)

### UI

| Page | Route |
|------|-------|
| Amendment queue | `/register-amendments/cases` |
| Amendment detail | `/register-amendments/cases/{id}` |

- Nav item **Register amendments** for Population officers
- Review dashboard tile **Amendments pending review**

---

## Demo

1. Run AppHost as Marie Dubois (Population officer).
2. **Person lookup** → open a registered person → **Request amendment** → Identity correction → new family name.
3. Attach supporting document → **Submit for review**.
4. **Approve** → **Apply to register**.
5. Return to person file → updated name in header + **Amendment applied** in history.

---

## Tests

- `RegisterAmendmentCaseTests` (domain): status guards, apply mutates person per type
- `RegisterAmendmentTests` (integration): happy path, cannot apply without approval, concurrent case block, unauthorized approve

---

## Out of scope (unchanged)

- Judicial integration for automatic updates
- FR / NL localization

---

## Related documents

- [phase-2.1-intake-corrections.md](./phase-2.1-intake-corrections.md) — intake-time corrections (different lifecycle)
- [phase-16-person-file.md](./phase-16-person-file.md) — entry point for amendments
