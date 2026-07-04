# Testing strategy

Testing supports learning and safe refactoring as the municipality project grows. Tests should read like **specifications of municipal behavior**, not implementation details.

## Test pyramid

```
        ┌─────────────┐
        │  UI (bUnit) │  few, high-value components
        ├─────────────┤
        │ Integration │  handlers + EF + API
        ├─────────────┤
        │   Domain    │  many, fast, no I/O
        └─────────────┘
```

| Layer | Project | Speed | I/O |
|-------|---------|-------|-----|
| Domain unit | `Domain.Tests` | ms | None |
| Integration (handlers, API, EF) | `Integration.Tests` | seconds | SQLite via WebApplicationFactory |
| Migration validation | `Integration.Tests` (`Category=PostgreSQL`) | tens of seconds | Testcontainers PostgreSQL |
| Blazor component (selective) | `Integration.Tests` | seconds | bUnit |

Two test projects only — API tests and handler tests overlap heavily (both use `WebApplicationFactory`), so they live together in `Integration.Tests` rather than a separate `Web.Tests` project.

**Target:** Domain tests for every invariant; at least one integration test per vertical slice; API test for each Minimal API route.

---

## Domain tests (`SchaerbeekMunicipality.Domain.Tests`)

**Purpose:** Prove business rules without database or HTTP.

**What to test:**

- Status transitions and completeness checklist computation on `RegistrationCase`
- Value object validation (`BelgianAddress`, `NationalRegisterNumber`)
- Residence policy outcomes
- Domain events raised on significant actions

**Patterns:**

- Arrange with factory methods or builders
- Use `FluentAssertions` for readable assertions
- No mocking — pure domain

**Example scenarios:**

```csharp
// RegistrationCaseTests.cs
[Fact]
public void Approve_WhenPoliceResultNegative_Throws()
{
    var registrationCase = RegistrationCaseBuilder.UnderReview().WithNegativePoliceResult().Build();
    var act = () => registrationCase.Approve(OfficerId.New(), RegisterTarget.ForeignersRegister);
    act.Should().Throw<InvalidRegistrationTransitionException>();
}

[Fact]
public void RecordIdentity_DuringIntake_MarksIdentityEstablished()
{
    var registrationCase = RegistrationCase.Open(...);
    registrationCase.RecordIdentity(...);
    registrationCase.Checklist.IdentityEstablished.Should().BeTrue();
    registrationCase.Status.Should().Be(RegistrationCaseStatus.Intake); // data collection does not advance status
}
```

**Test data builders:**

Place in `Domain.Tests/Builders/` — keep construction DRY without polluting production code.

---

## Integration tests (`SchaerbeekMunicipality.Integration.Tests`)

**Purpose:** Verify handlers, EF Core mappings, and repositories together.

**Setup:**

- `WebApplicationFactory` hosts `Web` directly — **not** via AppHost (keeps the default suite fast and Docker-free).
- SQLite `:memory:` with `EnsureCreated()` — schema created from the EF model, **not** from migrations.
- Replace `IDocumentStorage` with temp folder implementation.
- Fixed `TimeProvider` for deterministic timestamps.

**Honest limitation:** migrations are generated for PostgreSQL (snake_case naming, Npgsql column types) and generally cannot run on SQLite — SQLite also lacks several migration operations such as most `ALTER COLUMN`. The SQLite suite therefore validates the **model and behavior**, not the **migrations**. Migrations are validated by the Testcontainers PostgreSQL job below.

**What to test:**

- Each handler happy path persists expected state
- Queries return correct projections
- Unique constraints and duplicate detection

**Example:**

```csharp
[Fact]
public async Task OpenRegistrationCase_PersistsIntakeCase()
{
    await using var factory = new MunicipalAppFactory();
    var handler = factory.Services.GetRequiredService<OpenRegistrationCaseHandler>();

    var result = await handler.Handle(new OpenRegistrationCaseRequest { ... }, CancellationToken.None);

    var repo = factory.Services.GetRequiredService<IRegistrationCaseRepository>();
    var registrationCase = await repo.GetByIdAsync(result.CaseId, CancellationToken.None);
    registrationCase!.Status.Should().Be(RegistrationCaseStatus.Intake);
}
```

