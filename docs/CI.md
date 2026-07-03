# Continuous Integration

GitHub Actions runs on every push and pull request to `main`. The pipeline validates that the solution builds and all tests pass.

This project is intended as a **public repository**. GitHub provides **free Actions minutes** for public repos, which is more than enough for restore → build → test on each PR.

## Workflow

File: [`.github/workflows/ci.yml`](../.github/workflows/ci.yml)

| Step | Purpose |
|------|---------|
| Checkout | Clone the repository |
| Setup .NET 10 | Install SDK via `actions/setup-dotnet` |
| Restore | `dotnet restore` |
| Build | `dotnet build --configuration Release` |
| Test | `dotnet test --configuration Release` |

No Aspire workload step: since Aspire 9, hosting and integration libraries ship as regular NuGet packages, so `dotnet restore` is sufficient to build `AppHost`.

Triggers:

- Push to `main`
- Pull request targeting `main`

Concurrent runs on the same branch are cancelled when a newer commit is pushed.

## What CI does not run

By design, CI stays fast and Docker-free:

| Not in CI | Reason |
|-----------|--------|
| .NET Aspire AppHost | Local dev orchestration only |
| PostgreSQL container | Integration tests use SQLite via `WebApplicationFactory` |
| `dotnet run` | No smoke test against a live server in Phase 0 (add later if needed) |

See [TESTING.md](./TESTING.md) for the test strategy.

## Local parity

Run the same commands locally before pushing:

```bash
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release --verbosity normal
```

## Status badge

After the repository is published on GitHub, add a badge to [README.md](../README.md). Replace `YOUR_GITHUB_USER` with your username or organisation:

```markdown
[![CI](https://github.com/YOUR_GITHUB_USER/SchaerbeekMunicipality/actions/workflows/ci.yml/badge.svg)](https://github.com/YOUR_GITHUB_USER/SchaerbeekMunicipality/actions/workflows/ci.yml)
```

## Branch protection (recommended)

Once the repository is on GitHub, enable branch protection on `main`:

1. **Settings → Branches → Add rule**
2. Require status check: **Build & test**
3. Require pull request before merging (optional but recommended for learning)

## Public repository notes

| Topic | Guidance |
|-------|----------|
| **Secrets** | Not needed for CI — tests override connection strings to SQLite |
| **Fork PRs** | Secrets are not exposed to workflows from forks (safe default) |
| **Dependencies** | NuGet packages are restored from public feeds only |
| **Credentials** | Never commit connection strings, API keys, or `.env` files |

## PostgreSQL migration job (from Phase 1)

The SQLite suite creates its schema with `EnsureCreated()` and **cannot validate migrations** (they are PostgreSQL-specific). Once the first real migration exists (Phase 1), add a separate job that applies all migrations against Testcontainers PostgreSQL:

```yaml
# .github/workflows/ci-postgres.yml (add in Phase 1, when the first migration exists)
jobs:
  integration-postgres:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'
      # Testcontainers requires Docker — available on ubuntu-latest
      - run: dotnet test tests/SchaerbeekMunicipality.Integration.Tests --filter Category=PostgreSQL
```

Keep this **off the critical PR path** (nightly or push-to-main trigger) so PR feedback stays fast. SQLite tests remain the gate for merge.

## Troubleshooting

| Failure | Likely cause |
|---------|--------------|
| `dotnet restore` — project not found | Solution not scaffolded yet (Phase 0) |
| Test fails on CI but passes locally | Case-sensitive paths on Linux, or missing test isolation — check parallel test DB usage |
| Build fails on AppHost | Aspire package version mismatch with the SDK — align `Aspire.Hosting.*` versions |

## Related documents

- [TESTING.md](./TESTING.md) — test layers and what runs in CI
- [TECH-STACK.md](./TECH-STACK.md) — stack and tooling
- [ROADMAP.md](./ROADMAP.md) — Phase 0 deliverables
