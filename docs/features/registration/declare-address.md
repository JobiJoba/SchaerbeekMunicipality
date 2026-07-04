# Declare address

Records the citizen's domicile on the registration case. For first registration at the Schaerbeek desk, the address **must** be in `1030 Schaerbeek`.

| | |
|---|---|
| **Handler** | `DeclareAddressHandler` |
| **Route** | `POST /api/registration/cases/{id}/address` |
| **Blazor** | `AddressStep.razor` |
| **Request** | `DeclareAddressRequest(Street, HouseNumber, Box?, PostalCode, Municipality)` |

## Domain rules

- Requires `EnsureIntakeDataEditable()` (`Intake` or `UnderReview`) and `IdentityEstablished`.
- Municipality must be **1030 Schaerbeek** (`SchaerbeekCommune` constant) — enforced in domain and validator.
- Creates a `BelgianAddress` value object (4-digit postal code validation).
- Sets checklist flag `AddressDeclared`.

## Correction path

Uses the **upsert handler** pattern: the same `POST` route overwrites the declared address. `AddressStep.razor` shows an **Edit** button on the saved address card.

## UI

- Municipality field is **read-only** (`1030 Schaerbeek`).
- Street autocomplete queries `GET /api/registration/streets?postalCode=1030`.
- Info box explains the Schaerbeek-only rule.

## Related

- [Record housing situation](./record-housing-situation.md) — requires address declared first
- [Record household composition](./record-household-composition.md)
