# Phase 15 — Reporting dashboard

- **Status:** Complete
- **Completed:** July 2026
- **Goal:** Municipality **operational analytics** — case volumes, processing times, and backlog health across registration, birth declaration, and change-of-address workflows.
- **Maps to IDEA:** Management view of population office workload (educational simulation).

---

## Summary

Extend the officer experience with a **reporting area** separate from the actionable review dashboard (Phase 7). Read-only queries aggregate data from existing tables — no new domain aggregates except optional materialized views later.

Phase 7’s `GetReviewDashboard` remains the **officer queue**; Phase 15 is **historical / KPI analytics**.

A new **`BackOfficeOfficer`** demo role can access reports only (no case lists, review dashboard, or editing). Population officers retain full operational access plus the same reports page.

---

## Metrics (initial set)

| Metric | Source |
|--------|--------|
| Registrations per month | `RegistrationCase` where `Status = Registered`, group by month |
| Birth declarations per month | `BirthDeclarationCase` (Phase 12) |
| Address changes per month | `ChangeOfAddressCase` (Phase 13) |
| Average police wait time | `PoliceVerificationRequest` created → completed |
| Cases by status | Current backlog breakdown |
| Average time intake → registered | `RegistrationCase` timestamps |
| Rejection / suspension rate | Decision counts |

---

## Slices

| Slice | Notes |
|-------|-------|
| `GetMunicipalityReport` | Single handler returning summary KPIs, volume series, backlog, and outcome breakdown for the selected period |

All handlers are **read-only**; no domain mutations.

---

## UI

- `/administration/reports` — `AppStatisticCard` grid + `MudChart` line chart.
- Date range filter (last 3 / 6 / 12 months).
- Role gate: `PopulationOfficer` + `BackOfficeOfficer`.

Design-system: statistic cards and dashboard templates from [design-system/06-page-templates.md](../design-system/06-page-templates.md).

---

## Role access

| Surface | Population | Back office | Reception | Police |
|---------|------------|-------------|-----------|--------|
| Review dashboard (queue) | Yes | No | No | No |
| Reports (analytics) | Yes | Yes | No | No |

Demo officer: **Sophie Lambert (Back office)** — lands on `/administration/reports` with Reports-only navigation.

---

## Infrastructure

- Repository `ListAsync` + in-memory aggregation for MVP scale.
- `IPoliceVerificationRepository.ListAllAsync` for police wait metrics.
- No migration required for MVP (query existing tables).
- Optional later: `report_snapshots` table for nightly rollups.

---

## Demo

1. Switch to **Sophie Lambert (Back office)** → lands on **Reports** → KPI tiles + chart load; sidebar shows Reports only.
2. Switch to **Marie Dupont (Population)** → Reports appears alongside operational nav; review dashboard still works.
3. With seeded demo data: registrations, birth declarations, and address changes appear in monthly volume; average police wait shown when verifications completed.

---

## Tests

- `MunicipalityReportTests` — role boundaries, empty database zeroes, completed registration counted, API 403 for reception.
- Empty period returns zeroes, not errors.

---

## Out of scope

- Export to Excel / PDF.
- Real-time streaming dashboards.
- FR / NL localization.

---

## Dependencies

- **Phase 12** complete — birth declaration metrics available; **Phase 13+** recommended so COA metrics are meaningful.

---

## Related documents

- [phase-7-decision-registration.md](./phase-7-decision-registration.md) — review dashboard vs reporting distinction
- [phase-8.1-role-boundaries-case-locking.md](./phase-8.1-role-boundaries-case-locking.md) — role matrix (updated with Reports row)
