# Set residence category

Classifies why the person is in Belgium and runs the residence policy for that category.

| | |
|---|---|
| **Handler** | `SetResidenceCategoryHandler` |
| **Route** | `POST /api/registration/cases/{id}/residence-category` |
| **Blazor** | `ResidenceStep.razor` |
| **Request** | `SetResidenceCategoryRequest(ResidenceCategory)` |

## Domain rules

- Requires `EnsureIntakeDataEditable()` (`Intake` or `UnderReview`) and `IdentityEstablished`.
- Sets `RegistrationCase.ResidenceCategory` (overwrites on re-submit — no “already set” guard).
- Evaluates policy via `ResidencePolicyEvaluator` → updates `LegalResidenceEstablished`.
- EU citizens pass immediately; non-EU worker and student categories require a permit before the flag is set.

## Correction path

Uses the **upsert handler** pattern (see [intake corrections](./README.md#intake-corrections-phase-21)): the same `POST` route and handler serve first record and correction. `ResidenceStep.razor` exposes an **Edit** button on the saved category card.

Changing category re-runs residence policy. If the existing permit type no longer matches the new category (e.g. non-EU worker → student with an A card), `LegalResidenceEstablished` clears until the permit is corrected — the response `PolicyMessage` drives the UI warning.

## Categories

| Value | Policy | Permit required |
|-------|--------|-----------------|
| `EuCitizen` | `EuCitizenPolicy` | No |
| `NonEuWorker` | `NonEuWorkerPolicy` | A or B card |
| `Student` | `StudentPolicy` | Annex 15 or B card |
