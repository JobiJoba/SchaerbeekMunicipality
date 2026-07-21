# Continuous Integration

GitHub Actions runs on every push and pull request to `main`. After green tests on `main`, the same workflow builds the container image (Trivy-scanned), pushes it to GHCR, and can deploy to Azure Container Apps via OIDC.

This project is intended as a **public repository**. GitHub provides **free Actions minutes** for public repos.

## Workflows

| Workflow | File | When |
|----------|------|------|
| **CI** | [`.github/workflows/ci.yml`](../.github/workflows/ci.yml) | PR / push `main` / `workflow_dispatch` |
| **PostgreSQL migrations** | [`.github/workflows/ci-postgres.yml`](../.github/workflows/ci-postgres.yml) | Push `main` / `workflow_dispatch` (not a PR gate) |
| **CodeQL** | [`.github/workflows/codeql.yml`](../.github/workflows/codeql.yml) | PR / push `main` / weekly |

### CI jobs (`ci.yml`)

| Job | Purpose |
|-----|---------|
| **Build & test** | Restore (NuGet cache) → Release build → tests with `Category!=E2E&Category!=PostgreSQL` → upload Release `bin` artifacts |
| **E2E (Playwright)** | Download artifacts → cached Chromium → `Category=E2E` |
| **Build, scan & push image** | `main` only, after E2E: Docker Buildx + GHA cache → Trivy (HIGH/CRITICAL) → push `latest` + short SHA to GHCR |
| **Deploy to Azure** | `main` only, after publish, when `AZURE_DEPLOY_ENABLED=true`: OIDC login → `az containerapp update` with SHA-tagged image |

Concurrent runs on the same branch are cancelled when a newer commit is pushed.

No Aspire workload step: since Aspire 9, hosting packages ship as NuGet packages, so `dotnet restore` builds `AppHost`.

## What the PR path does not run

| Not on PRs | Reason |
|------------|--------|
| PostgreSQL / Testcontainers | Kept on `ci-postgres.yml` for `main` only — filter excludes `Category=PostgreSQL` |
| Container publish / Azure deploy | Image + deploy only after green CI on `main` |
| .NET Aspire AppHost | Local orchestration only |

See [TESTING.md](./TESTING.md) for the test strategy.

## Local parity

```bash
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release --verbosity normal --filter "Category!=E2E&Category!=PostgreSQL"
```

E2E (Playwright, slower):

```bash
dotnet test tests/SchaerbeekMunicipality.E2E.Tests --configuration Release --filter "Category=E2E"
```

See [tests/SchaerbeekMunicipality.E2E.Tests/README.md](../tests/SchaerbeekMunicipality.E2E.Tests/README.md) for browser install.

## Status badge

```markdown
[![CI](https://github.com/JobiJoba/SchaerbeekMunicipality/actions/workflows/ci.yml/badge.svg)](https://github.com/JobiJoba/SchaerbeekMunicipality/actions/workflows/ci.yml)
```

## Branch protection (recommended)

1. **Settings → Branches → Add rule** for `main`
2. Require status checks: **Build & test** and **E2E (Playwright)**
3. Require pull request before merging (optional but recommended)

## Security automation

| Mechanism | Location |
|-----------|----------|
| Dependabot (NuGet, Actions, Docker) | [`.github/dependabot.yml`](../.github/dependabot.yml) |
| NuGet advisory fail | `TreatWarningsAsErrors` — no blanket `NU190x` suppressions; transitive pins in [`Directory.Packages.props`](../Directory.Packages.props) |
| Trivy image scan | Publish job in `ci.yml` — fails on unfixed HIGH/CRITICAL |
| CodeQL (C#) | `codeql.yml` — `security-extended` queries |

## Public repository notes

| Topic | Guidance |
|-------|----------|
| **CI secrets** | Not required for build/test — tests use SQLite |
| **Azure CD secrets** | Optional — see [Continuous deploy](#continuous-deploy-azure-oidc) |
| **Fork PRs** | Secrets are not exposed to workflows from forks |
| **Credentials** | Never commit connection strings, API keys, or `.env` files |

## PostgreSQL migration job

SQLite tests use `EnsureCreated()` and **cannot validate** PostgreSQL migrations. [`ci-postgres.yml`](../.github/workflows/ci-postgres.yml) applies migrations against Testcontainers PostgreSQL (`Category=PostgreSQL`).

Runs on push to `main` and via `workflow_dispatch`. Keep this **off the critical PR path**.

## Continuous deploy (Azure OIDC)

After a successful image push on `main`, the **Deploy to Azure** job updates the existing Container App to the **short SHA** tag (not `:latest`). It does **not** rewrite the README URL.

### One-time setup

1. Create an Azure AD app registration (or use `az ad app create`) and a service principal.
2. Add a **federated credential** for GitHub:
   - Issuer: `https://token.actions.githubusercontent.com`
   - Subject: `repo:JobiJoba/SchaerbeekMunicipality:ref:refs/heads/main`
   - Audience: `api://AzureADTokenExchange`
3. Grant the principal **Contributor** (or a tighter custom role) on resource group `schaerbeek-rg`.
4. In the GitHub repo **Settings → Secrets and variables → Actions**:
   - Secrets: `AZURE_CLIENT_ID`, `AZURE_TENANT_ID`, `AZURE_SUBSCRIPTION_ID`
   - Variable: `AZURE_DEPLOY_ENABLED` = `true`
   - Optional variables: `AZURE_RESOURCE_GROUP` (default `schaerbeek-rg`), `AZURE_CONTAINER_APP_NAME` (default `schaerbeek-web`)
5. Ensure the app already exists (first-time infra: run [`deploy/azure/sqlite/deploy.sh`](../deploy/azure/sqlite/deploy.sh) manually).

Until `AZURE_DEPLOY_ENABLED` is set, publish still runs; deploy is skipped.

Full Bicep / first-time deploy remains manual — see [deploy/azure/README.md](../deploy/azure/README.md). Postgres profile is not auto-deployed.

Optional `HEALTH_CHECK_API_KEY` remains available in app config but is **not** wired into Bicep probes (probe header wiring is easy to misconfigure for a demo).

## Troubleshooting

| Failure | Likely cause |
|---------|--------------|
| Test fails on CI but passes locally | Case-sensitive paths on Linux, or missing test isolation |
| Build fails on AppHost | Aspire package version mismatch — align `Aspire.Hosting.*` versions |
| Trivy fails publish | Fix or upgrade base/app packages; `ignore-unfixed` is enabled for HIGH/CRITICAL |
| Deploy skipped | `AZURE_DEPLOY_ENABLED` not `true`, or Azure OIDC secrets missing |
| Deploy fails auth | Federated credential subject/repo mismatch, or missing RBAC on the resource group |

## Related documents

- [TESTING.md](./TESTING.md) — test layers and what runs in CI
- [TECH-STACK.md](./TECH-STACK.md) — stack and tooling
- [deploy/azure/README.md](../deploy/azure/README.md) — Bicep profiles and manual deploy
- [ROADMAP.md](./ROADMAP.md) — phased delivery
