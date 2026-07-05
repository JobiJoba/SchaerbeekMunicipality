# Architecture

This document defines how the Schaerbeek Municipality educational project is structured. It is opinionated toward the chosen stack and toward **Vertical Slice Architecture (VSA)** with **lightweight Domain-Driven Design (DDD)**.

## Architectural goals

| Goal | How we achieve it |
|------|-------------------|
| Learn real-world patterns | Model Belgian municipal workflows, not toy CRUD |
| Stay navigable as the project grows | Vertical slices grouped by bounded context |
| Keep domain logic testable | `Domain` project has zero infrastructure dependencies |
| Avoid ceremony | No MediatR, generic repositories, or CQRS split unless pain appears |
| Support long-term growth | Clear boundaries now; extract services or read models later if needed |

## High-level diagram

```
┌─────────────────────────────────────────────────────────────────┐
│              SchaerbeekMunicipality.AppHost (.NET Aspire)       │
│   Orchestrates: Web project + PostgreSQL container + dashboard  │
└───────────────────────────────┬─────────────────────────────────┘
                                │ WithReference / service discovery
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│                    SchaerbeekMunicipality.Web                   │
│  (references ServiceDefaults — telemetry, health, resilience)     │
│  ┌──────────────────────┐    ┌──────────────────────────────┐  │
│  │   Blazor (MudBlazor) │    │   Minimal API endpoints      │  │
│  │   Pages / Components │◄──►│   Feature handlers           │  │
│  └──────────────────────┘    └──────────────────────────────┘  │
│              │                            │                     │
│              └────────── Features/ ───────┘                     │
│                         (vertical slices)                       │
└───────────────────────────────┬─────────────────────────────────┘
                                │
              ┌─────────────────┴─────────────────┐
              ▼                                   ▼
┌─────────────────────────┐         ┌─────────────────────────────┐
│ Domain                  │         │ Infrastructure              │
│ Aggregates, VOs, rules  │◄────────│ EF Core, DbContext, files   │
│ Domain events           │  impl   │ External system stubs       │
└─────────────────────────┘         └─────────────────────────────┘
```

The **Web** project remains the **composition root** for domain DI, endpoints, and Blazor. **AppHost** is the **runtime entry point** for local development — it wires infrastructure resources and injects configuration. **ServiceDefaults** provides cross-cutting ASP.NET concerns shared by Web (and any future services).

## Projects and dependency rules

```
AppHost  →  Web (project reference for orchestration only)
Web  →  ServiceDefaults, Domain, Infrastructure
ServiceDefaults  →  (Aspire shared packages)
Infrastructure  →  Domain
Domain  →  (nothing)
Tests  →  relevant production projects (not AppHost)
```

| Project | Runs in production? | Purpose |
|---------|---------------------|---------|
| `AppHost` | Dev / deploy tooling only | Orchestration manifest |
| `ServiceDefaults` | Yes (referenced by Web) | Observability and health |
| `Web` | Yes | Application |
| `Domain` / `Infrastructure` | Yes (as libraries) | Core logic and persistence |

**Hard rules:**

- `Domain` must not reference EF Core, ASP.NET, MudBlazor, or FluentValidation.
- Feature handlers in `Web` orchestrate: load aggregate → call domain method → persist → return DTO.
- EF Core entity configurations live in `Infrastructure`; aggregates in `Domain` are persistence-ignorant (mapping layer or owned types as needed).
- Cross-context calls go through **application handlers** or **domain events**, not direct aggregate mutation across boundaries.

## Vertical slice layout

Each use case is a folder with everything needed for that operation:

```
Web/Features/
└── Registration/
    └── OpenRegistrationCase/
        ├── OpenRegistrationCaseEndpoint.cs      # Minimal API route
        ├── OpenRegistrationCaseHandler.cs       # Orchestration
        ├── OpenRegistrationCaseRequest.cs       # Input DTO
        ├── OpenRegistrationCaseValidator.cs     # FluentValidation
        └── OpenRegistrationCaseResponse.cs      # Output DTO
```

Shared UI for a bounded context lives alongside features:

