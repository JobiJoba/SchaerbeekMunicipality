# Phase 17 — Post-registration amendments

- **Status:** Planned
- **Goal:** Formal workflow to **correct register data after registration** — name changes, civil status updates, nationality corrections — with audit trail and approval gate.
- **Maps to IDEA:** Ongoing maintenance of population register entries (simplified).

---

## Summary

Phase 2.1 covers **intake corrections** during `Intake` and `UnderReview`. Once a case is `Registered`, changes require a separate **amendment case** that references the registered person, documents the reason, and applies approved updates to the golden `Person` record.

---

## Bounded context

| Context | Role |
|---------|------|
| **Amendments** (new) | `RegisterAmendmentCase` aggregate |
| **Person** (existing) | Target of approved changes |

---

## Domain

### Aggregate: `RegisterAmendmentCase`

| Field / concept | Description |
|-----------------|-------------|
| `RegisterAmendmentCaseId` | Strongly typed identifier |
| `PersonId` | Subject |
| `AmendmentType` | `IdentityCorrection`, `CivilStatusUpdate`, `NationalityChange`, … |
| `Status` | `Draft` → `UnderReview` → `Approved` → `Applied` / `Rejected` |
| `ProposedChanges` | JSON or typed payload (e.g. new family name) |
| `SupportingDocuments` | Court judgment, marriage certificate, etc. |
| `AppliedAt` | When `Person` was updated |

### Rules

- Only population officers approve amendments.
- Applied amendments append to person audit history (extend `CaseAuditEntry` pattern or `PersonAmendmentLog`).
- Cannot amend persons with open amendment cases.

---

## Slices

| Slice | Notes |
|-------|-------|
| `OpenRegisterAmendmentCase` | From person file or case history |
| `ProposeAmendment` | Capture typed change + evidence |
| `ApproveAmendment` / `RejectAmendment` | Review gate |
| `ApplyAmendment` | Mutate `Person`; terminal |
| `ListRegisterAmendmentCases` | Back-office queue |

---

## UI

- Open from Phase 16 person file: “Request amendment”.
- Amendment wizard: type → proposed values → documents → submit for review.
- Review queue on population officer dashboard.

---

## Demo

Registered person legally changes family name → officer opens amendment → attaches judgment → approves → person file shows new name with amendment audit entry.

---

## Tests

- Cannot apply without approval.
- Concurrent amendment cases blocked.
- Applied amendment visible in person file history.

---

## Out of scope

- Judicial integration for automatic updates.
- FR / NL localization.

---

## Dependencies

- **Phase 16** person file recommended as entry point.
- Registered persons from Phase 7+.

---

## Related documents

- [phase-2.1-intake-corrections.md](./phase-2.1-intake-corrections.md) — intake-time corrections (different lifecycle)
