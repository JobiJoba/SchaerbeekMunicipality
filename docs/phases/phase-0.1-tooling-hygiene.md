# Phase 0.1 — Tooling hygiene

- **Status:** Complete
- **Completed:** July 2026
- **Goal:** Modernize solution and dependency management before Phase 1 adds packages and slices.

---

## Summary

Phase 0.1 is a small infrastructure pass on top of [Phase 0](./phase-0-foundation.md). No business logic changed. The solution file was migrated to the new SLNX format, and all NuGet package versions were moved to a single central manifest so future bumps (EF Core, Aspire, test libraries, FluentValidation in Phase 1) happen in one place.

---

## Deliverables checklist

| Deliverable | Status | Notes |
|-------------|--------|-------|
| Migrate `SchaerbeekMunicipality.sln` → `SchaerbeekMunicipality.slnx` | Done | Generated via `dotnet solution migrate` |
| Remove legacy `.sln` file | Done | SLNX is the source of truth |
| Add `Directory.Packages.props` (Central Package Management) | Done | 24 packages; versions grouped by area |
| Strip inline `Version=` from all `.csproj` files | Done | Projects reference packages by name only |
| Build + test verification | Done | Same 2 tests passing |

---

## SLNX migration

The classic solution file (`Format Version 12.00`, GUIDs, platform matrix) was replaced by a compact XML file:

```
SchaerbeekMunicipality.slnx
```

Structure:

- `/src/` folder — AppHost, ServiceDefaults, Domain, Infrastructure, Web
- `/tests/` folder — Domain.Tests, Integration.Tests

**Why:** SLNX is the direction for .NET 10+ tooling — easier to read, diff, and merge than the legacy `.sln` format. Project references and build behavior are unchanged.

**Migration command** (for reference):

```bash
dotnet solution migrate
```

**IDE / CLI:** Open or build with `SchaerbeekMunicipality.slnx`. CI still works with bare `dotnet restore/build/test` (no solution path required), but you can pass the `.slnx` explicitly for clarity.

---

## Central Package Management (CPM)

All package versions now live in [`Directory.Packages.props`](../../Directory.Packages.props) at the repository root.

```xml
<PropertyGroup>
  <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
</PropertyGroup>
```

Projects declare dependencies without versions:

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" />
```

### Package groups

| Group | Examples | Purpose |
|-------|----------|---------|
| Aspire | `Aspire.Hosting.PostgreSQL` | AppHost orchestration |
| Microsoft — ASP.NET / hosting | `Microsoft.AspNetCore.OpenApi`, `Microsoft.Extensions.*` | Web + ServiceDefaults |
| Microsoft — EF Core | `Microsoft.EntityFrameworkCore.*`, `Npgsql.*`, `EFCore.NamingConventions` | Infrastructure + tests |
| OpenTelemetry | `OpenTelemetry.*` | ServiceDefaults telemetry (Aspire template defaults) |
| UI | `MudBlazor` | Blazor components |
| Testing | `xunit`, `FluentAssertions`, `coverlet.collector`, etc. | Test projects |

Shared MSBuild settings (`Nullable`, `TreatWarningsAsErrors`, etc.) remain in [`Directory.Build.props`](../../Directory.Build.props).

### What CPM does *not* cover

The **Aspire AppHost SDK** is pinned in the AppHost project file, not in `Directory.Packages.props`:

```xml
<Project Sdk="Aspire.AppHost.Sdk/13.0.2">
```

When bumping Aspire, update **both** the SDK version in `SchaerbeekMunicipality.AppHost.csproj` and `Aspire.Hosting.PostgreSQL` in `Directory.Packages.props`.

---

## Version policy (unchanged from Phase 0)

This phase moved versions — it did **not** upgrade them. Notable pins carried forward:

| Package | Version | Reason |
|---------|---------|--------|
| **FluentAssertions** | 7.2.0 | v8+ has a different license model; v7 stays Apache-2.0 — see [TECH-STACK.md](../TECH-STACK.md) |
| **MudBlazor** | 8.15.0 | v9 is a major UI migration — defer to a dedicated pass |
| **Microsoft packages** | Mixed 10.0.0 / 10.0.4 | Template pins; align in a future bump via CPM |

**FluentValidation** is still not referenced — planned for Phase 1 intake slices.

---

## How to bump a package

1. Edit the version in `Directory.Packages.props`.
2. Run:

```bash
dotnet restore SchaerbeekMunicipality.slnx
dotnet build SchaerbeekMunicipality.slnx --configuration Release
dotnet test SchaerbeekMunicipality.slnx --configuration Release
```

3. For Aspire: also update `Aspire.AppHost.Sdk` in the AppHost `.csproj`.

To see what is outdated:

```bash
dotnet list package --outdated
```

---

## Verification commands

```bash
dotnet restore SchaerbeekMunicipality.slnx
dotnet build SchaerbeekMunicipality.slnx --configuration Release
dotnet test SchaerbeekMunicipality.slnx --configuration Release
```

Expected: **2 tests passing** (1 domain, 1 integration) — same as Phase 0.

---

## Intentionally deferred

| Item | When |
|------|------|
| Align all Microsoft.* packages to one patch (e.g. 10.0.9) | Optional hygiene PR |
| Bump Aspire / OpenTelemetry to latest minors | After smoke-testing AppHost |
| MudBlazor 8 → 9 | Separate UI migration |
| Add FluentValidation to CPM | Phase 1 |
| Point CI explicitly at `.slnx` | Optional |

---

## Related documents

- [phase-0-foundation.md](./phase-0-foundation.md) — original solution layout
- [TECH-STACK.md](../TECH-STACK.md) — library choices and licensing notes
- [CI.md](../CI.md) — pipeline (unchanged behavior)