```
Web/Features/Registration/
├── Components/           # MudBlazor components used by multiple registration pages
├── RegistrationRoutes.cs
└── OpenRegistrationCase/
    └── ...
```

### Handler responsibilities

Handlers are thin orchestrators. They should:

1. Validate input (FluentValidation).
2. Load required aggregates via `Infrastructure` abstractions (e.g. `IRegistrationCaseRepository`).
3. Invoke **domain methods** that enforce invariants and emit state changes.
4. Persist through the repository / `DbContext`.
5. Map to a response DTO.

Handlers should **not** contain business rules that belong in the domain (e.g. “cannot confirm registration without police approval”).

### Health checks vs Minimal API

**Do not** add a custom `/health` Minimal API endpoint. Aspire **ServiceDefaults** already registers health checks and maps endpoints via `MapDefaultEndpoints()`:

| Endpoint | Purpose |
|----------|---------|
| `/health` | Readiness — all registered checks must pass |
| `/alive` | Liveness — process is running |

These are exposed in **Development** by default (which covers local dev and `WebApplicationFactory` smoke tests). Phase 10 adds a **database check** (`AddDbContextCheck`) to the same pipeline — still no custom route.

The Aspire **dashboard** monitors resource health separately (Web + PostgreSQL containers). That is orchestration-level; `/health` on Web is application-level.

**Minimal API** is reserved for **business use cases** under `/api/...`.

### Minimal API conventions

```csharp
// RegistrationEndpoints.cs — group related routes
public static class RegistrationEndpoints
{
    public static RouteGroupBuilder MapRegistrationEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/cases", OpenRegistrationCaseEndpoint.Handle);
        group.MapPost("/cases/{id}/identity", RecordIdentityEndpoint.Handle);
        // ...
        return group;
    }
}
```

- Route prefix: `/api/{bounded-context}` (e.g. `/api/registration/cases`).
- Use `RouteGroupBuilder` per bounded context.
- Return typed results: `Results<Ok<T>, ValidationProblem, NotFound>`.
- Problem Details for errors (`ValidationProblemDetails`, custom `ProblemDetails` for domain conflicts).

### Blazor UI conventions

- **Interactive Server** render mode for municipal desk workflows (fast iteration, no WASM payload).
- One routable page per major workflow step; wizard-style multi-step flows use shared state in a scoped service or URL-driven steps.
- Pages call the **same handlers** the API uses (inject handler directly in Server mode) OR call internal API — prefer **direct handler injection** in Blazor Server to avoid HTTP overhead during development.
- MudBlazor for layout, forms, dialogs, data grids, and snackbar notifications.

#### DbContext concurrency (Blazor Server)

`MunicipalDbContext` is scoped per Blazor circuit. Every handler and repository injected into a page shares that single instance — EF Core does not allow overlapping operations on it.

| Layer | May load from DB in lifecycle hooks? | Pattern |
|-------|--------------------------------------|---------|
| Routable page (`Pages/*.razor`) | Yes | `OnInitializedAsync` / `OnParametersSetAsync` → handlers |
| Child component (`Components/*.razor`) | **No** | Receive DTOs via `[Parameter]`; map display data synchronously |
| Layout / background refresh | Own scope | `IServiceScopeFactory.CreateAsyncScope()` (see `MainLayout.razor`) |

**Page reload rule:** finish all handler `await` calls before assigning fields that trigger child re-renders. Assign `_case`, `_auditEntries`, etc. in one batch after queries complete — not mid-method.

**Child components** must not call handlers in `OnParametersSetAsync`. If the parent DTO already contains the needed data (checklists, status flags), derive it locally. Re-querying from a child while the parent is still loading causes the classic *"second operation was started on this context"* error.

Button-click handlers (`OnSave`, `OnApprove`, …) may inject handlers directly — those run sequentially and are safe.

```
Web/Features/Registration/Pages/
├── RegistrationCaseList.razor
├── RegistrationCaseDetail.razor
└── Steps/
    ├── IdentityStep.razor
    ├── AddressStep.razor
    └── DecisionStep.razor
```

## Lightweight DDD

We use DDD tactically, not ceremonially.

### Aggregates (initial set)

