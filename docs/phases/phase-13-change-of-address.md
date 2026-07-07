# Phase 13 — Change of address

- **Status:** Planned
- **Goal:** Handle a **registered resident** moving to a new address **within the municipality** (or reporting arrival from another Belgian municipality in simplified form).
- **Maps to IDEA:** Reception visit reason “Change of address”; domicile update without repeating full first registration.

---

## Summary

A resident who already has a National Register number requests a domicile change. Officers verify identity, record the **new Belgian address**, update **household composition** if needed, optionally trigger a **police verification** for the new address (reuse Phase 6 loop in a lighter form), and confirm the register update.

`VisitReason.ChangeOfAddress` exists today but only as a reception label — Phase 13 delivers the workflow.

---

## Bounded context

| Context | Role |
|---------|------|
| **ChangeOfAddress** (new) | `ChangeOfAddressCase` aggregate |
| **Registration** (existing) | Read registered `Person` + prior address; no mutation of `RegistrationCase` |
| **Police** (existing) | Optional verification request for new domicile |

---

## Domain

### Aggregate: `ChangeOfAddressCase`

| Field / concept | Description |
|-----------------|-------------|
| `ChangeOfAddressCaseId` | Strongly typed identifier |
| `PersonId` | Subject already in population register |
| `Status` | `Intake` → `AwaitingPoliceVerification` → `UnderReview` → `Confirmed` / `Rejected` |
| `PreviousAddress` | Snapshot at open (from person file / last registration) |
| `NewAddress` | `BelgianAddress` value object |
| `HousingSituation` | Tenant, owner, lodged, etc. (reuse domain enums) |
| `HouseholdChanges` | Optional add/remove household members at new address |
| `PoliceVerificationRequestId` | Optional link to Phase 6 aggregate |
| `EffectiveDate` | When the change takes effect |

### Checklist

| Flag | Rule |
|------|------|
| `PersonIdentified` | NR number or identity match |
| `NewAddressDeclared` | Valid Schaerbeek address |
| `HousingDocumentAttached` | Lease or deed when policy requires |
| `PoliceVerificationPositive` | When verification requested and completed |
| `ReadyForConfirmation` | All required flags |

### Domain events

- `AddressChanged` — outbound notifications (tax, electoral roll stub).

---

## Slices

| Slice | Notes |
|-------|-------|
| `OpenChangeOfAddressCase` | NR lookup to identify person |
| `ListChangeOfAddressCases` / `GetChangeOfAddressCase` | Standard CRUD reads |
| `DeclareNewAddress` | Address + housing situation |
| `UpdateHouseholdForMove` | Simplified household delta |
| `RequestPoliceVerification` | Delegate to existing police slice |
| `ConfirmAddressChange` | Persist new domicile on `Person` / household |
| `RejectChangeOfAddress` | Invalid address or negative police |
| Claim / release lock | Same as Phase 8.1 |

---

## UI

- `/change-of-address` case list and detail wizard.
- NR lookup dialog at case open (person must already be registered).
- Reuse `AddressStep` patterns from registration where possible (shared components, not shared aggregate).
- Side-by-side **previous vs new** address on review panel.

---

## Demo

Registered EU citizen moves from Rue de la Paix to Avenue Rogier → attach rental contract → police confirms → domicile updated → certificate of residence reflects new address (Phase 8 integration).

---

## Tests

- Cannot open case for person without NR number.
- Invalid postal code rejected.
- Cannot confirm while police verification pending.
- Happy path: open → new address → confirm updates person domicile.

---

## Out of scope

- Inter-municipal move **out** of Schaerbeek (notification to other commune only as log stub).
- Homeless reference address mechanism (see Phase 18).
- FR / NL localization.

---

## Related documents

- [phase-4-address-household.md](./phase-4-address-household.md) — first-registration address model to reuse
- [phase-6-police-verification-loop.md](./phase-6-police-verification-loop.md) — police loop reuse
