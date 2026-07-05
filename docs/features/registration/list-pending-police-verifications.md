# List pending police verifications

Returns all open police verification requests for the police clerk queue.

| | |
|---|---|
| **Handler** | `ListPendingPoliceVerificationsHandler` |
| **Route** | `GET /api/registration/police-verifications/pending` |
| **Blazor** | `PoliceVerificationList.razor` (`/registration/police-verifications`) |
| **Response** | `ListPendingPoliceVerificationsResponse` |

## Response shape

```json
{
  "items": [
    {
      "requestId": "a1b2c3d4-...",
      "caseId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "personName": "Sophie Lambert",
      "address": "Chaussée de Louvain 10, 1030 Schaerbeek",
      "requestedAt": "2026-07-05T08:00:00+00:00",
      "attemptNumber": 1
    }
  ],
  "totalCount": 1
}
```

Items are ordered by `requestedAt` ascending (oldest first).

## UI

- **Police clerk** role: nav item **Police verifications** with warning badge when `totalCount > 0`.
- Table columns: Person (links to case), Address, Requested, Attempt, **Record result** button.
- **Record result** opens `RecordPoliceResultDialog`.

### Nav badge and DbContext

`MainLayout` loads the pending count via `IServiceScopeFactory.CreateAsyncScope()` so the badge query does not share the page's scoped `DbContext` (avoids Blazor Server concurrency errors).

## Related

- [Record police result](./record-police-result.md)
- [Request police verification](./request-police-verification.md)
