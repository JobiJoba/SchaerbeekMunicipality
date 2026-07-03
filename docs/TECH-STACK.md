# Technology stack

Rationale for each choice in the Schaerbeek Municipality project, aligned with educational goals and long-term growth.

## Stack overview

| Layer | Choice | Version target |
|-------|--------|----------------|
| Runtime | .NET | 10 (latest stable) |
| Language | C# | 13+ features as available |
| Orchestration | .NET Aspire | AppHost + ServiceDefaults |
| UI | Blazor Web App | .NET 10 template |
| Components | MudBlazor | Latest compatible with .NET 10 |
| API | Minimal API | Co-hosted in Web project |
| Architecture | Vertical Slice + lightweight DDD | See ARCHITECTURE.md |
| Validation | FluentValidation | Latest |
| ORM | EF Core | 10 |
| Test database (fast suite) | SQLite | `:memory:` + `EnsureCreated`; validates model, not migrations |
| Migration validation | PostgreSQL via Testcontainers | Separate CI job (`Category=PostgreSQL`) |
| Dev / prod-like database | PostgreSQL | Container via Aspire; Npgsql provider |
| Testing | xUnit, FluentAssertions, NSubstitute, Testcontainers | — |

---

## .NET 10 and C#

- Single modern runtime for UI, API, domain, and tests.
- Native `TimeProvider`, improved performance, and current LTS cadence.
- Enable **nullable reference types** and **implicit usings** in all projects.
- Treat warnings as errors in CI (`TreatWarningsAsErrors` once scaffold exists).

---

## .NET Aspire

**Why Aspire:**

- Single entry point (`AppHost`) to run the Blazor app, PostgreSQL, and future dependencies together.
- Built-in **dashboard** for logs, traces, and resource health — valuable while learning distributed-style setups on a monolith.
- **Service discovery** and **connection string injection** — Web receives `ConnectionStrings__postgres` (or similar) without hardcoding ports.
- **ServiceDefaults** centralizes OpenTelemetry, health checks, and HTTP resilience patterns once, reused by `Web`.
- Natural path to add stub external services later (Immigration Office API, National Register mock) as Aspire resources.

**Solution projects:**

| Project | Role |
|---------|------|
| `SchaerbeekMunicipality.AppHost` | Orchestrator — defines resources and project references |
| `SchaerbeekMunicipality.ServiceDefaults` | Shared extensions (`AddServiceDefaults`, `MapDefaultEndpoints`) |
| `SchaerbeekMunicipality.Web` | References ServiceDefaults; business composition root unchanged |

**AppHost (planned shape):**

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .AddDatabase("schaerbeek");

builder.AddProject<Projects.SchaerbeekMunicipality_Web>("web")
    .WithReference(postgres)
    .WaitFor(postgres);

