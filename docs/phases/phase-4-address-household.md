# Phase 4 — Address & household

- **Status:** Complete
- **Completed:** July 2026
- **Goal:** Declare domicile, housing situation, household composition, and civil status — with correction paths from day one.

---

## Summary

Phase 4 adds the address and household slice of registration intake. Officers declare a domicile in **1030 Schaerbeek** (fixed municipality; street autocomplete from the local register), record housing situation, compose the household member list, and capture civil status with conditional marriage fields. The `AddressDeclared` checklist flag is set when an address is saved.

---

## Deliverables checklist

| Deliverable | Status | Notes |
|-------------|--------|-------|
| `DeclareAddress` slice | Done | Upsert; sets `AddressDeclared`; 1030 Schaerbeek only |
| `RecordHousingSituation` slice | Done | Requires address declared |
| `RecordHouseholdComposition` slice | Done | Upsert household + members |
| `RecordCivilStatus` slice | Done | On `Person`; marriage fields when married |
| `BelgianAddress` value object | Done | 4-digit postal code validation |
| `Household` aggregate | Done | `households` + `household_members` tables |
| Reference data seed | Done | Schaerbeek streets; neighbouring communes reserved for future flows |
| Address / household / civil status UI | Done | `AddressStep`, `HouseholdStep`, `CivilStatusStep` |
| Domain tests | Done | Postal code validation, identity gate |
| Integration tests | Done | Full address → household → civil status flows |
| EF migration `AddressAndHousehold` | Done | Address columns, household, reference tables |

---

## API routes

| Method | Route | Slice |
|--------|-------|-------|
| `POST` | `/api/registration/cases/{id}/address` | Declare address |
| `POST` | `/api/registration/cases/{id}/housing-situation` | Record housing |
| `POST` | `/api/registration/cases/{id}/household` | Record household composition |
| `POST` | `/api/registration/cases/{id}/civil-status` | Record civil status |
| `GET` | `/api/registration/streets?postalCode=1030` | Street autocomplete (Schaerbeek only at intake) |

Note: `GET /api/registration/municipalities` exists for reference data but is **not** used in the intake UI — municipality is fixed to 1030 Schaerbeek for first registration.

---

## Demo walkthrough

1. Open a case and record identity (Phase 1).
2. Under **Address & housing**, confirm municipality **1030 Schaerbeek** (read-only), pick street **Chaussée de Louvain**, enter number, save → checklist shows **Address ✓**.
3. Record **Tenant** as housing situation.
4. Under **Household composition**, add spouse and child, save.
5. Under **Civil status**, choose **Married** and fill marriage details.

---

## Tests

```bash
dotnet test --configuration Release --filter "Category!=PostgreSQL"
```

Expected: **77 tests passing** in the fast suite (30 domain + 47 integration).

---

## Related documents

- [ROADMAP.md](../ROADMAP.md)
- [phase-2.1-intake-corrections.md](./phase-2.1-intake-corrections.md)
- [phase-3-design-system.md](./phase-3-design-system.md)
- [phase-4.1-document-preview-case-ux.md](./phase-4.1-document-preview-case-ux.md)
- [DOMAIN.md](../DOMAIN.md)
