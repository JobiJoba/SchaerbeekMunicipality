# Record housing situation

Captures how the person occupies the declared address (owner, tenant, lodging, student housing, shelter).

| | |
|---|---|
| **Handler** | `RecordHousingSituationHandler` |
| **Route** | `POST /api/registration/cases/{id}/housing-situation` |
| **Blazor** | `AddressStep.razor` (housing card) |
| **Request** | `RecordHousingSituationRequest(HousingSituation)` |

## Domain rules

- Requires `EnsureIntakeDataEditable()` and `IdentityEstablished`.
- Requires `DeclaredAddress` on the case — address must be saved first.
- Overwrites on re-submit (upsert).

## Correction path

Same handler and route for first record and correction. **Edit** button on the saved housing card in `AddressStep.razor`.

## Values

| `HousingSituation` | UI label |
|--------------------|----------|
| `Owner` | Owner |
| `Tenant` | Tenant |
| `Lodging` | Lodging |
| `StudentHousing` | Student housing |
| `Shelter` | Shelter |
