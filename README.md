# Schaerbeek Municipality — Educational Population Register

An educational .NET project that simulates the **Population Department** of a Belgian municipality (Schaerbeek). The goal is to learn clean architecture, domain modeling, and full-stack development by gradually building a realistic administrative workflow — starting with foreign citizen first registration.

This is **not** a production system. It simplifies Belgian law and inter-administration integrations while preserving the *shape* of real municipal work: identity verification, legal residence, address confirmation, register assignment, and audit trails.

<!-- Uncomment after publishing to GitHub (replace YOUR_GITHUB_USER):
[![CI](https://github.com/YOUR_GITHUB_USER/SchaerbeekMunicipality/actions/workflows/ci.yml/badge.svg)](https://github.com/YOUR_GITHUB_USER/SchaerbeekMunicipality/actions/workflows/ci.yml)
-->

## Core questions the system must answer

Every first registration ultimately resolves four sequential decisions (see [IDEA.md](./IDEA.md)):

1. Can we establish the person's identity with sufficient certainty?
2. Do they have a legal basis to reside in Belgium?
3. Do they genuinely reside at the declared address within the municipality?
4. In which register should they be entered, and what legal status should be recorded?

## Documentation

| Document | Purpose |
|----------|---------|
| [Architecture](./docs/ARCHITECTURE.md) | Solution layout, vertical slices, DDD boundaries, API and UI conventions |
| [Domain model](./docs/DOMAIN.md) | Bounded contexts, aggregates, workflows, state machines |
| [Technology stack](./docs/TECH-STACK.md) | Stack choices, libraries, and what we deliberately avoid |
| [Roadmap](./docs/ROADMAP.md) | Phased delivery plan from MVP to advanced scenarios |
| [Phase delivery notes](./docs/phases/) | What was built in each completed phase (start with [Phase 0](./docs/phases/phase-0-foundation.md)) |
| [Testing strategy](./docs/TESTING.md) | Unit, integration, and UI test approach |
| [Continuous integration](./docs/CI.md) | GitHub Actions workflow for public repo |
| [Decision records](./docs/adr/) | Architecture Decision Records for key choices |
| [Glossary](./docs/GLOSSARY.md) | Belgian administrative terminology used in the codebase |
| [Domain source](./IDEA.md) | Original process walkthrough (Population Department perspective) |

## Technology (summary)

- .NET 10, C#, Blazor Web App, MudBlazor
- **.NET Aspire** — local orchestration, dashboard, and service wiring (AppHost + ServiceDefaults)
- Minimal API endpoints co-hosted with the Blazor app
- Vertical Slice Architecture with lightweight DDD
- EF Core (SQLite for fast tests → PostgreSQL via Aspire in dev/prod-like runs)
- FluentValidation
- MediatR **only when** cross-cutting pipelines justify it (see [TECH-STACK.md](./docs/TECH-STACK.md))

## Solution layout

```
SchaerbeekMunicipality/
├── src/
│   ├── SchaerbeekMunicipality.AppHost/        # .NET Aspire orchestrator (start here)
│   ├── SchaerbeekMunicipality.ServiceDefaults/ # Shared observability, health, resilience
│   ├── SchaerbeekMunicipality.Domain/           # Aggregates, value objects, domain events
│   ├── SchaerbeekMunicipality.Infrastructure/ # EF Core, file storage, external stubs
│   └── SchaerbeekMunicipality.Web/            # Blazor UI + Minimal API + feature slices
├── tests/
│   ├── SchaerbeekMunicipality.Domain.Tests/       # Pure domain rules, no I/O
│   └── SchaerbeekMunicipality.Integration.Tests/  # Handlers, API, EF, selective bUnit
└── docs/
    └── phases/                                  # Delivery notes per roadmap phase
```

Feature code lives in **vertical slices** inside `Web/Features/{Context}/{UseCase}/`, not in horizontal “Services” or “Repositories” folders.

## Getting started

```bash
dotnet restore
dotnet build
dotnet test
dotnet run --project src/SchaerbeekMunicipality.AppHost   # starts Web + PostgreSQL + Aspire dashboard
```

**Prerequisites:** .NET 10 SDK and Docker (for the PostgreSQL container started by AppHost).

Integration tests run against `Web` directly (SQLite, no AppHost). Day-to-day development uses the AppHost.

**Phase 0 demo:** Open the Aspire dashboard → confirm Web and PostgreSQL are healthy → browse to the Web app → empty registration case list → switch officer role in the app bar.

## Design principles

1. **Model the workflow, not the database** — tables follow the domain; the domain does not follow CRUD screens.
2. **One use case, one slice** — endpoint, handler, validator, and tests colocated.
3. **Explicit state transitions** — registration cases move through defined statuses; invalid transitions throw domain exceptions.
4. **Audit everything that matters** — who changed what, when, and why (educational substitute for formal Belgian audit requirements).
5. **Grow by bounded context** — add slices inside a context before adding new contexts.

## License

[MIT](./LICENSE). Educational project — not affiliated with the Commune de Schaerbeek or any Belgian government body.
