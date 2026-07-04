# Set residence category

Classifies why the person is in Belgium and runs the residence policy for that category.

| | |
|---|---|
| **Handler** | `SetResidenceCategoryHandler` |
| **Route** | `POST /api/registration/cases/{id}/residence-category` |
| **Blazor** | `ResidenceStep.razor` |
| **Request** | `SetResidenceCategoryRequest(ResidenceCategory)` |

## Domain rules

- Requires case status `Intake` and `IdentityEstablished`.
- Sets `RegistrationCase.ResidenceCategory`.
- Evaluates policy via `ResidencePolicyEvaluator` → updates `LegalResidenceEstablished`.
- EU citizens pass immediately; non-EU worker and student categories require a permit before the flag is set.

## Categories

| Value | Policy | Permit required |
|-------|--------|-----------------|
| `EuCitizen` | `EuCitizenPolicy` | No |
| `NonEuWorker` | `NonEuWorkerPolicy` | A or B card |
| `Student` | `StudentPolicy` | Annex 15 or B card |
