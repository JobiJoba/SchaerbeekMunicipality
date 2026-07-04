# Record immigration decision (stub)

Stores a reference to an external federal immigration office decision. Optional for most demo categories.

| | |
|---|---|
| **Handler** | `RecordImmigrationDecisionHandler` |
| **Route** | `POST /api/registration/cases/{id}/immigration-decision` |
| **Blazor** | `ResidenceStep.razor` |
| **Request** | `RecordImmigrationDecisionRequest(ReferenceNumber, DecisionDate)` |

## Domain rules

- Requires `EnsureIntakeDataEditable()` and identity recorded.
- Persists owned `ImmigrationDecisionReference` on the case (overwrites on re-submit).
- Re-evaluates residence policy (stub policies do not require a decision today).

## Correction path

Uses the **upsert handler** pattern (see [intake corrections](./README.md#intake-corrections-phase-21)): the same `POST` route and handler serve first record and correction. `ResidenceStep.razor` exposes an **Edit** button on the saved decision card.