**Database isolation:**

- Option A: new SQLite file per test (simple, slower)
- Option B: transaction rollback per test (fast, requires setup)
- Option C: `:memory:` with shared connection per test class

Start with Option A; optimize when suite grows.

**PostgreSQL migration validation (from Phase 1, once the first real migration exists):**

- Testcontainers starts PostgreSQL and applies **all migrations** — the only place migrations are exercised before deployment.
- Tag with `[Trait("Category", "PostgreSQL")]`; runs as a separate CI job off the PR critical path (see [CI.md](./CI.md)).
- Same `MunicipalAppFactory` with a PostgreSQL connection string; the primary suite stays on SQLite.

**Aspire:**

- Do **not** start AppHost in unit or integration tests.
- Aspire is for local dev and deployment manifests; tests prove application logic independently.

---

## API tests (in `Integration.Tests`)

**Purpose:** HTTP contract and Minimal API wiring.

**Tool:** `Microsoft.AspNetCore.Mvc.Testing`

```csharp
[Fact]
public async Task Post_Cases_Returns201()
{
    var client = _factory.CreateClient();
    var response = await client.PostAsJsonAsync("/api/registration/cases", new { ... });
    response.StatusCode.Should().Be(HttpStatusCode.Created);
}
```

**Also test:**

- Validation failures → 400 ProblemDetails
- Not found → 404
- Domain conflict → 409 with problem type

---

## Blazor component tests (selective)

**Tool:** bUnit + FluentAssertions

**Test when:**

- Complex wizard step with conditional fields
- Review checklist component computing flags from case state

**Skip when:**

- Thin pages that only call a handler — integration tests cover those

**Example focus:**

- `IdentityStep` shows marriage fields when civil status is Married
- `CaseReviewChecklist` shows red flag when police result missing

---

## Naming conventions

```
{UnitUnderTest}_{Scenario}_{ExpectedOutcome}
```

Examples:

- `RegistrationCase_Approve_WithoutPoliceResult_Throws`
- `OpenRegistrationCaseHandler_MissingVisitReason_FailsValidation`
- `PostCases_ValidRequest_ReturnsCreated`

File naming mirrors production:

```
Domain.Tests/Registration/RegistrationCaseTests.cs
Integration.Tests/Features/Registration/OpenRegistrationCaseTests.cs
```

---

## Test doubles

| Dependency | Approach |
|------------|----------|
| `TimeProvider` | Fixed instant in tests |
| `ICurrentOfficer` | Test officer with known roles |
| `IDocumentStorage` | Temp directory |
| Immigration Office | Stub returning canned decisions |
| National Register | Seeded in-memory data |
| Police | Record results via handler in same test |

Prefer **real SQLite** over mocking `DbContext` for integration tests.

---

## Coverage expectations

No arbitrary percentage target. Instead:

| Area | Expectation |
|------|-------------|
| Domain aggregates | Every public command method has success + key failure tests |
| Handlers | One integration test minimum per slice |
| API endpoints | Status codes for success + validation error |
| UI | Critical conditional forms only |

Use coverage tools optionally to find untested domain branches.

---

## Running tests

```bash
# All tests
dotnet test

# Domain only (fast feedback)
dotnet test tests/SchaerbeekMunicipality.Domain.Tests

# Watch mode during development
dotnet watch test --project tests/SchaerbeekMunicipality.Domain.Tests
```

---

## CI pipeline

GitHub Actions runs on every push and PR to `main`. See [CI.md](./CI.md) for the full workflow, badge setup, branch protection, and Phase 10 PostgreSQL options.

```bash
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release --verbosity normal
```

---

## Testing anti-patterns (avoid)

- Testing private methods
- Mocking EF Core for handler tests when SQLite is fast enough
- Brittle UI tests that break on MudBlazor markup changes
- Shared mutable test state across parallel tests
- Asserting exact exception message strings (assert exception type + key properties)

---

## Related documents

- [ARCHITECTURE.md](./ARCHITECTURE.md) — handler and slice layout
- [ROADMAP.md](./ROADMAP.md) — when to add tests per phase
- [CI.md](./CI.md) — GitHub Actions workflow
