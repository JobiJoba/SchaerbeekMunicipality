# Phase 2 — Residence category & permits

- **Status:** Complete
- **Completed:** July 2026
- **Goal:** Classify legal stay and attach permit evidence; establish the `LegalResidenceEstablished` checklist flag.

---

## Summary

Phase 2 adds the immigration/residence slice of the registration workflow. Officers set a residence category (EU citizen, non-EU worker, student), record permit details when required, and optionally stub an external immigration decision. Policy classes validate evidence and drive the `LegalResidenceEstablished` checklist flag without advancing case status.

---

## Deliverables checklist

| Deliverable | Status | Notes |
|-------------|--------|-------|
| `SetResidenceCategory` slice | Done | Handler, validator, endpoint, UI |
| `RecordResidencePermit` slice | Done | Upsert permit per case |
| `RecordImmigrationDecision` slice | Done | Stub external decision reference |
| `ResidenceCategory` enum | Done | EuCitizen, NonEuWorker, Student |
| `ResidencePermit` entity | Done | `residence_permits` table |
| Policy classes | Done | `EuCitizenPolicy`, `NonEuWorkerPolicy`, `StudentPolicy` |
| `LegalResidenceEstablished` checklist | Done | Set/cleared via `ResidencePolicyEvaluator` |
| Residence step UI | Done | `ResidenceStep.razor` on case detail |
| Domain tests | Done | Policies + case residence guards |
| Integration tests | Done | EU path, non-EU permit path, identity gate |
| EF migration `ResidenceCategoryAndPermits` | Done | Case columns + permits table |

---

## API routes

| Method | Route | Slice |
|--------|-------|-------|
| `POST` | `/api/registration/cases/{id}/residence-category` | Set category |
| `POST` | `/api/registration/cases/{id}/residence-permit` | Record permit |
| `POST` | `/api/registration/cases/{id}/immigration-decision` | Record decision (stub) |

---

## Demo walkthrough

1. Open a case and record identity (Phase 1).
2. On case detail, under **Legal residence**, choose **EU citizen** → checklist shows **Legal residence ✓** immediately.
3. For a non-EU worker: choose **Non-EU worker** → checklist stays open → record a **B card** with future validity → **Legal residence ✓**.

---

## Tests

| Project | New tests | Coverage |
|---------|-----------|----------|
| `Domain.Tests` | 8 | Policies, identity gate, checklist |
| `Integration.Tests` | 6 | Handlers, API, residence flows |

```bash
dotnet test --configuration Release --filter "Category!=PostgreSQL"
```

Expected: **49 tests passing** in the fast suite (12 domain + 37 integration).

---

## Related documents

- [ROADMAP.md](../ROADMAP.md)
- [phase-1-case-intake-identity.md](./phase-1-case-intake-identity.md)
- [DOMAIN.md](../DOMAIN.md)
