# Open Registration Case

Creates a new registration case in `Intake` status and assigns it to an officer.

## Overview

| | |
|---|---|
| **Handler** | `OpenRegistrationCaseHandler` |
| **Endpoint** | `OpenRegistrationCaseEndpoint` |
| **Validator** | `OpenRegistrationCaseValidator` |
| **Route** | `POST /api/registration/cases` |
| **Blazor entry** | `NewCaseDialog.razor` (opened from `RegistrationCaseList.razor`) |
| **Request** | `OpenRegistrationCaseRequest(VisitReason, Guid? AssignedOfficerId)` |
| **Response** | `OpenRegistrationCaseResponse(CaseId, Status, VisitReason, OpenedAt)` |

## Flow diagram

```mermaid
sequenceDiagram
    participant Client as Client<br/>(Blazor or HTTP)
    participant EP as OpenRegistrationCaseEndpoint
    participant V as OpenRegistrationCaseValidator
    participant H as OpenRegistrationCaseHandler
    participant Auth as ICurrentOfficer
    participant TP as TimeProvider
    participant Domain as RegistrationCase
    participant Repo as IRegistrationCaseRepository
    participant DB as MunicipalDbContext

    alt Blazor (direct)
        Client->>H: Handle(request)
        H->>V: ValidateAndThrowAsync(request)
    else HTTP API
        Client->>EP: POST /api/registration/cases
        EP->>V: ValidateAsync(request)
        alt invalid
            V-->>EP: errors
            EP-->>Client: 400 ValidationProblem
        end
        EP->>H: Handle(request)
    end

    H->>Auth: OfficerId (if no AssignedOfficerId in request)
    H->>TP: GetUtcNow()
    H->>Domain: RegistrationCase.Open(visitReason, officerId, openedAt)
    Note over Domain: Status = Intake<br/>Checklist = empty<br/>New RegistrationCaseId

    H->>Repo: AddAsync(registrationCase)
    H->>Repo: SaveChangesAsync()
    Repo->>DB: INSERT registration_cases

    H-->>Client: OpenRegistrationCaseResponse

    opt HTTP only
        EP-->>Client: 201 Created<br/>Location: /api/registration/cases/{id}
    end

    opt Blazor dialog
        Client-->>Client: Navigate to /registration/cases/{caseId}
    end
```

## Call chain

```
RegistrationCaseList.razor
  └─ OpenNewCaseDialog()
       └─ NewCaseDialog.razor → Submit()
            └─ OpenRegistrationCaseHandler.Handle(request)
                 ├─ OpenRegistrationCaseValidator.ValidateAndThrowAsync()
                 ├─ ICurrentOfficer.OfficerId (default assignee)
                 ├─ TimeProvider.GetUtcNow()
                 ├─ RegistrationCase.Open(...)          [Domain]
                 ├─ IRegistrationCaseRepository.AddAsync()
                 └─ IRegistrationCaseRepository.SaveChangesAsync()
```

## Domain logic

`RegistrationCase.Open()` factory method:

- Generates a new `RegistrationCaseId`
- Sets status to `Intake`
- Initializes an empty checklist
- Records visit reason, assigned officer, and timestamp

No domain events are raised in the current implementation.

## Validation rules

| Field | Rule |
|-------|------|
| `VisitReason` | Must be a valid enum value |

## Request example

```json
{
  "visitReason": "FirstRegistration",
  "assignedOfficerId": null
}
```

When `assignedOfficerId` is null, the current officer from `ICurrentOfficer` is used.

## Error responses (HTTP)

| Status | Condition |
|--------|-----------|
| `400` | Validation failure (invalid visit reason) |
| `201` | Success |

## Dependencies

| Dependency | Role |
|------------|------|
| `IRegistrationCaseRepository` | Persist new case |
| `ICurrentOfficer` | Default officer assignment |
| `TimeProvider` | Consistent UTC timestamps |
| `IValidator<OpenRegistrationCaseRequest>` | Input validation |
