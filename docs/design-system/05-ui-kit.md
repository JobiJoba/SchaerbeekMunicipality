# UI kit — wrapper component specifications

Application pages compose `App*` wrappers instead of raw MudBlazor. The wrapper layer exists to:

1. **Encode the design system once** — tokens, variants, densities, and accessibility attributes live inside the wrapper, not on every call site.
2. **Give components domain-flavored APIs** — `<AppStatusChip Status="CaseStatus.Intake" />` instead of five MudChip parameters.
3. **Make upgrades cheap** — a MudBlazor breaking change touches `DesignSystem/`, not hundreds of pages.

**When raw MudBlazor is fine:** layout primitives (`MudGrid`, `MudItem`, `MudStack`, `MudSpacer`, `MudText`, `MudDivider`, `MudTooltip`, `MudIcon`) and standard form inputs (`MudTextField`, `MudSelect`, `MudDatePicker`, …) — their theme defaults already carry the design system, and wrapping every input adds indirection without value. Everything with *policy* (elevation, structure, semantics, confirmation flows) goes through a wrapper.

Naming: `App` prefix, PascalCase, in `DesignSystem/Components/` organized by category. All wrappers forward `Class`/`Style` and use `RenderFragment` or `string` parameters for user-facing text (localization-ready, see [02-accessibility.md](./02-accessibility.md)).

Wave 1 = built in Phase 3. Wave 2 = built when the consuming phase arrives.

---

## Structure & layout (Wave 1)

### `<AppPage>`

- **Purpose:** page root — container width, padding, document title, optional breadcrumb slot. Emits the `<main id="main">` landmark targeted by the skip link.
- **API:** `Title (string, required — sets browser title "…| Schaerbeek BO")`, `Breadcrumbs (List<BreadcrumbItem>?)`, `MaxWidth (MaxWidth = ExtraLarge)`, `ChildContent`.
- **Composition:** `PageTitle` + `MudContainer MaxWidth pa-6 (pa-4 on xs)` + optional `MudBreadcrumbs`.
- **Use:** every routable page. **Don't:** nest inside another `AppPage`.

### `<AppPageHeader>`

- **Purpose:** the single `h1` + subtitle + right-aligned action cluster; the only place a page's primary action lives.
- **API:** `Title (string, required)`, `Subtitle (string?)`, `Actions (RenderFragment?)`, `StatusChip (RenderFragment?)` (case status beside the title).
- **Composition:** flex row (wraps below sm) — `MudText Typo.h1` + chip slot, `MudSpacer`, actions.
- **Accessibility:** guarantees exactly one `h1`; heading levels inside content start at `h2`.
- **Use:** directly under `AppPage` on every page. **Don't:** put more than one filled-primary button in `Actions`.

### `<AppSection>`

- **Purpose:** titled page section with standard vertical rhythm.
- **API:** `Title (string?)` (`h2`), `Description (string?)`, `ChildContent`.
- **Composition:** heading + `mb-8` block. No surface — sections group, cards contain.

### `<AppCard>`

- **Purpose:** standard content card.
- **API:** `Title (string?)` (`h3`), `Icon (string?)`, `HeaderActions (RenderFragment?)`, `Outlined (bool = false)` (true → elevation 0 + border for busy pages), `ChildContent`, `Actions (RenderFragment?)`.
- **Composition:** `MudCard Elevation=1|0` + `MudCardHeader` + `MudCardContent pa-4` + optional `MudCardActions` (right-aligned).
- **Use:** self-contained content blocks, dashboard tiles. **Don't:** wrap a full-width table (use `AppDataTable`'s own surface) or nest cards.

### `<AppDetailsPanel>` *(Wave 2)*

Right-hand 320px `MudDrawer Anchor.End Variant.Temporary` for previews/filters. API: `Open (two-way)`, `Title`, `ChildContent`.

### `<AppFooter>` *(Wave 1, part of layout)*

Blue band (`Primary` background, white caption text) with environment/version info; landmark `<footer>`. Not a page component — rendered by `MainLayout` only.

## Navigation (Wave 1)

### `<AppSideNavigation>`