| Aggregate | Root responsibility |
|-----------|---------------------|
| `RegistrationCase` | Lifecycle of a first-registration procedure |
| `Person` | Identity and civil status (may link to NR number later) |
| `Household` | Composition at an address |
| `Address` | Normalized Belgian address (value object or small aggregate) |
| `PoliceVerificationRequest` | Outbound check and result |
| `AdministrativeDocument` | Uploaded/scanned document metadata |

Start with **`RegistrationCase`** as the primary workflow aggregate. Other aggregates are referenced by ID until their slices are built.

### Value objects

Examples: `NationalRegisterNumber`, `BisNumber`, `BelgianAddress`, `DateRange`, `ResidencePermitReference`, `DocumentReference`.

Value objects are immutable, equality-by-value, and validate on construction.

### Domain events (when useful)

Use domain events for side effects **inside** the educational system:

- `RegistrationCaseOpened`
- `IdentityRecorded`
- `PoliceVerificationRequested`
- `RegistrationConfirmed`
- `RegistrationRejected`

Handlers in `Web` or `Infrastructure` subscribe to dispatch notifications, update read models, or enqueue stub “messages to other administrations.”

Do **not** introduce an event bus framework early — a simple `IDomainEventDispatcher` with in-process handlers is enough.

### Repositories

One repository interface per aggregate root in `Domain` (or in `Web/Features/.../Abstractions` if you prefer slice-local ports):

```csharp
public interface IRegistrationCaseRepository
{
    Task<RegistrationCase?> GetByIdAsync(RegistrationCaseId id, CancellationToken ct);
    Task AddAsync(RegistrationCase registrationCase, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
```

Implementation in `Infrastructure/Persistence/`. No generic `IRepository<T>`.

All repositories share the scoped `DbContext`, so a handler that touches two aggregates (e.g. `RecordPoliceResult` updates both the verification request and the case) still commits atomically with a single `SaveChangesAsync` at the end of the handler.

## Registration case lifecycle

The aggregate combines a **small status machine** with a **computed completeness checklist**. Real intake is not linear — an officer collects identity, residence category, address, household, and documents in whatever order the conversation allows — so **data collection does not advance statuses; decisions do**.

**Statuses:**

```
Intake
  → AwaitingPoliceVerification
  → UnderReview
  → Approved → Registered (terminal success)

UnderReview → Rejected (terminal)
Intake / UnderReview ⇄ Suspended (paused, resumable)
```

**Completeness checklist** — computed from case data, mirroring the four core decisions:

| Flag | True when |
|------|-----------|
| `IdentityEstablished` | Minimum identity data recorded and officer-confirmed |
| `LegalResidenceEstablished` | Residence category set and permit evidence passes policy |
| `AddressDeclared` / `AddressConfirmed` | Address declared / positive police result recorded |
| `RegisterDeterminable` | Category and outcome map to a register target |

**Data-recording methods** work during `Intake` (corrections also allowed during `UnderReview`); they update checklist flags without changing status:

```csharp
public void RecordIdentity(IdentityDetails identity, OfficerId officer)
public void DeclareAddress(BelgianAddress address, HousingSituation situation)
```

**Guarded transitions** enforce the checklist:

```csharp
public void RequestPoliceVerification()  // requires IdentityEstablished + AddressDeclared
public void RecordPoliceResult(PoliceVerificationResult result)  // → UnderReview
public void Approve(OfficerId officer, RegisterTarget register)  // requires full checklist + positive police result
public void Reject(OfficerId officer, RejectionReason reason)
public void ConfirmRegistration()  // Approved → Registered
```

Invalid transitions raise `InvalidRegistrationTransitionException`. The officer review screen renders the checklist directly from domain state — no ad-hoc UI booleans.

## Bounded contexts and integration

See [DOMAIN.md](./DOMAIN.md) for full context map. Between contexts:

| Integration style | Example |
|-------------------|---------|
| Same aggregate / case file | Identity and address steps on `RegistrationCase` |
| Reference by ID | `PoliceVerificationRequest` linked to `RegistrationCaseId` |
| Domain event | `RegistrationConfirmed` → stub “notify tax administration” |
| Anti-corruption layer | External “Immigration Office” stub returns simplified decisions |

