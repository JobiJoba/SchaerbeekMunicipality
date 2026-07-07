# Phase 15 — Reporting dashboard

- **Status:** Planned
- **Goal:** Municipality **operational analytics** — case volumes, processing times, and backlog health across registration, birth declaration, and change-of-address workflows.
- **Maps to IDEA:** Management view of population office workload (educational simulation).

---

## Summary

Extend the officer experience with a **reporting area** separate from the actionable review dashboard (Phase 7). Read-only queries aggregate data from existing tables — no new domain aggregates except optional materialized views later.

Phase 7’s `GetReviewDashboard` remains the **officer queue**; Phase 15 is **historical / KPI analytics**.

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
| `GetMunicipalityReportSummary` | KPI tiles for selected period |
| `GetRegistrationVolumeSeries` | Monthly time series (chart data) |
| `GetPoliceWaitTimeStats` | Average, median, p95 (simplified: average only for MVP) |
| `GetCaseOutcomeBreakdown` | Registered / rejected / suspended counts |

All handlers are **read-only**; no domain mutations.

---

## UI

- `/admin/reports` or `/registration/reports` — `AppStatisticCard` grid + `MudChart` line/bar charts.
- Date range filter (last 3 / 6 / 12 months).
- Role gate: population officer + admin demo roles.

Design-system: statistic cards and dashboard templates from [design-system/06-page-templates.md](../design-system/06-page-templates.md).

---

## Infrastructure

- EF queries with `AsNoTracking`; consider raw SQL or grouped queries for performance if datasets grow.
- No migration required for MVP (query existing tables).
- Optional later: `report_snapshots` table for nightly rollups.

---

## Demo

Officer opens reports → sees 12 registrations and 3 birth declarations this month → average police wait 2.3 days → chart shows upward trend in intake volume.

---

## Tests

- Integration: seed cases with known dates → assert KPI counts.
- Empty period returns zeroes, not errors.

---

## Out of scope

- Export to Excel / PDF.
- Real-time streaming dashboards.
- FR / NL localization.

---

## Dependencies

- **Phase 12+** recommended so birth and COA metrics are meaningful; can ship earlier with registration-only metrics.

---

## Related documents

- [phase-7-decision-registration.md](./phase-7-decision-registration.md) — review dashboard vs reporting distinction
