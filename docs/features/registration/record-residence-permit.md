# Record residence permit

Records permit type, validity, and card reference for cases that require permit evidence.

| | |
|---|---|
| **Handler** | `RecordResidencePermitHandler` |
| **Route** | `POST /api/registration/cases/{id}/residence-permit` |
| **Blazor** | `ResidenceStep.razor` |
| **Request** | `RecordResidencePermitRequest(PermitType, ValidFrom, ValidUntil, CardNumber?, IssuingAuthority?)` |

## Domain rules

- Requires identity recorded and residence category set.
- One permit per case (upsert on re-submit).
- Re-evaluates residence policy with the new permit; sets `LegalResidenceEstablished` when valid.

## Permit types

`ACard`, `BCard`, `CCard`, `Annex15`, `EuRegistrationCertificate`
