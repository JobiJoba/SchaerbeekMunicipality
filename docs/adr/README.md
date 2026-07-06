# Architecture Decision Records

Short, dated records of significant technical decisions and their rationale. When a decision is revisited, write a new ADR that supersedes the old one — do not rewrite history.

## Index

| ADR | Title | Status |
|-----|-------|--------|
| [0001](./0001-no-mediatr.md) | No MediatR; direct handler calls | Accepted |
| [0002](./0002-test-database-strategy.md) | SQLite fast suite + Testcontainers PostgreSQL for migrations | Accepted |
| [0003](./0003-health-endpoints-via-servicedefaults.md) | Health endpoints via Aspire ServiceDefaults | Accepted |
| [0004](./0004-checklist-over-linear-state-machine.md) | Completeness checklist over linear state machine | Accepted |
| [0005](./0005-deployment-database-by-environment.md) | Deployment database by environment (local PG, Azure SQLite/PG) | Accepted |

## Template

```markdown
# ADR-NNNN: Title

- **Status:** Proposed | Accepted | Superseded by ADR-XXXX
- **Date:** YYYY-MM-DD

## Context

What situation or problem forces a decision?

## Decision

What we chose, stated plainly.

## Consequences

What becomes easier, what becomes harder, and what would trigger revisiting this.
```
