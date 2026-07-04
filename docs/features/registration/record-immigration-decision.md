# Record immigration decision (stub)

Stores a reference to an external federal immigration office decision. Optional for most demo categories.

| | |
|---|---|
| **Handler** | `RecordImmigrationDecisionHandler` |
| **Route** | `POST /api/registration/cases/{id}/immigration-decision` |
| **Blazor** | `ResidenceStep.razor` |
| **Request** | `RecordImmigrationDecisionRequest(ReferenceNumber, DecisionDate)` |

## Domain rules

- Requires identity recorded.
- Persists owned `ImmigrationDecisionReference` on the case.
- Re-evaluates residence policy (stub policies do not require a decision today).
