# Architecture, guidelines & conventions

How the design system lives in the codebase, how developers use it, and how it evolves.

## Folder structure

The design system is UI infrastructure, not a feature — it lives beside `Features/` and `Municipal/`, never inside a slice:

```
src/SchaerbeekMunicipality.Web/
├── DesignSystem/
│   ├── Theme/
│   │   └── SchaerbeekTheme.cs            # MudTheme (palettes, typography, layout, shadows)
│   ├── Tokens/
│   │   └── SchaerbeekTokens.cs           # SchaerbeekColors, -Spacing, -Layout, -Motion, -Elevation, -Typography
│   ├── Components/
│   │   ├── Layout/                       # AppPage, AppPageHeader, AppSection, AppCard, AppCollapsibleSection, AppEditableSection
│   │   ├── Navigation/                   # AppSideNavigation, AppBreadcrumb, AppTopNavigation, AppFooter
│   │   ├── Data/                         # AppDataTable, AppStatusChip, AppChecklist, AppFilePreview, AppTimeline
│   │   ├── Forms/                        # AppFormSection, AppFieldLabel, AppSaveCancelBar, AppSearchBar, AppDateField
│   │   ├── Feedback/                     # AppAlert, AppEmptyState, AppLoading, AppError, AppInfoBox
│   │   └── Dialogs/                      # AppConfirmDialog, AppDialogOptions, IAppDialogService
│   └── _Imports.razor                    # @using MudBlazor, tokens
├── Municipal/                            # Cross-feature Belgian municipal UI — see docs/MUNICIPAL-UI.md
├── Validation/                           # BelgianAddressRules, FluentMudValidation
├── Components/
│   ├── Layout/                           # MainLayout (composes AppTopNavigation/AppSideNavigation/AppFooter)
│   └── Pages/                            # Home, Error, NotFound, DesignSystemShowcase (dev-only)
└── Features/                             # vertical slices — pages compose DesignSystem + Municipal wrappers
```

Rules:

- `DesignSystem/` has **no references to `Features/` or the domain**, with one pragmatic exception: `AppStatusChip` mappings may reference domain enums (it's the presentation dictionary for them). If that ever feels wrong, split mappings into a `DesignSystem.Domain/` adapter folder.
- Slices reference the design system and the municipal layer, never each other's components.
- If a component is used by two or more slices **and carries municipal domain meaning** (NR search, Belgian address, officer checklist), it moves to `Municipal/` — not `DesignSystem/`. See [MUNICIPAL-UI.md](../MUNICIPAL-UI.md).
- If a component is used by two or more slices **and is generic** (no domain vocabulary), it moves *up* into `DesignSystem/Components/` (and gains the `App` prefix).

## Naming conventions

| Thing | Convention | Example |
|-------|-----------|---------|
| Wrapper components | `App` + role, PascalCase | `AppStatusChip.razor` |
| Token classes | `Schaerbeek` + category | `SchaerbeekColors` |
| Token members | semantic, not literal | `TextMuted`, never `Gray700` |
| Wrapper parameters | MudBlazor vocabulary where equivalent | `Dense`, `Elevation`, `ChildContent` |
| Enums for wrappers | `App` prefix | `AppSeverity` |
| CSS classes (rare) | `app-` prefix, kebab-case | `app-footer`, `skip-link` |
| Showcase page route | `/design-system` (Development only) | — |

## Building a new page

1. Start from the scaffold:

```razor
@page "/some/route"

<AppPage Title="Page name">
    <AppPageHeader Title="Page name" Subtitle="What this page is for.">
        <Actions>@* one primary action, if any *@</Actions>
    </AppPageHeader>

    <AppSection>
        @* content: AppCard / AppDataTable / form template *@
    </AppSection>
</AppPage>
```

2. Pick the matching [page template](./06-page-templates.md) and compose wrappers per its recipe.
3. Use raw MudBlazor only for layout primitives and form inputs ([the allowed list](./05-ui-kit.md)).
4. Spacing via utility classes on the 4px grid (`pa-4`, `mb-8`, `gap-2`) — no inline `style` margins/paddings.
5. Colors via `Color.*` enum or tokens — a hex literal in a `.razor` file fails review.
6. Verify keyboard-only + check the focus ring on everything interactive.
7. If you need a component that doesn't exist: build it *in the slice* first; promote to `DesignSystem/` when a second slice needs it (with spec added to [05-ui-kit.md](./05-ui-kit.md)).

## Do's and don'ts

**Do**

- One `h1` per page (via `AppPageHeader`); heading levels in order.
- One filled-primary button per view; everything else outlined/text.
- Status through `AppStatusChip`; confirmations through `AppConfirmDialog`; empty data through `AppEmptyState`.
- `Dense` tables/lists; 25 rows default paging.
- Snackbar for operation results; `AppAlert` for persistent context.
- dd/MM/yyyy dates; sentence-case labels and buttons.

**Don't**

- Don't use elevation above 3, or shadow + border on the same surface.
- Don't use turquoise `#08B0A0` or yellow `#FDC300` for text, or any color as the sole carrier of meaning.
- Don't remove focus outlines, use positive `tabindex`, or ship icon-only buttons without `aria-label` + tooltip.
- Don't write custom CSS for something a theme property, utility class, or wrapper parameter can do; don't add `<style>` blocks to pages (component-scoped `.razor.css` is allowed for genuinely local layout).
- Don't hard-code user-facing strings inside `DesignSystem/` components.
- Don't introduce a second pattern for a solved problem — extend the existing wrapper instead.

## Code conventions

- Wrappers are `.razor` files with `@code` blocks (small) — no code-behind unless the class exceeds ~100 lines.
- Every wrapper: XML-doc summary on the class-level `@code` members, `[Parameter, EditorRequired]` for required parameters, `CascadingTypeParameter` where generic.
- Forward `Class` and `Style` with `string?` defaults and merge with internal classes (`CssBuilder` from MudBlazor.Utilities).
- bUnit test per wrapper: renders, forwards parameters, applies defaults, emits accessibility attributes.
- The showcase page (`/design-system`) must show every wrapper in every documented state — it is the living contract and the manual regression surface.

## Governance & evolution

- Changing a **token value** = one-line change + contrast tests green → normal PR.
- Adding a **wrapper** = spec entry in 05 + showcase entry + bUnit test in the same PR.
- Changing a **pattern** (e.g. new dialog convention) = update this documentation set in the same PR; the docs are part of the definition of done.
- MudBlazor upgrades: run the showcase page and the bUnit suite first; theme/wrapper layer is designed to absorb the churn.

## Future recommendations

1. **CSS isolation audit** — as pages migrate, delete leftover per-page styles (`RegistrationCaseList` header CSS, etc.); target: `app.css` + theme covers everything global.
2. **Playwright + axe-core** on `/design-system` and the two main workflows — E2E infrastructure is in place; accessibility scans are the next increment.
3. **FR/NL localization** (Phase 11) — the kit is string-free by design; introduce `IStringLocalizer` at the page level and localized `DisplayName` mappings for status chips.
4. **Dark-mode toggle** — `PaletteDark` ships with the theme; expose a `MudThemeProvider @bind-IsDarkMode` toggle in the app bar when officers ask.
5. **Density switch** — if certificate/citizen-facing screens appear (Phase 8), consider a `Comfortable` density context for those pages instead of new components.
6. **Visual regression** — screenshot tests of the showcase page (Playwright) catch theme drift cheaper than eyeballing hundreds of pages.
7. **Design-token export** — if a second front end ever appears (public portal), extract tokens to a JSON source-of-truth generating both C# and CSS custom properties. Not before.