builder.Build().Run();
```

**Web project integration:**

```csharp
// Program.cs
builder.AddServiceDefaults();
// ...
var app = builder.Build();
app.MapDefaultEndpoints(); // /health, /alive in Development — no custom health endpoint needed
```

**Health checks:** ServiceDefaults owns `/health` and `/alive`. Add domain-specific checks (e.g. EF Core database) via `AddHealthChecks()` in Phase 9 — not a separate Minimal API route. The Aspire dashboard additionally shows container and project resource status.

**Database strategy with Aspire:**

| Mode | How | When |
|------|-----|------|
| **Aspire + PostgreSQL** | `dotnet run --project AppHost` | Daily development, demo, prod-like runs |
| **SQLite (test factory)** | `EnsureCreated()` in `WebApplicationFactory` | Fast integration tests — no Docker |
| **Testcontainers PostgreSQL** | `Category=PostgreSQL` tests | Migration validation in CI |
| **Aspire deployment** | `aspire publish` / manifest (Phase 9+) | Deploy to Azure Container Apps or Docker |

SQLite remains for **tests only** — not wired through AppHost. This keeps `dotnet test` fast while dev runs use real PostgreSQL in a container.

**Prerequisites:**

- .NET 10 SDK
- Docker Desktop (or compatible container runtime) for PostgreSQL container
- No separate Aspire workload — since Aspire 9, hosting and integration packages ship as NuGet packages (`dotnet workload install aspire` is deprecated)

**Start command:**

```bash
dotnet run --project src/SchaerbeekMunicipality.AppHost
```

Opens the Aspire dashboard (default) and launches `Web` with injected PostgreSQL connection string.

---

## Blazor Web App + MudBlazor

**Why Blazor Web App (not WASM-only):**

- Municipal desk workflows fit **Interactive Server**: rich forms, fast dev loop, no large client download.
- Same assembly hosts UI and Minimal API — one deployable unit for learning.
- Can add Interactive WebAssembly later for specific public-facing kiosks if desired.

**Why MudBlazor:**

- Mature Material Design component set: forms, dialogs, tables, steppers, snackbars.
- Good fit for data-entry-heavy admin UIs (population officers).
- Themeable for eventual “Commune de Schaerbeek” branding.

**UI patterns:**

- `MainLayout` with role-aware navigation
- `MudForm` + FluentValidation validator integration
- `MudStepper` or wizard pages for registration workflow
- `MudDataGrid` for case lists and register search results

---

## Minimal API

**Why not controllers:**

- Less ceremony for slice-based endpoints.
- Colocated static `Handle` methods map cleanly from feature folders.
- Still supports OpenAPI via `Microsoft.AspNetCore.OpenApi` for learning API documentation.

**Blazor ↔ backend:**

- Prefer **direct handler injection** in Interactive Server components (same process).
- Keep Minimal API routes for integration tests, future mobile clients, and HTTP contract clarity.

---

## Vertical Slice Architecture

Features are organized as **use cases**, not technical layers.

Benefits for this project:

- Each IDEA phase maps to one or more slices you can build and demo independently.
- New contributors open one folder and see the full story.
- Tests mirror production folder structure.

See [ARCHITECTURE.md](./ARCHITECTURE.md) for folder layout.

---

## Lightweight DDD

**Use:**

- Aggregate roots with explicit methods
- Value objects for identifiers and addresses
- Domain exceptions for rule violations
- Domain events for significant outcomes (in-process)

**Skip (initially):**

- Full strategic DDD workshops-style context mapping in code
- Separate domain services for every rule — prefer methods on aggregates first
- Specification pattern unless queries become complex

---

## FluentValidation

- Validators live next to handlers in each slice (`OpenRegistrationCaseValidator`).
- ASP.NET integration: `AddValidatorsFromAssemblyContaining<...>()` + automatic validation filter on Minimal API groups.
- **Input validation** in FluentValidation; **business invariants** in domain aggregates.

Example split:

- FluentValidation: “Postal code is required, 4 digits”
- Domain: “Cannot approve case without police confirmation”

---

## EF Core + SQLite / PostgreSQL

**Development — PostgreSQL via Aspire:**

- AppHost starts a PostgreSQL container with a persistent data volume.
- Web reads the connection string from Aspire configuration (`WithReference(postgres)`).
- Migrations run on startup in Development (or via explicit `dotnet ef database update`).

**Tests — SQLite fast suite + Testcontainers for migrations:**

- The fast suite uses SQLite `:memory:` with `EnsureCreated()` — validates the EF model and handler behavior without Docker.
- Migrations target PostgreSQL (snake_case, Npgsql types) and are **not** runnable on SQLite. They are validated by Testcontainers PostgreSQL tests (`Category=PostgreSQL`) in a separate CI job — see [TESTING.md](./TESTING.md).

**PostgreSQL conventions:**

- Use `UseSnakeCaseNamingConvention()` (EFCore.NamingConventions package) for PostgreSQL idioms.
- Npgsql provider configured when connection string indicates PostgreSQL.

**Modeling notes:**

- Aggregates map to tables with clear boundaries; avoid lazy-loading across aggregates.
- `AuditEntry` and `CaseAuditEntry` as owned collections or separate table.
- Document binary content **not** in DB — store path + metadata only.

**Migrations:**

```bash
dotnet ef migrations add InitialCreate \
  --project src/SchaerbeekMunicipality.Infrastructure \
  --startup-project src/SchaerbeekMunicipality.Web
