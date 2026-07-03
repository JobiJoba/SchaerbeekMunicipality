# ADR-0003: Health endpoints via Aspire ServiceDefaults

- **Status:** Accepted
- **Date:** 2026-07-03

## Context

The app needs health endpoints for smoke tests, orchestration, and eventual deployment probes. Aspire's ServiceDefaults project already registers health checks and maps `/health` (readiness) and `/alive` (liveness) via `MapDefaultEndpoints()` — by default only in the Development environment.

## Decision

Do not write a custom health Minimal API endpoint. Use ServiceDefaults' `/health` and `/alive`. Minimal API is reserved for business use cases under `/api/...`. Domain-specific checks (e.g. `AddDbContextCheck`) plug into the same health-check pipeline in Phase 9.

## Consequences

- One less hand-rolled endpoint; smoke tests hit `/health` through `WebApplicationFactory` (Development environment, so endpoints are mapped).
- Production deployment (Phase 9) must explicitly expose health endpoints outside Development, with response caching and access restrictions.
- Revisit when: deployment target requires custom probe semantics ServiceDefaults cannot express.
