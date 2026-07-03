# ADR-0001: No MediatR; direct handler calls

- **Status:** Accepted
- **Date:** 2026-07-03

## Context

Vertical Slice Architecture projects commonly use MediatR to dispatch requests to handlers and to attach pipeline behaviors (validation, logging, authorization). This project is educational, currently small (5–20 slices expected in the first phases), and values traceable call chains. Additionally, MediatR moved to commercial licensing in 2025.

## Decision

Use direct calls: `Endpoint → Handler → Repository`. Handlers are plain classes registered in DI and injected into Minimal API endpoints and Blazor components.

If cross-cutting pipeline behaviors ever repeat across 3+ handlers, hand-roll a minimal `IRequestHandler<TRequest, TResponse>` abstraction with decorators instead of adopting MediatR.

## Consequences

- Easier debugging and navigation for learners; no dispatch indirection.
- Cross-cutting concerns (validation, logging) are wired explicitly per endpoint group for now.
- No licensing cost or constraint.
- Revisit when: pipeline duplication becomes painful, or background-job dispatch needs the same handlers as HTTP and UI.
