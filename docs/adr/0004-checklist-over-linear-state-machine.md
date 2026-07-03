# ADR-0004: Completeness checklist over linear state machine

- **Status:** Accepted
- **Date:** 2026-07-03

## Context

The first design gave `RegistrationCase` a strictly linear status pipeline (`Draft → IdentityPending → ResidenceCategoryPending → AddressDeclared → DocumentsPending → …`). Real municipal intake is not linear: officers collect identity, residence category, address, household, and documents in whatever order the conversation allows. The linear model also produced backwards semantics (recording identity *entered* `IdentityPending` instead of clearing it).

## Decision

Split the model in two:

- **Small status machine** for decisions only: `Intake → AwaitingPoliceVerification → UnderReview → Approved → Registered`, with `Rejected` (terminal) and `Suspended` (resumable).
- **Computed completeness checklist** for data: `IdentityEstablished`, `LegalResidenceEstablished`, `AddressDeclared`/`AddressConfirmed`, `RegisterDeterminable` — mirroring the four core decisions from IDEA.md.

Data-recording methods update checklist flags without changing status. Guarded transitions (e.g. `Approve`) require the relevant flags.

## Consequences

- Intake order is flexible, matching reality; the officer review screen renders the checklist directly from domain state.
- Fewer statuses to test; invariants become "flag required for transition" rules.
- The checklist must be recomputed consistently — it lives on the aggregate, not in the UI.
- Revisit when: a workflow step genuinely requires strict ordering (then add a targeted guard, not more statuses).