Avoid a shared “God entity” table for Person that every module writes to.

## Cross-cutting concerns

| Concern | Approach |
|---------|----------|
| Validation | FluentValidation in Web slice; domain validates invariants in aggregates |
| Mapping | Manual mapping for small DTOs; no AutoMapper unless mapping explodes |
| Logging | `ILogger<T>` in handlers |
| Time | `TimeProvider` (or `ISystemClock`) injected for testability |
| Current user | `ICurrentOfficer` scoped service (fake auth in educational project) |
| Auditing | `AuditEntry` entity + EF interceptor or explicit `case.RecordAudit(...)` |
| File uploads | Local filesystem in dev; abstract `IDocumentStorage` |

## Authentication (educational simplification)

Phase 1: role picker or fixed test users (`ReceptionOfficer`, `PopulationOfficer`, `PoliceClerk`).

Phase 2+: ASP.NET Core Identity or OpenID Connect stub.

Authorization checks live in handlers: `if (!_currentOfficer.CanApproveRegistration) throw ...`.

## Database strategy

- **Development (Aspire):** PostgreSQL container started by AppHost; connection string injected into Web.
- **Integration tests:** SQLite in-memory with `EnsureCreated()` — tests host `Web` directly via `WebApplicationFactory`, bypassing AppHost. Migrations are PostgreSQL-specific and validated separately with Testcontainers (see [TESTING.md](./TESTING.md)).
- **Production-like deploy (Phase 10+):** PostgreSQL via Aspire publish manifest or managed cloud database.
- EF Core migrations in `Infrastructure`; provider chosen from connection string (Npgsql vs SQLite).
- Use `snake_case` column naming in PostgreSQL via EF naming convention plugin or explicit configuration.

Seeding: `Infrastructure/Persistence/Seed/` with reference data (streets, permit types, register types).

## .NET Aspire integration

**Start locally:**

```bash
dotnet run --project src/SchaerbeekMunicipality.AppHost
```

**Future resources** (added to AppHost as the project grows):

| Resource | Phase | Purpose |
|----------|-------|---------|
| PostgreSQL | 0 | Primary database |
| Immigration Office stub API | 2+ | External decision simulation |
| National Register stub API | 4+ | Duplicate search simulation |
| Blob storage (Azurite or volume) | 1+ | Document file storage |

Stub external systems become separate Aspire projects (`AddProject<...>`) so the Population Department app calls them over HTTP — closer to real inter-administration integration.

## MediatR decision

**Start without MediatR.**

Direct call chain: `Endpoint → Handler → Repository` is easier to follow in an educational codebase.

**Introduce MediatR later if:**

- Multiple pipeline behaviors repeat across slices (validation, logging, authorization wrappers).
- You want uniform request/response dispatch for Blazor and API.

If added, one handler per slice implements `IRequestHandler<TRequest, TResponse>` — still keep slice folders; do not revert to horizontal layers.

## What we avoid (until pain is real)

- Generic repository / unit of work abstractions
- Separate `Application` project with duplicate DTO layers
- CQRS with distinct read databases
- Microservices
- Event sourcing for the full registration case (audit log is enough initially)

## Naming conventions

| Element | Convention | Example |
|---------|------------|---------|
| Feature folder | Verb or verb phrase | `OpenRegistrationCase` |
| Endpoint static class | `{UseCase}Endpoint` | `OpenRegistrationCaseEndpoint` |
| Handler | `{UseCase}Handler` | `OpenRegistrationCaseHandler` |
| Domain entity IDs | Strongly typed record | `record RegistrationCaseId(Guid Value)` |
| API routes | kebab-case, plural resources | `/api/registration/cases` |
| EF tables | snake_case | `registration_cases` |

## Related documents

- [DOMAIN.md](./DOMAIN.md) — bounded contexts and aggregate details
- [TECH-STACK.md](./TECH-STACK.md) — libraries and tooling
- [TESTING.md](./TESTING.md) — how slices are tested
- [ROADMAP.md](./ROADMAP.md) — build order