```

Apply against the Aspire-managed database (AppHost running, or connection string from dashboard):

```bash
dotnet ef database update \
  --project src/SchaerbeekMunicipality.Infrastructure \
  --startup-project src/SchaerbeekMunicipality.Web
```

---

## MediatR — deliberate omission (for now)

| With MediatR | Without MediatR |
|--------------|-----------------|
| Uniform pipeline behaviors | Explicit `Handler.Handle(request)` |
| Extra indirection for learners | Easier debugging and stack traces |
| Nice at 50+ slices | Better at 5–20 slices |

**Adopt a pipeline when** you have 3+ cross-cutting behaviors duplicated across handlers, or you need identical dispatch from Blazor, API, and background jobs.

**Licensing note:** MediatR moved to commercial licensing in 2025. If a pipeline ever becomes necessary, prefer hand-rolling a minimal `IRequestHandler<TRequest, TResponse>` interface plus decorators — license-free and more educational than adopting a dependency for ~50 lines of dispatch code.

If a pipeline is adopted: one request type per slice; handlers stay in slice folders.

---

## Testing libraries

| Library | Role |
|---------|------|
| xUnit | Test runner |
| FluentAssertions | Readable assertions — **v8+ is free only for non-commercial/OSS use**; fine for this project, or pin v7 (Apache-2.0) |
| NSubstitute | Mocking ports |
| Microsoft.AspNetCore.Mvc.Testing | WebApplicationFactory for API tests |
| bUnit | Blazor component tests (selective) |
| Testcontainers.PostgreSql | Migration validation job (`Category=PostgreSQL`) |

---

## Tooling and quality

| Tool | Purpose |
|------|---------|
| `dotnet format` | Consistent formatting |
| EditorConfig | Shared editor rules |
| GitHub Actions | build + test on push/PR — see [CI.md](./CI.md) |
| Swagger / OpenAPI | API exploration |

Optional later: **Stryker** mutation testing on domain project.

---

## Packages (expected)

**AppHost:**

- `Aspire.Hosting.AppHost`
- `Aspire.Hosting.PostgreSQL`

**ServiceDefaults:**

- `Microsoft.Extensions.ServiceDiscovery`
- `OpenTelemetry.*` (via Aspire template defaults)
- `Microsoft.Extensions.Http.Resilience`

**Web:**

- Project reference to `ServiceDefaults`
- `MudBlazor`
- `FluentValidation.DependencyInjectionExtensions`
- `Microsoft.AspNetCore.OpenApi`

**Infrastructure:**

- `Microsoft.EntityFrameworkCore.Sqlite`
- `Npgsql.EntityFrameworkCore.PostgreSQL` (when enabling PG)
- `EFCore.NamingConventions` (PG)

**Tests:**

- `Microsoft.NET.Test.Sdk`
- `xunit`
- `FluentAssertions`
- `Microsoft.AspNetCore.Mvc.Testing`

No package bloat — add packages when a slice needs them.

---

## Configuration

```
appsettings.json
appsettings.Development.json   → fallback / local overrides
```

**Connection strings:**

| Source | Used by |
|--------|---------|
| Aspire injection (`WithReference`) | `dotnet run` via AppHost → PostgreSQL |
| Test factory override | Integration tests → SQLite |
| User Secrets / env vars | Standalone Web run (optional, without AppHost) |

Aspire injects credentials automatically — do not commit connection strings.

Secrets: **User Secrets** for non-Aspire standalone runs; Aspire manages container credentials in dev.

---

## Deployment

**Local (default):** `dotnet run --project src/SchaerbeekMunicipality.AppHost`

**Phase 9+ — Aspire deployment:**

- `aspire publish` generates deployment manifests
- Target: Azure Container Apps, Kubernetes, or Docker Compose (generated from manifest)
- Replaces hand-written Docker Compose as the primary deployment story

Educational note: learn orchestration locally first; cloud deploy is optional stretch goal.

---

## Related documents

- [ARCHITECTURE.md](./ARCHITECTURE.md)
- [TESTING.md](./TESTING.md)
- [CI.md](./CI.md)
- [ROADMAP.md](./ROADMAP.md)