- **Purpose:** the drawer nav — white surface, outlined icons, active item bold `Primary` with 3px left accent bar (translation of the site's underline-grow).
- **API:** `Items (IReadOnlyList<AppNavItem>)` where `AppNavItem(string Label, string Href, string Icon, string? RequiredRole, int? BadgeCount)`.
- **Composition:** `MudNavMenu` in `MudDrawer ClipMode.Always Elevation=0` + border; role filtering via `ICurrentOfficer`; badge via `MudBadge` (pending-work counts).
- **Accessibility:** `<nav aria-label="Main navigation">`; badge counts included in the accessible name ("Police verifications, 3 pending").

### `<AppBreadcrumb>`

Thin wrapper over `MudBreadcrumbs` (separator `›`, last item unlinked, `nav aria-label="Breadcrumb"`). API: `Items (List<BreadcrumbItem>)`. Usually supplied through `AppPage.Breadcrumbs`.

### `<AppToolbar>` / `<AppActionBar>`

- **Purpose:** dense action row above tables/filter zones (`AppToolbar`); `AppActionBar` is its sticky-bottom variant for multi-step screens.
- **API:** `ChildContent`, `Dense (bool = true)`; ActionBar adds `Sticky (bool = true)`.
- **Composition:** `MudToolBar Dense Gutters=false gap-2`; ActionBar wraps in `MudPaper Elevation=3` pinned bottom.

## Data (Wave 1)

### `<AppDataTable<T>>`

- **Purpose:** the standard work-queue table. One component = one consistent look for every list in the app.
- **API:** `Items (IEnumerable<T>?)`, `Loading (bool)` (renders `MudSkeleton` rows), `HeaderContent`, `RowTemplate (RenderFragment<T>)`, `OnRowClick (EventCallback<T>?)`, `EmptyState (RenderFragment?)` (defaults to `AppEmptyState`), `Caption (string, required)` (accessible table name), `AllowPaging (bool = true)`, `PageSize (int = 25)`.
- **Composition:** `MudTable<T> Dense Hover Striped Elevation=0 Outlined Breakpoint.Sm` + `MudTablePager` + loading/empty branches. Header cells `subtitle2 TextSecondary`, 2px `BorderStrong` bottom border (site table header).
- **Accessibility:** caption/`aria-label` required; clickable rows get `cursor:pointer` *and* a real link or button in the row (row click is an enhancement, never the only path).
- **Don't:** use for <4 columns of static facts (use `AppPropertyGrid`) or when per-column filtering is needed (use `MudDataGrid` directly, documented exception).

### `<AppStatusChip>`

- **Purpose:** render domain statuses consistently; the only allowed way to show `CaseStatus` & friends.
- **API:** `Status (enum)` overloads per domain enum, or generic `Label (string) + Severity (AppSeverity)`; `Size (Size = Small)`.
- **Composition:** `MudChip Size.Small` filled with semantic background token + AA text color + leading icon. Mapping table lives in one place (`AppStatusChipMappings`): Intake → Info, AwaitingPoliceVerification → Warning, Registered → Success, Rejected → Error, Suspended → neutral.
- **Accessibility:** always text + icon + color, never color alone.

### `<AppPropertyGrid>` *(Wave 2)*

Definition list for record facts. API: `Items (IEnumerable<(string Label, string? Value)>)` or child `<AppProperty>` elements; renders 2-column responsive grid, `subtitle2` labels / `body1` values, em-dash for empty.

### `<AppStatisticCard>` / `<AppDashboardCard>` *(Wave 2)*

KPI tile: `Label`, `Value`, `Icon`, `Accent (bool)` (yellow top bar), `Href`. Composition: `AppCard` + `h2` number + caption. For Phase 7 review dashboard.

### `<AppDocumentPreview>` *(Wave 2)*

Document row/tile: type icon, name, size, upload date, download & delete actions. Wraps the Phase 1 document list; delete goes through `AppConfirmDialog`.

**Interim (Phase 4.1):** case detail uses feature-local `RegistrationCaseDocumentPanel` + `DocumentPreviewContent` until this wrapper ships.

## Search & filtering (Wave 1: SearchBar; Wave 2: FilterPanel)

### `<AppSearchBar>`

- **Purpose:** debounced free-text search above tables.
- **API:** `Value (string, two-way)`, `Placeholder (string = "Search…")`, `DebounceMs (int = 300)`, `OnSearch (EventCallback<string>)`.
- **Composition:** `MudTextField` outlined dense, start search adornment, `Clearable`, `Immediate` + debounce.
- **Accessibility:** `aria-label` from placeholder; announces result count via the page's live region.

### `<AppFilterPanel>` *(Wave 2)*

Active-filter chip row (deletable chips) + "Filters" button opening `AppDetailsPanel` with the filter form. API: `Filters (IReadOnlyList<AppFilter>)`, `OnChanged`.

## Forms (Wave 1)

### `<AppFormSection>`

- **Purpose:** titled group inside a form — the unit of form organization (Identity, Contact, Documents).
- **API:** `Title (string)` (`h5`), `Description (string?)`, `Columns (int = 2)`, `ChildContent`.
- **Composition:** heading + `MudGrid Spacing=4`; children placed in `MudItem xs=12 md=6` by convention (full-width fields set `xs=12` explicitly).
- **Use:** every form longer than three fields.

### `<AppFieldLabel>`

Standalone label for composite widgets (address block, upload zone) that lack a Mud `Label`. API: `Text`, `Required (bool)`, `For (string?)`. Renders `subtitle2` + red asterisk with `aria-hidden` and "(required)" for screen readers.

### `<AppSaveCancelBar>`

- **Purpose:** uniform form conclusion — primary save, text cancel, dirty-state hint.
- **API:** `SaveText (string = "Save")`, `CancelText (string = "Cancel")`, `OnSave`, `OnCancel`, `Saving (bool)` (spinner-in-button + disables), `Disabled (bool)`, `Sticky (bool = false)` (uses `AppActionBar` for long forms).
- **Composition:** right-aligned `MudStack Row gap-2`: outlined cancel then filled primary save (48px).
- **Accessibility:** save button keeps its label while loading (`aria-busy="true"`), spinner is `aria-hidden`.

### `<AppForm>` *(explicitly not built)*

A generic form wrapper was considered and rejected: `MudForm` + FluentValidation adapter is already the project convention (`ARCHITECTURE.md`), and hiding `@ref`/validation wiring behind another layer obscures the mechanics this educational project wants visible. Standard usage is documented in [06-page-templates.md](./06-page-templates.md) instead.

## Feedback (Wave 1)

### `<AppAlert>`

- **Purpose:** inline message box in the site's bordered style (red-on-pink, etc.).
- **API:** `Severity (Severity)`, `Title (string?)`, `Dismissible (bool = false)`, `ChildContent`.
- **Composition:** `MudAlert Dense Variant.Text` with token background/border/text per severity, outlined icon.
- **Use:** contextual page/form messages, validation summaries (`role="alert"`). **Don't:** use for transient operation results — that's the snackbar.

### `<AppEmptyState>`

- **Purpose:** friendly zero-data placeholder.
- **API:** `Icon (string = Outlined.Inbox)`, `Title (string, required)`, `Description (string?)`, `Action (RenderFragment?)`.
- **Composition:** centered column `pa-16`: 64px icon in `TextMuted`, `h3`, `body2`, optional primary button.

### `<AppLoading>`

- **Purpose:** consistent loading placeholder.
- **API:** `Kind (Spinner|Skeleton)`, `SkeletonRows (int = 5)`, `Label (string = "Loading…")`.
- **Composition:** `MudProgressCircular Color.Primary` or `MudSkeleton` rows; `role="status"` + visually hidden label.

### `<AppError>` *(Wave 2)*

Failed-load state with retry: `Message`, `OnRetry`. Error icon + message + outlined retry button. Distinct from validation errors.

### `<AppInfoBox>`

Compact hint box (`InfoBackground` fill, info icon, `body2`) for procedural guidance inside forms — the back-office cousin of the site's content callouts. API: `ChildContent`, `Icon (string?)`.

## Dialogs (Wave 1)

### `<AppConfirmDialog>`

- **Purpose:** the only confirmation pattern. Injectable service wrapper: `IAppDialogService.ConfirmAsync(...)`.
- **API (service):** `ConfirmAsync(string title, string message, string confirmText, bool destructive = false, string? consequence = null)` → `Task<bool>`.
- **Composition:** `MudDialog MaxWidth.ExtraSmall`, `h4` title, message + optional consequence list, outlined cancel + filled confirm (`Color.Error` when destructive). Confirm label is the verb ("Reject case"), never "OK/Yes".
- **Accessibility:** focus lands on cancel for destructive dialogs; Escape cancels; focus returns to trigger.

### Dialog conventions (raw `IDialogService`)

Feature dialogs (e.g. `NewCaseDialog`) keep using `IDialogService` with standard options exposed as `AppDialogOptions.Small/Medium/Large` (`MaxWidth` presets, `CloseButton = true`, `BackdropClick = false` for forms).

## Top navigation *(Wave 1, layout-owned)*

`<AppTopNavigation>` = the app bar cluster: menu toggle, product title ("Schaerbeek — Population Department"), spacer, officer identity, `RoleSwitcher`. Rendered only by `MainLayout`; not for page use.

## Deferred (build when a phase needs them)

| Wrapper | Trigger phase |
|---------|---------------|
| `AppWizard` (stepper form shell) | Phase 4 (address & household wizard) |
| `AppTimeline` (case audit trail) | Phase 7 |
| `AppQuickActions` (dashboard shortcut grid) | Phase 7 |
| `AppPersonCard` / citizen summary header | Phase 5 (NR search results) |
| `AppAppointmentCard` | Phase 11 |
| `AppPageTitle`, `AppContentContainer` | Not planned — covered by `AppPage`/`AppPageHeader` |
