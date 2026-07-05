# Request police verification

Sends a registration case to the local police for a residence check. Creates a `PoliceVerificationRequest` and moves the case to `AwaitingPoliceVerification`.

| | |
|---|---|
| **Handler** | `RequestPoliceVerificationHandler` |
| **Route** | `POST /api/registration/cases/{id}/police-verification` |
| **Blazor** | `RegistrationCaseDetail.razor` — **Send to police** header button |
| **Response** | `RequestPoliceVerificationResponse` |

## Domain rules

- Case must be in `Intake` or `UnderReview`.
- `Checklist.IdentityEstablished` and `Checklist.AddressDeclared` must be true.
- `DeclaredAddress` must be set.
- No other pending verification request for the same case.
- `AttemptNumber` = max existing attempt + 1 (first request = 1).

## Response

```json
{
  "caseId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "requestId": "a1b2c3d4-...",
  "attemptNumber": 1,
  "status": "AwaitingPoliceVerification"
}
```

## Error responses

| Status | Condition |
|--------|-----------|
| `404` | Case not found |
| `409` | Invalid transition (missing identity/address, wrong status) |
| `409` | Pending verification already exists (`InvalidPoliceVerificationException`) |

## UI

- Button shown when identity + address declared, no active pending request, and status is `Intake` or `UnderReview`.
- Confirmed via `AppConfirmDialog` before sending.
- After send, intake sections are locked until police records a result.

## Related

- [Record police result](./record-police-result.md)
- [List pending police verifications](./list-pending-police-verifications.md)
- [Get registration case](./get-registration-case.md) — `activePoliceVerification`
