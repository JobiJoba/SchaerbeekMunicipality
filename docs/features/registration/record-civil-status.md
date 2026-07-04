# Record civil status

Captures marital status on the `Person` linked to the case, with marriage details when required.

| | |
|---|---|
| **Handler** | `RecordCivilStatusHandler` |
| **Route** | `POST /api/registration/cases/{id}/civil-status` |
| **Blazor** | `CivilStatusStep.razor` |
| **Request** | `RecordCivilStatusRequest(Status, SpouseGivenName?, SpouseFamilyName?, MarriageDate?, MarriagePlace?)` |

## Domain rules

- Requires `EnsureIntakeDataEditable()` and `IdentityEstablished`.
- Updates `Person.CivilStatus` via `CivilStatusRecord.Create`.
- **Married** and **Registered partnership** require spouse names and marriage date.

## Correction path

Upsert on the same `POST` route. **Edit** button on the saved civil status card.

## Values

| `CivilStatus` | Marriage fields |
|---------------|-----------------|
| `Single`, `Divorced`, `Widowed`, `Separated` | Not required |
| `Married`, `RegisteredPartnership` | Spouse name + marriage date required; place optional |
