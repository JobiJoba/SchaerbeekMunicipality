# Phase 0 — Foundation

- **Status:** Complete
- **Completed:** July 2026
- **Goal:** Runnable solution skeleton with architecture enforced by project structure and conventions.

---

## Summary

Phase 0 establishes the full solution layout — Aspire orchestration, Blazor UI, domain/infrastructure split, vertical-slice scaffolding, and CI — without implementing any business workflow beyond listing an empty registration case table. Everything is wired so Phase 1 can add real intake slices without restructuring.

---

## Deliverables checklist

| Deliverable | Status | Notes |
|-------------|--------|-------|
| Solution projects (`AppHost`, `ServiceDefaults`, `Domain`, `Infrastructure`, `Web`, test projects) | Done | See [solution layout](#solution-layout) |
| .NET Aspire AppHost with PostgreSQL | Done | Persistent data volume; Web waits for database |
| ServiceDefaults in Web | Done | `/health` and `/alive` via `MapDefaultEndpoints()` |
| Blazor Web App + MudBlazor theme | Done | Interactive Server; municipal blue palette |
| Minimal API route group scaffold | Done | `/api/registration/cases` |
| EF Core + Npgsql + initial migration | Done | Empty `registration_cases` table |
| `RegistrationCase` aggregate stub | Done | Id + `Intake` status only |
| `ICurrentOfficer` fake auth | Done | Role switcher in app bar |
| README getting started verified | Done | `dotnet run --project AppHost` |
| GitHub Actions CI | Done | Build + test on push/PR to `main` |

---

## Solution layout

```
SchaerbeekMunicipality/
├── src/
│   ├── SchaerbeekMunicipality.AppHost/          # Aspire orchestrator — local entry point
│   ├── SchaerbeekMunicipality.ServiceDefaults/  # Health, telemetry, resilience
│   ├── SchaerbeekMunicipality.Domain/           # RegistrationCase stub + repository port
│   ├── SchaerbeekMunicipality.Infrastructure/   # EF Core, migration, repository impl
│   └── SchaerbeekMunicipality.Web/              # Blazor + Minimal API + feature slices
├── tests/
│   ├── SchaerbeekMunicipality.Domain.Tests/
│   └── SchaerbeekMunicipality.Integration.Tests/
└── docs/
    ├── phases/                                  # This folder
    └── adr/                                     # Decisions taken during Phase 0
```

### Dependency graph

```
AppHost  →  Web
Web  →  ServiceDefaults, Domain, Infrastructure
Infrastructure  →  Domain
Domain  →  (no dependencies)
Tests  →  Domain / Web (not AppHost)
```

---

## What was built, by layer

### AppHost

`AppHost.cs` registers:

- A **PostgreSQL** container with a named data volume (`postgres`).
- A database resource `schaerbeek` referenced by the Web project.
- Web project startup gated on PostgreSQL readiness (`WaitFor`).

Local development starts with:

```bash
dotnet run --project src/SchaerbeekMunicipality.AppHost
```

The Aspire dashboard shows Web and PostgreSQL health. Web receives the `schaerbeek` connection string via Aspire service discovery.

### ServiceDefaults

Shared Aspire template providing:

- OpenTelemetry logging, metrics, and tracing (OTLP export when configured).
- Default health checks including a liveness `"self"` check.
- HTTP client resilience and service discovery defaults.

Web calls `AddServiceDefaults()` at startup and `MapDefaultEndpoints()` after routes are registered. Health endpoints are **Development-only** by default:

| Endpoint | Purpose |
|----------|---------|
| `/health` | Readiness — all checks pass |
| `/alive` | Liveness — `"self"` check only |

No custom Minimal API health route was added — see [ADR-0003](../adr/0003-health-endpoints-via-servicedefaults.md).

### Domain

Initial registration bounded-context stub:

| Type | Location | Purpose |
|------|----------|---------|
| `RegistrationCase` | `Domain/Registration/` | Factory `Create()` → new id, `Intake` status |
| `RegistrationCaseId` | same | Strongly typed `record` wrapping `Guid` |
| `RegistrationCaseStatus` | same | Enum; only `Intake` used so far |
| `IRegistrationCaseRepository` | same | `ListAsync`, `AddAsync`, `SaveChangesAsync` |

No lifecycle transitions, checklist flags, or domain events yet — those arrive in Phase 1+.

### Infrastructure

- **`MunicipalDbContext`** — single DbSet for `RegistrationCase`.
- **`RegistrationCaseConfiguration`** — maps to `registration_cases` with snake_case columns under PostgreSQL.
- **`RegistrationCaseRepository`** — implements the domain port.
- **`InitialCreate` migration** — creates empty `registration_cases` table.
- **`DependencyInjection.AddInfrastructure`** — provider selection by connection string:
  - SQLite (`Data Source=...`) → `EnsureCreatedAsync()` (tests).
  - Npgsql → `MigrateAsync()` in Development (Aspire runs).

Snake-case naming convention is applied for PostgreSQL via `EFCore.NamingConventions`.

### Web

**Composition root** (`Program.cs`):

1. ServiceDefaults + Infrastructure DI.
2. Blazor Interactive Server + MudBlazor.
3. Scoped `ICurrentOfficer` and `ListRegistrationCasesHandler`.
4. Database initialization on startup.
5. Razor components, registration API group, OpenAPI (Development), default health endpoints.

**Auth (educational fake):**

| Type | Role |
|------|------|
| `ICurrentOfficer` | Current officer id, display name, role, `CanApproveRegistration` |
| `CurrentOfficer` | Fixed demo officer; `SetRole` updates name and permissions |
| `OfficerRole` | `ReceptionOfficer`, `PopulationOfficer`, `PoliceClerk` |
| `RoleSwitcher` | MudSelect in app bar to switch roles at runtime |

**UI:**

- `MainLayout` — MudBlazor shell with drawer nav, officer name, role switcher, Schaerbeek branding (`#1B4F72` primary).
- `RegistrationCaseList.razor` — `/registration/cases`; calls handler directly; shows empty-state message pointing to Phase 1.
- Home page and standard error/not-found pages from the Blazor template.

**First vertical slice — `ListRegistrationCases`:**

```
Web/Features/Registration/
├── ListRegistrationCases/
│   ├── ListRegistrationCasesHandler.cs
│   └── ListRegistrationCasesEndpoint.cs
├── Pages/
│   └── RegistrationCaseList.razor
├── Components/
│   └── RoleSwitcher.razor
└── RegistrationEndpoints.cs      # MapGroup /api/registration
```

Handler loads all cases from the repository and maps to `RegistrationCaseSummary` DTOs. Blazor page injects the handler directly (no HTTP round-trip in Server mode).

### Tests

| Project | Test | What it verifies |
|---------|------|------------------|
| `Domain.Tests` | `RegistrationCaseTests.Create_ReturnsIntakeStatus` | Factory assigns non-empty id and `Intake` status |
| `Integration.Tests` | `HealthEndpointTests.Health_ReturnsOk` | `WebApplicationFactory` returns 200 on `/health` |

**`MunicipalAppFactory`** (integration test host):

- Forces `Development` environment (so `/health` is mapped).
- Overrides connection string to SQLite in-memory.
- Replaces `DbContext` registration with a shared open SQLite connection.
- Does **not** start AppHost or PostgreSQL — see [ADR-0002](../adr/0002-test-database-strategy.md).

### CI

GitHub Actions workflow (`.github/workflows/ci.yml`):

- Triggers on push/PR to `main`.
- `dotnet restore` → `dotnet build --configuration Release` → `dotnet test`.
- No Docker, no AppHost run — fast feedback on every PR.

Details: [CI.md](../CI.md).

### Documentation & decisions

Architecture docs written before/during scaffold: [ARCHITECTURE](../ARCHITECTURE.md), [DOMAIN](../DOMAIN.md), [TECH-STACK](../TECH-STACK.md), [TESTING](../TESTING.md), [GLOSSARY](../GLOSSARY.md).

ADRs accepted in Phase 0:

| ADR | Decision |
|-----|----------|
| [0001](../adr/0001-no-mediatr.md) | Direct handler calls; no MediatR |
| [0002](../adr/0002-test-database-strategy.md) | SQLite fast suite; Testcontainers PG later |
| [0003](../adr/0003-health-endpoints-via-servicedefaults.md) | Health via ServiceDefaults, not custom routes |
| [0004](../adr/0004-checklist-over-linear-state-machine.md) | Checklist + status machine (design for Phase 1+) |

Solution-wide MSBuild settings in `Directory.Build.props`: nullable enabled, warnings as errors.

---

## Demo walkthrough

1. **Start the stack**
   ```bash
   dotnet run --project src/SchaerbeekMunicipality.AppHost
   ```
2. **Aspire dashboard** — confirm `web` and `postgres` resources are healthy.
3. **Open the Web app** — link from the dashboard or the URL shown in the console.
4. **Navigate** — Home → Registration cases (`/registration/cases`).
5. **Observe** — empty list with info alert (“No registration cases yet…”).
6. **Role switcher** — change between Reception, Population, and Police; officer name in the app bar updates.
7. **API (optional)** — `GET /api/registration/cases` returns `[]`.
8. **Health (Development)** — `GET /health` returns 200.

---

## Verification commands

```bash
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release
```

Expected: **2 tests passing** (1 domain, 1 integration).

---

## Intentionally deferred to Phase 1

| Item | Phase |
|------|-------|
| `OpenRegistrationCase` slice | 1 |
| `RecordIdentity`, document upload | 1 |
| Case detail page and intake wizard | 1 |
| Domain transitions and completeness checklist | 1 |
| FluentValidation on request DTOs | 1 |
| Testcontainers PostgreSQL migration job in CI | 1 |
| Real seed data | 1+ |

---

## Related documents

- [ROADMAP.md](../ROADMAP.md) — full phased plan
- [ARCHITECTURE.md](../ARCHITECTURE.md) — slice and DDD conventions
- [TESTING.md](../TESTING.md) — test strategy per phase
- [CI.md](../CI.md) — pipeline details
