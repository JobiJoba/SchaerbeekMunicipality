# Record police result

Records the outcome of a police residence visit. Updates both the `PoliceVerificationRequest` and the parent `RegistrationCase`.

| | |
|---|---|
| **Handler** | `RecordPoliceResultHandler` |
| **Route** | `POST /api/registration/police-verifications/{requestId}/result` |
| **Validator** | `RecordPoliceResultValidator` |
| **Blazor** | `RecordPoliceResultDialog.razor` (from police clerk portal) |
| **Request** | `RecordPoliceResultRequest(Result, OfficerNotes?)` |

## Request body

```json
{
  "result": "Confirmed",
  "officerNotes": "Person present at declared address."
}
```

| Field | Rules |
|-------|-------|
| `result` | Required; one of `PoliceVerificationResult` enum values |
| `officerNotes` | Optional; max 2000 characters |

### Outcome values

| Value | Meaning |
|-------|---------|
| `Confirmed` | Person found at address |
| `NotFound` | Person not present |
| `AddressIncorrect` | Wrong address |
| `MailboxOnly` | Only mailbox, not residing |
| `EmptyDwelling` | No one living at address |
| `RefusedAccess` | Could not enter / verify |
| `Incomplete` | Needs second visit |

## Domain rules

- Request must exist and be pending (`CompletedAt` is null).
- Case must be in `AwaitingPoliceVerification`.
- `Confirmed` → `Checklist.AddressConfirmed = true`; all other results clear it.
- Case status → `UnderReview`.

## Response

```json
{
  "requestId": "a1b2c3d4-...",
  "caseId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "result": "Confirmed",
  "addressConfirmed": true,
  "status": "UnderReview"
}
```

## Error responses

| Status | Condition |
|--------|-----------|
| `400` | Validation failure |
| `404` | Request or case not found |
| `409` | Request already completed or invalid case status |

## UI

Police clerk selects outcome from dropdown and optionally enters notes. After save, the case detail **Police verification** section shows the recorded visit (card or table depending on visit count).

## Related

- [Request police verification](./request-police-verification.md)
- [Get registration case](./get-registration-case.md) — `policeVerificationHistory`
