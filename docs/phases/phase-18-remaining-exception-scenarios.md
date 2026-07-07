# Phase 18 — Remaining exception scenarios

- **Status:** Planned (optional depth)
- **Goal:** Implement the **low-priority exception slices** deferred from Phase 9 — diplomat register rules and homeless reference address.
- **Maps to IDEA:** Major exceptions for special legal statuses and domicile without fixed abode.

---

## Summary

Phase 9 delivered high- and medium-priority exception handling on the first-registration path. Two slices were explicitly **deferred**:

| Scenario | Phase 9 status |
|----------|----------------|
| Diplomat separate rules | Deferred |
| Homeless reference address | Deferred |

Phase 18 adds domain rules, checklist branches, UI alerts, and tests for these edge cases without restructuring the Phase 9 evaluator.

---

## Slice 18.1 — Diplomat register rules

### Domain

- `ResidenceCategory.Diplomat` or dedicated flag on residence step.
- `DiplomatPolicy`: lighter document set; `RegisterTarget.SpecialRegister` or dedicated convention.
- Visa / carte diplomatique document type.
- Blocks standard police verification or uses waived path (configurable demo rule).

### UI

- Residence category picker includes diplomat path.
- `AppAlert` explaining special register and federal notification stub.

### Tests

- Diplomat case approves without residence permit.
- Suggested register target is special register.

---

## Slice 18.2 — Homeless reference address

### Domain

- `AddressDeclarationType`: `Domicile` vs `ReferenceAddress` (no fixed abode).
- Reference address: municipality-hosted address per Belgian practice (simplified).
- Checklist: `AddressDeclared` satisfied by reference mechanism.
- Police verification may still apply to reference site.

### UI

- Address step toggle: “No fixed abode — use reference address”.
- Pre-filled Schaerbeek reference address from seed data.

### Tests

- Reference address completes address checklist without street number.
- Change-of-address (Phase 13) later: transition from reference to domicile.

---

## Out of scope

- Full Vienna Convention diplomatic protocol.
- Social housing referral workflows.
- FR / NL localization.

---

## Dependencies

- Phase 9 `RegistrationExceptionEvaluator` extension points.
- Phase 13 benefits from homeless reference for COA follow-up (optional ordering).

---

## Related documents

- [phase-9-exception-scenarios.md](./phase-9-exception-scenarios.md) — evaluator architecture to extend
