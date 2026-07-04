# Record household composition

Lists who lives at the declared address with roles (head, spouse, child, other).

| | |
|---|---|
| **Handler** | `RecordHouseholdCompositionHandler` |
| **Route** | `POST /api/registration/cases/{id}/household` |
| **Blazor** | `HouseholdStep.razor` |
| **Request** | `RecordHouseholdCompositionRequest(Members[])` |

## Domain rules

- Requires `EnsureIntakeDataEditable()` and `IdentityEstablished`.
- Requires `DeclaredAddress` on the case.
- `Household` aggregate (1:1 with case): `SetComposition` replaces all members on each save.

## Correction path

Upsert: each save replaces the full member list. UI lets officers add/remove members locally, then **Save household** when the list differs from persisted state.

## Member fields

| Field | Required |
|-------|----------|
| Given name | Yes |
| Family name | Yes |
| Birth date | Yes (not in future) |
| Role | Yes (`Head`, `Spouse`, `Child`, `Other`) |
