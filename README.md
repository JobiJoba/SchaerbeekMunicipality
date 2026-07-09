# Schaerbeek Municipality — Educational Population Register

An educational .NET project that simulates the **Population Department** of a Belgian municipality (Schaerbeek). The goal is to learn clean architecture, domain modeling, and full-stack development by building realistic administrative workflows — first registration of foreign citizens, birth declarations, police verification, officer decision-making, and stub outbound notifications.

This is **not** a production system. It simplifies Belgian law and inter-administration integrations while preserving the *shape* of real municipal work: identity verification, legal residence, address confirmation, register assignment, audit trails, and role-based officer UX.

**Progress:** Phases **0–12** are complete (see [ROADMAP](./docs/ROADMAP.md)). Next up: change of address, passport/ID requests, reporting, person file.

<!-- Uncomment after publishing to GitHub (replace YOUR_GITHUB_USER):
[![CI](https://github.com/YOUR_GITHUB_USER/SchaerbeekMunicipality/actions/workflows/ci.yml/badge.svg)](https://github.com/YOUR_GITHUB_USER/SchaerbeekMunicipality/actions/workflows/ci.yml)
-->

## What's built today

| Area | Capabilities |
|------|----------------|
| **First registration** | Full intake wizard — identity, residence category & permits, address & household, civil status, documents, NR search & BIS link |
| **Police verification** | Request residence check, police clerk queue, record result, case blocks until resolved |
| **Officer decision** | Four core questions checklist, approve / reject / suspend, confirm registration, stub NR assignment |
| **Role boundaries** | Reception, population officer, and police clerk roles with claim/lock on cases |
| **Review dashboard** | KPI tiles and unified **Needs my attention** queue (registration + birth declaration) |
| **Birth declaration** | Separate `BirthDeclarationCase` workflow — child details, parent NR link, medical document, household, confirm |
| **Certificates & stubs** | Residence certificate, household composition, outbound notification log |
| **Exception scenarios** | Duplicate investigation, immigration decision, birth information on applicant, etc. |
| **Azure deploy** | Container Apps + SQLite (default) or PostgreSQL profile |

Population officers land on the **review dashboard** (`/` redirects). Reception opens cases at **New case**; police clerks use **Police verifications**.

## Core questions (first registration)

Every first registration ultimately resolves four sequential decisions (see [IDEA.md](./IDEA.md)):

1. Can we establish the person's identity with sufficient certainty?
2. Do they have a legal basis to reside in Belgium?
3. Do they genuinely reside at the declared address within the municipality?
4. In which register should they be entered, and what legal status should be recorded?

Birth declaration is a **separate bounded context** — child-centric, not the same as recording an applicant's place of birth during first registration. See [phase-12-birth-declaration.md](./docs/phases/phase-12-birth-declaration.md).

## Documentation

| Document | Purpose |
|----------|---------|
| [Application overview](./docs/APP-OVERVIEW.md) | Screenshots and UI tour (partially dated — see roadmap for latest) |
| [Architecture](./docs/ARCHITECTURE.md) | Solution layout, vertical slices, DDD boundaries, API and UI conventions |
| [Domain model](./docs/DOMAIN.md) | Bounded contexts, aggregates, workflows, state machines |
| [Feature flows](./docs/features/README.md) | Per-slice documentation (registration + birth declaration index) |
| [Technology stack](./docs/TECH-STACK.md) | Stack choices, libraries, and what we deliberately avoid |
| [Roadmap](./docs/ROADMAP.md) | Phased delivery plan — phases 0–12 complete, 13–18 planned |
| [Phase delivery notes](./docs/phases/) | What was built in each completed phase |
| [Testing strategy](./docs/TESTING.md) | Unit, integration, and UI test approach |
| [Continuous integration](./docs/CI.md) | GitHub Actions (build, test, container publish) |
| [Decision records](./docs/adr/) | Architecture Decision Records for key choices |
| [Glossary](./docs/GLOSSARY.md) | Belgian administrative terminology used in the codebase |
| [Domain source](./IDEA.md) | Original process walkthrough (Population Department perspective) |

## Technology (summary)

- .NET 10, C#, Blazor Web App (Interactive Server), MudBlazor
- **.NET Aspire** — local orchestration, dashboard, and service wiring (AppHost + ServiceDefaults)
- Minimal API endpoints co-hosted with the Blazor app
- Vertical Slice Architecture with lightweight DDD
- EF Core — **PostgreSQL** locally via Aspire; **SQLite** in tests and default Azure production deploy
- FluentValidation
- MediatR **only when** cross-cutting pipelines justify it (see [TECH-STACK.md](./docs/TECH-STACK.md))

## Solution layout

```
SchaerbeekMunicipality/
├── src/
│   ├── SchaerbeekMunicipality.AppHost/         # .NET Aspire orchestrator (start here)
│   ├── SchaerbeekMunicipality.ServiceDefaults/ # Shared observability, health, resilience
│   ├── SchaerbeekMunicipality.Domain/          # Aggregates, value objects, domain events
│   │   ├── Registration/
│   │   └── BirthDeclaration/
│   ├── SchaerbeekMunicipality.Infrastructure/  # EF Core, file storage, external stubs
│   └── SchaerbeekMunicipality.Web/             # Blazor UI + Minimal API + feature slices
│       └── Features/
│           ├── Registration/                   # First-registration procedures
│           └── BirthDeclaration/               # Newborn registration (Phase 12)
├── tests/
│   ├── SchaerbeekMunicipality.Domain.Tests/
│   └── SchaerbeekMunicipality.Integration.Tests/
└── docs/
    └── phases/                                 # Delivery notes per roadmap phase
```

Feature code lives in **vertical slices** inside `Web/Features/{Context}/{UseCase}/`, not in horizontal “Services” or “Repositories” folders.

## Getting started

```bash
dotnet restore
dotnet build
dotnet test                    # 159 tests (domain + integration, SQLite)
dotnet run --project src/SchaerbeekMunicipality.AppHost
```

**Prerequisites:** .NET 10 SDK and Docker (for the PostgreSQL container started by AppHost).

1. Open the **Aspire dashboard** (URL printed in the terminal, typically `https://localhost:17148`).
2. Confirm **Web** and **PostgreSQL** are healthy.
3. Open the Web app (typically `http://localhost:5155`).
4. Use the **role switcher** in the app bar: Marie Dupont (population), Jean Martin (reception), Luc Bernard (police).

**Stop the app with Ctrl+C** — not Ctrl+Z. Suspending AppHost leaves child processes (Web, Aspire dashboard) running in the background.

Integration tests run against `Web` directly (SQLite, no AppHost). Day-to-day development uses AppHost.

### Quick demo paths

| Role | Start at | Try |
|------|----------|-----|
| Reception | New case | Open first registration or birth declaration → hand off to population |
| Population | Review dashboard | Claim unassigned case → complete intake → approve → confirm registration |
| Population | Birth declarations | Record child → link parent via NR search → attach medical PDF → confirm |
| Police | Police verifications | Complete a pending residence check |

## Deploy to Azure (optional)

Production hosting uses **Azure Container Apps** with an **ephemeral SQLite** database (fresh demo on each cold start). Images are published to **GHCR** on push to `main`.

1. Enable **Actions → Workflow permissions → Read and write** in GitHub.
2. Push to `main` (or run the **Container publish** workflow).
3. Set the GHCR package to **Public** (or configure ACA registry credentials).
4. Run `deploy/azure/sqlite/deploy.sh` with your `CONTAINER_IMAGE`.

Optional **PostgreSQL** profile: [deploy/azure/postgres/](./deploy/azure/postgres/README.md).

Full notes: [docs/phases/phase-10-azure-deployment.md](./docs/phases/phase-10-azure-deployment.md).

## Design principles

1. **Model the workflow, not the database** — tables follow the domain; the domain does not follow CRUD screens.
2. **One use case, one slice** — endpoint, handler, validator, and tests colocated.
3. **Explicit state transitions** — cases move through defined statuses; invalid transitions throw domain exceptions.
4. **Audit everything that matters** — who changed what, when, and why (educational substitute for formal Belgian audit requirements).
5. **Grow by bounded context** — add slices inside a context before adding new contexts (e.g. `BirthDeclaration` separate from `Registration`).

## License

[MIT](./LICENSE). Educational project — not affiliated with the Commune de Schaerbeek or any Belgian government body.
