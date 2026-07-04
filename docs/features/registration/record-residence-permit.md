# Record residence permit

Records permit type, validity, and card reference for cases that require permit evidence.

| | |
|---|---|
| **Handler** | `RecordResidencePermitHandler` |
| **Route** | `POST /api/registration/cases/{id}/residence-permit` |
| **Blazor** | `ResidenceStep.razor` |
| **Request** | `RecordResidencePermitRequest(PermitType, ValidFrom, ValidUntil, CardNumber?, IssuingAuthority?)` |

## Domain rules

- Requires `EnsureIntakeDataEditable()`, identity recorded, and residence category set.
- One permit per case (upsert on re-submit via `ResidencePermit.Update()`).
- Re-evaluates residence policy with the new permit; sets `LegalResidenceEstablished` when valid.

## Correction path

Uses the **upsert handler** pattern (see [intake corrections](./README.md#intake-corrections-phase-21)): the same `POST` route and handler serve first record and correction. `ResidenceStep.razor` exposes an **Edit** button on the saved permit card with a pre-filled form.

## Permit types

`ACard`, `BCard`, `CCard`, `Annex15`, `EuRegistrationCertificate`
