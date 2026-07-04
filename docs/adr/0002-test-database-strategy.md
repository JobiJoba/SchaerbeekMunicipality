# ADR-0002: SQLite fast suite + Testcontainers PostgreSQL for migrations

- **Status:** Accepted
- **Date:** 2026-07-03

## Context

Development runs PostgreSQL via .NET Aspire; tests should be fast and runnable without Docker. A naive "same migrations, different provider" approach does not work: migrations generated for Npgsql contain PostgreSQL-specific SQL (snake_case naming, PG column types), and SQLite lacks several migration operations (e.g. most `ALTER COLUMN`).

## Decision

Two-tier strategy:

1. **Fast suite (default, every PR):** SQLite `:memory:` with `EnsureCreated()` inside `WebApplicationFactory`. Validates the EF model and handler behavior. Does **not** validate migrations.
2. **Migration validation (separate CI job, from Phase 1):** Testcontainers PostgreSQL applies all migrations and runs tests tagged `[Trait("Category", "PostgreSQL")]`. Off the PR critical path (nightly or push-to-main).

## Consequences

- `dotnet test` stays fast and Docker-free locally and in PR CI.
- Migration bugs are caught by the PostgreSQL job, not the fast suite — a red nightly run must be treated as blocking.
- Provider-specific behavior differences (collations, case sensitivity) can slip through the SQLite suite; Phase 10 expands full-suite PostgreSQL coverage.
- Revisit when: the PostgreSQL job becomes fast enough to run on every PR, at which point SQLite could be dropped entirely.
