# ADR-0005: Deployment database by environment

- **Status:** Accepted
- **Date:** 2026-07-06

## Context

The project uses PostgreSQL locally via Aspire and SQLite in the fast integration-test suite ([ADR-0002](./0002-test-database-strategy.md)). Phase 10 adds Azure Container Apps deployment. Managed PostgreSQL on Azure adds ~$16+/month even when the web app scales to zero. For an educational portfolio demo, ephemeral state on cold start is acceptable.

## Decision

Use **environment-specific database profiles** with a single codebase and connection-string switching in `DependencyInjection`:

| Environment | Database | Schema | Persistence |
|-------------|----------|--------|-------------|
| Local (AppHost) | PostgreSQL | `MigrateAsync` | Yes |
| Tests | SQLite `:memory:` | `EnsureCreated` | No |
| **Azure default (production)** | SQLite on container disk (`/tmp/schaerbeek.db`) | `EnsureCreated` + seeders | No — fresh demo on cold start |
| **Azure optional** | PostgreSQL Flexible Server | `MigrateAsync` + seeders | Yes |

Container images are published to **GHCR** (not ACR). Azure deploy uses Bicep + shell scripts under `deploy/azure/`.

## Consequences

- Production SQLite does not validate EF migrations; the Testcontainers PostgreSQL CI job remains important ([ADR-0002](./0002-test-database-strategy.md)).
- Officers lose in-progress cases and uploads when ACA scales to zero or replaces the container — acceptable for the default demo profile.
- Optional PostgreSQL deploy script exists for a production-like path without changing application code.
- `aspire publish` / ACR are not required for the primary deployment story.

## Revisit when

- Always-on production hosting with persistent cases is required → use the Azure PostgreSQL profile or external managed Postgres.
- Full integration suite runs on PostgreSQL in every PR → expand Testcontainers coverage per ADR-0002.
