# Municipal UI layer

Cross-feature Blazor components and presentation helpers for Belgian municipal desk workflows. This layer sits **between** the generic design system and feature-specific slices.

## Why a separate layer?

The Web project has three UI tiers:

| Layer | Location | Purpose |
|-------|----------|---------|
| **Design system** | `Web/DesignSystem/` | Generic `App*` wrappers — layout, tables, dialogs, tokens. No domain vocabulary. |
| **Municipal UI** | `Web/Municipal/` | Reusable Belgian municipal patterns shared by two or more bounded contexts. |
| **Feature slices** | `Web/Features/{Context}/` | Context-specific pages, thin dialog wrappers, and handlers. |

Before this layer existed, Registration, Birth declaration, and Change of address duplicated the same address fields, NR search form, document panel, officer checklist, and case-lock chrome — or reached across slice boundaries (`BirthDeclaration` referencing `Registration` components).

**Promotion rule:** when a UI pattern is needed in **two or more bounded contexts** and carries **municipal domain meaning** (NR numbers, Belgian addresses, officer decision checklists), extract it to `Municipal/`. Generic patterns with no domain vocabulary belong in `DesignSystem/` instead.

See also [design-system/07-architecture-and-guidelines.md](./design-system/07-architecture-and-guidelines.md) and [ARCHITECTURE.md](./ARCHITECTURE.md#municipal-ui-layer).

## Folder structure

```
src/SchaerbeekMunicipality.Web/
├── Municipal/
│   ├── Components/
│   │   ├── BelgianAddressFields.razor       # Street autocomplete + house number + box
│   │   ├── CaseDocumentPanel.razor          # Upload slot, document list, inline preview
│   │   ├── CaseDocumentItem.cs              # Document row DTO
│   │   ├── CaseLockActions.razor            # Take case / release lock buttons
│   │   ├── CaseLockBar.razor                # Read-only warning when case locked elsewhere
│   │   ├── NationalRegisterSearchForm.razor # Name + DOB search + results table
│   │   ├── NationalRegisterSearchModels.cs  # Search criteria and match DTOs
│   │   └── OfficerDecisionChecklist.razor   # Four core questions checklist display
│   ├── AddressFormatting.cs                 # Belgian address display strings
│   ├── NationalRegisterFormatting.cs        # NR/BIS number display (YYMMDD-XXX.XX)
│   └── _Imports.razor
└── Validation/
    ├── BelgianAddressRules.cs               # FluentValidation extensions for Belgian addresses
    └── FluentMudValidation.cs               # Bridge FluentValidation → MudForm field validation
```

**Naming:** Municipal components do **not** use the `App` prefix — that is reserved for design-system wrappers. They use plain descriptive names (`BelgianAddressFields`, not `MunicipalAddressFields`).

**Dependencies:**

- Municipal components **may** reference `DesignSystem/` and domain DTOs from handlers.
- Municipal components **must not** reference `Features/` or inject feature handlers directly.
- Feature slices pass data and callbacks **in** via parameters (`SearchAsync`, `GetDocumentUrl`, `OnRemoveDocument`, …).

## Components

### `BelgianAddressFields`

Standard domicile input block: fixed municipality (defaults to Schaerbeek `1030`), street autocomplete from the local register, house number, and optional box.

| Parameter | Purpose |
|-----------|---------|
| `SearchStreets` | `Func<string, CancellationToken, Task<IEnumerable<StreetDto>>>` — wired to `SearchReferenceDataHandler` at the call site |
| `SelectedStreet` / `HouseNumber` / `Box` | Two-way bound address parts |
| `PostalCode` / `MunicipalityName` | Override when a desk accepts addresses outside Schaerbeek (e.g. change of address) |
| `TrailingContent` | Slot for extra fields (effective date, housing situation, …) |
| `ShowHelpText` | Shows the Schaerbeek domicile info box |

**Used by:** `AddressStep.razor` (registration), `ChangeOfAddressCaseDetail.razor`.

Pair with `BelgianAddressRules` in the feature validator and `FluentMudValidation.ToMudValidateValue` on the enclosing `MudForm`.

### `NationalRegisterSearchForm`

Self-contained NR/BIS search UI: criteria fields, search button, results table with optional match score, formatted numbers, and a `RowActions` slot per match.

| Parameter | Purpose |
|-----------|---------|
| `SearchAsync` | `Func<NrSearchFormCriteria, Task<IReadOnlyList<NationalRegisterSearchMatch>>>` — feature handler at call site |
| `InitialGivenName` / `InitialFamilyName` / `InitialBirthDate` | Pre-fill from case context |
| `ShowBirthDateColumn` / `ShowMatchScore` / `ShowFormattedNumbers` | Column visibility |
| `RowActions` | Per-row buttons ("Link", "Open case", …) |
| `LeadContent` | Slot above criteria (e.g. explanatory text) |

**Used by:** `NationalRegisterSearchDialog.razor`, `ParentLinkDialog.razor`, `OpenChangeOfAddressDialog.razor`.

Feature dialogs stay thin: they supply the handler callback and row actions, then close on success.

### `CaseDocumentPanel`

Sticky side-panel pattern for case documents: optional upload slot, dense document list, click-to-select preview (`AppFilePreview`), download link, and per-row delete when `CanEdit`.

| Parameter | Purpose |
|-----------|---------|
| `Documents` | `IReadOnlyList<CaseDocumentItem>` |
| `GetDocumentUrl` | Maps document id → download/preview URL |
| `UploadContent` | Render fragment for the upload form |
| `OnRemoveDocument` | Delete callback |
| `CanEdit` | Shows delete buttons |

**Used by:** `RegistrationCaseDocumentPanel.razor`, `BirthDeclarationDocumentPanel.razor` — each maps its context-specific DTOs to `CaseDocumentItem` and wires upload/remove handlers.

### `OfficerDecisionChecklist`

Renders the four core registration questions (or equivalent) via `AppChecklist`. Maps domain checklist DTOs to `AppChecklistItem` at the feature layer.

**Used by:** `CaseDecisionSection.razor`, `BirthDeclarationDecisionSection.razor`, `ChangeOfAddressDecisionSection.razor`.

### `CaseLockBar` / `CaseLockActions`

Officer case-lock UX from Phase 8.1:

- **`CaseLockBar`** — warning alert when the case is read-only because another officer holds the lock.
- **`CaseLockActions`** — "Take case" / "Release lock" buttons for the page header action cluster.

**Used by:** `RegistrationCaseDetail.razor`, `BirthDeclarationCaseDetail.razor`, `ChangeOfAddressCaseDetail.razor`.

## Formatting helpers

### `AddressFormatting`

```csharp
AddressFormatting.FormatBelgianAddress(street, houseNumber, box, postalCode, municipality);
AddressFormatting.FormatBelgianAddress(belgianAddressDto);  // null-safe
```

Produces display strings like `Rue du Progrès 12 bus 3, 1030 Schaerbeek`.

### `NationalRegisterFormatting`

```csharp
NationalRegisterFormatting.FormatNumber("85073012345");  // → "850730-123.45"
```

Leaves non-11-digit values unchanged (partial BIS, display fallbacks).

## Validation helpers (`Web/Validation/`)

Shared FluentValidation extensions and MudBlazor integration live beside Municipal UI — they encode Belgian field rules used across slices.

### `BelgianAddressRules`

| Extension | Rule |
|-----------|------|
| `BelgianStreet()` | Required, max 256 |
| `BelgianHouseNumber()` | Required, max 16 |
| `BelgianPostalCode()` | Exactly 4 digits |
| `BelgianMunicipality()` | Required, max 128 |
| `SchaerbeekPostalCode()` | Must equal `1030` |
| `SchaerbeekMunicipality()` | Must equal Schaerbeek (case-insensitive) |

**Used by:** `DeclareAddressValidator`, `DeclareNewAddressValidator`, `SetDeclarationHouseholdRequest` validator, and others.

### `FluentMudValidation`

Bridges slice validators to per-field MudForm validation:

```csharp
Validation="@(DeclareAddressValidator.ToMudValidateValue<DeclareAddressRequest, AddressFormModel>(MapAddressRequest))"
```

The form model stays in the feature slice; the validator remains the single source of truth; MudBlazor shows inline errors as the officer types.

## Adding a new Municipal component

1. Confirm at least **two bounded contexts** need the same pattern and it carries **municipal domain meaning**.
2. Build in `Municipal/Components/` with **parameter-driven** callbacks — no handler injection.
3. Add `@using SchaerbeekMunicipality.Web.Municipal.Components` to the feature `_Imports.razor` (or rely on the root imports).
4. Replace duplicated markup in each slice with the shared component.
5. Document the component in this file.

If the pattern is generic (no Belgian/NR/officer vocabulary), promote to `DesignSystem/` with an `App` prefix instead.

## Related documents

- [ARCHITECTURE.md](./ARCHITECTURE.md) — vertical slices, Blazor conventions, handler responsibilities
- [design-system/05-ui-kit.md](./design-system/05-ui-kit.md) — `App*` wrapper specifications
- [design-system/07-architecture-and-guidelines.md](./design-system/07-architecture-and-guidelines.md) — three-layer UI model and promotion rules
- [phase-8.1-role-boundaries-case-locking.md](./phases/phase-8.1-role-boundaries-case-locking.md) — case lock behaviour
- [phase-5-national-register-search-bis.md](./phases/phase-5-national-register-search-bis.md) — NR search domain rules
