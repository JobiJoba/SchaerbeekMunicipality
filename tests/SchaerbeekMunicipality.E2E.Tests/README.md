# E2E tests (Playwright)

Full-stack browser tests for the Blazor Server BFF against an in-process API host.

## Prerequisites

Install Playwright browsers once:

```bash
dotnet build tests/SchaerbeekMunicipality.E2E.Tests
pwsh tests/SchaerbeekMunicipality.E2E.Tests/bin/Debug/net10.0/playwright.ps1 install chromium
```

On macOS without PowerShell:

```bash
node tests/SchaerbeekMunicipality.E2E.Tests/bin/Debug/net10.0/.playwright/package/cli.js install chromium
```

On Linux CI, use `install chromium --with-deps` (see `.github/workflows/ci.yml`).

## Run locally

```bash
dotnet test tests/SchaerbeekMunicipality.E2E.Tests --filter "Category=E2E"
```

Optional: point at a running Web instance instead of the in-process test host:

```bash
E2E_BASE_URL=https://localhost:PORT dotnet test tests/SchaerbeekMunicipality.E2E.Tests --filter "Category=E2E"
```

## Coverage

| Journey | Test class |
|---------|------------|
| Registration intake (reception opens case) | `RegistrationIntakeE2ETests` |
| Registration confirm (population confirms approved case) | `RegistrationConfirmE2ETests` |
| Birth declaration (reception opens + population confirms) | `BirthDeclarationE2ETests` |
| Change of address (NR search → open case) | `ChangeOfAddressE2ETests` |
| Role boundary (reception blocked from case list) | `RoleBoundaryE2ETests` |

Demo officer identity is set via the `?demoOfficer={guid}` query parameter (see `DemoOfficerUrls`).
