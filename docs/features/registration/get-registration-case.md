# Get Registration Case

Loads a single registration case with its checklist, linked person (if any), and attached documents.

## Overview

| | |
|---|---|
| **Handler** | `GetRegistrationCaseHandler` |
| **Endpoint** | `GetRegistrationCaseEndpoint` |
| **Route** | `GET /api/registration/cases/{id}` |
| **Blazor page** | `RegistrationCaseDetail.razor` (`/registration/cases/{CaseId}`) |
| **Response** | `RegistrationCaseDetailDto` or `null` |

## Flow diagram

```mermaid
sequenceDiagram
    participant Client as Client<br/>(Blazor or HTTP)
    participant EP as GetRegistrationCaseEndpoint
    participant H as GetRegistrationCaseHandler
    participant CaseRepo as IRegistrationCaseRepository
    participant PersonRepo as IPersonRepository
    participant DocRepo as IAdministrativeDocumentRepository
    participant DB as MunicipalDbContext

    alt Blazor (direct)
        Client->>H: Handle(RegistrationCaseId)
    else HTTP API
        Client->>EP: GET /api/registration/cases/{id}
        EP->>H: Handle(RegistrationCaseId)
    end

    H->>CaseRepo: GetByIdAsync(caseId)
    CaseRepo->>DB: RegistrationCases.FirstOrDefault
    DB-->>H: RegistrationCase | null

    alt case not found
        H-->>Client: null
        opt HTTP
            EP-->>Client: 404 Not Found
        end
    end

    opt PersonId is set
        H->>PersonRepo: GetByIdAsync(personId)
        PersonRepo->>DB: Persons.AsNoTracking()
        DB-->>H: Person
    end

    H->>DocRepo: ListByCaseIdAsync(caseId)
    DocRepo->>DB: AdministrativeDocuments<br/>.Where(caseId)<br/>.OrderByDescending(UploadedAt)
    DB-->>H: List<AdministrativeDocument>

    H->>H: Map → RegistrationCaseDetailDto<br/>(case + checklist + person + documents)
    H-->>Client: RegistrationCaseDetailDto

    opt HTTP
        EP-->>Client: 200 OK (JSON)
    end
```

## Call chain

```
RegistrationCaseDetail.razor
  └─ OnParametersSetAsync → ReloadCase()
       └─ GetRegistrationCaseHandler.Handle(RegistrationCaseId)
            ├─ IRegistrationCaseRepository.GetByIdAsync()
            ├─ IPersonRepository.GetByIdAsync()        [if PersonId set]
            ├─ IAdministrativeDocumentRepository.ListByCaseIdAsync()
            └─ Map() → RegistrationCaseDetailDto
```

The same handler is called again after identity recording or document upload to refresh the page.

## Response shape

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "status": "Intake",
  "visitReason": "FirstRegistration",
  "assignedOfficerId": "11111111-1111-1111-1111-111111111111",
  "openedAt": "2026-07-04T10:30:00+00:00",
  "checklist": {
    "identityEstablished": true,
    "legalResidenceEstablished": false,
    "addressDeclared": false,
    "addressConfirmed": false,
    "registerDeterminable": false
  },
  "person": {
    "id": "...",
    "givenName": "Marie",
    "familyName": "Leclerc",
    "birthDate": "1975-01-01",
    "nationality": "Belgian",
    "bisNumber": "75010112345",
    "nationalRegisterNumber": null,
    "linkedFromRegister": true
  },
  "possibleDuplicateMatches": [],
  "activePoliceVerification": null,
  "policeVerificationHistory": [
    {
      "requestId": "a1b2c3d4-...",
      "attemptNumber": 1,
      "requestedAt": "2026-07-05T08:00:00+00:00",
      "completedAt": "2026-07-05T14:30:00+00:00",
      "result": "Confirmed",
      "officerNotes": "Person present at declared address.",
      "isPending": false
    }
  ],
  "documents": [
    {
      "id": "...",
      "documentType": "Passport",
      "fileName": "passport.pdf",
      "uploadedAt": "2026-07-04T11:00:00+00:00"
    }
  ]
}
```

When identity has not been recorded, `person` is `null`.

### Possible duplicate matches (Phase 5)

When a person exists **without** `linkedFromRegister`, the handler runs a National Register search using the person's identity and returns high-confidence matches in `possibleDuplicateMatches` (score ≥ 80). The case detail page shows a warning banner when this list is non-empty.

Each match includes `registerPersonId`, name, birth date, BIS/NR numbers, `matchScore`, and `matchReason`. See [search-national-register.md](./search-national-register.md).

### Police verification (Phase 6)

| Field | When set | Description |
|-------|----------|-------------|
| `activePoliceVerification` | Case has a pending police request | Current open visit (`isPending: true`) |
| `policeVerificationHistory` | One or more completed visits | Outcome, dates, and `officerNotes` from police clerk |

History excludes pending requests and is ordered by `attemptNumber` descending. Rendered on case detail by `PoliceVerificationSection` — one visit as a card, multiple visits as `AppDataTable`.

See [record-police-result.md](./record-police-result.md) and [phase-6-police-verification-loop.md](../../phases/phase-6-police-verification-loop.md).

## Blazor UI behaviour

`RegistrationCaseDetail.razor` uses the DTO to:

- Show case header (status chip, visit reason, opened date)
- Render checklist status chips (including **Address confirmed**)
- Collapse completed intake sections; auto-expand the next actionable step (Phase 6.1)
- Show **Police verification** section when a visit was sent or recorded
- Show **Send to police** when identity + address declared and no pending request
- Show **duplicate warning** when `possibleDuplicateMatches` is non-empty and person not linked
- Show identity form **or** read-only person card with BIS/NR and convert button
- Open `NationalRegisterSearchDialog` from search button or warning
- Embed intake steps and `RegistrationCaseDocumentPanel`

## Error responses (HTTP)

| Status | Condition |
|--------|-----------|
| `404` | Case ID not found |
| `200` | Success |

## Dependencies

| Dependency | Role |
|------------|------|
| `IRegistrationCaseRepository` | Load case aggregate |
| `IPersonRepository` | Load linked person |
| `IAdministrativeDocumentRepository` | Load case documents |
| `INationalRegisterRepository` | Duplicate detection search (Phase 5) |
| `IPoliceVerificationRepository` | Active request + completed visit history (Phase 6) |

This is a read-only slice with no domain mutations.
