# Component catalogue & MudBlazor mapping

Inventory of every reusable UI element the application needs, its source pattern on 1030.be where one exists, and the standard MudBlazor configuration. Where a wrapper component exists (or is planned), pages must use the wrapper — see [05-ui-kit.md](./05-ui-kit.md).

**Global defaults** (why: the site is flat, bordered, sober; density suits all-day data work):

- `Variant.Outlined` for inputs; `Variant.Filled` for primary buttons, `Variant.Outlined` for secondary, `Variant.Text` for tertiary.
- `Elevation="0"` plus border for in-page surfaces; elevation only per the [elevation scale](./01-design-tokens.md#4-elevation-system).
- `Dense="true"` on tables, lists, toolbars, menus.
- `Margin.Dense` + `Margin="Margin.Dense"` on form fields inside dense contexts; normal in citizen-facing certificate previews.
- Icons: `Icons.Material.Outlined.*`.

These defaults are set once via `MudGlobal` settings and wrapper components — never repeated per page.

---

## 1. Actions

| Component | MudBlazor | Standard configuration | Notes |
|-----------|-----------|------------------------|-------|
| Primary button | `MudButton` | `Variant.Filled`, `Color.Primary`, `Size.Medium` (48px), `DropShadow="false"` | One per view/section. Bold sentence-case label (theme). Site: blue fill, white bold text, hover shadow. |
| Secondary button | `MudButton` | `Variant.Outlined`, `Color.Primary` | Cancel, back, secondary paths |
| Tertiary/inline button | `MudButton` | `Variant.Text`, `Color.Primary` | Low-emphasis ("View all") — echoes site's underlined links |
| Destructive button | `MudButton` | `Variant.Filled`, `Color.Error` | Always paired with `AppConfirmDialog` |
| Icon button | `MudIconButton` | `Size.Small` in tables, `Medium` in toolbars; **`aria-label` required** + `MudTooltip` | Only for universal actions (edit, delete, download, close) |
| Loading state | `MudButton` | disable + `MudProgressCircular Size.Small` in `ChildContent` | Wrapper `AppButton` handles it via `Loading` parameter |
| FAB | `MudFab` | **Not used** | Back-office pages have toolbars; a floating button hides actions |
| Button group | `MudButtonGroup` | `Variant.Outlined`, `Color.Primary` | View switches (table/card) |

States, all buttons: hover = tonal wash + `Raised` shadow on filled (site behavior); active/pressed = opacity 0.9 (site: `.btn:active{opacity:.9}`); focus = global turquoise ring; disabled = `SurfaceSunken`/`TextDisabled`, `cursor: not-allowed`.

## 2. Surfaces & containers

| Component | MudBlazor | Standard configuration | Notes |
|-----------|-----------|------------------------|-------|
| Card | `MudCard` | `Elevation="1"`, `Outlined="false"`; or `Elevation="0" Outlined="true"` inside busy pages | Title = `h3`; content `pa-4`; actions right-aligned |
| Panel/paper | `MudPaper` | `Elevation="0"`, `Outlined="true"`, `Class="pa-4"` | Generic grouping surface |
| Section | heading + spacing conventions | `h2` + `mb-8` | Wrapper `AppSection` |
| Expansion panels | `MudExpansionPanels` | `Elevation="0"`, `Outlined="true"`, `Dense="true"` | Optional/advanced form blocks; site FAQ pattern |
| Tabs | `MudTabs` | `Elevation="0"`, `Color.Primary`, `SliderColor` accent-yellow underline | Detail-page facets (Identity / Documents / History) |
| Dialog | `MudDialog` via `IDialogService` | `MaxWidth.Small` default, `CloseButton="true"`, `BackdropClick="false"` for forms | Title `h4` + icon; actions right, primary rightmost |
| Drawer (nav) | `MudDrawer` | `ClipMode.Always`, `Elevation="0"` + right border, `Variant.Responsive`, width 260px | White background, blue active item (site nav) |
| Detail side panel | `MudDrawer Anchor.End` | `Variant.Temporary`, width 320px | Filters, quick previews |
| App bar | `MudAppBar` | `Color.Primary`, `Elevation="1"`, `Dense="false"` (64px) | Blue like site header/footer band |
| Toolbar | `MudToolBar` | `Dense="true"`, `Gutters="false"` inside cards | List page action rows |
| Footer | custom `AppFooter` | Blue band (`wrapper--blue` pattern), white text, caption size | Environment/version info |

## 3. Data display

| Component | MudBlazor | Standard configuration | Notes |
|-----------|-----------|------------------------|-------|
| Data table (default) | `MudTable<T>` | `Dense="true"`, `Hover="true"`, `Striped="true"`, `Elevation="0"`, `Outlined="true"`, `Breakpoint.Sm` | Site table style: bold header over strong border, zebra rows. Header row `subtitle2` `TextSecondary`, 2px bottom border. |
| Data grid (heavy) | `MudDataGrid<T>` | Same density; enable `SortMode.Multiple`, column filters, `Virtualize` beyond ~200 rows | Only when column filtering/grouping is actually needed — table otherwise |
| Pagination | `MudTablePager` / `MudPagination` | `PageSizeOptions="[10, 25, 50]"`, default 25 | Server-side paging from Phase 5 volumes |
| Status chip | `MudChip<string>` | `Size.Small`, filled, 12px radius, semantic color mapping | Wrapper `AppStatusChip` maps `CaseStatus` → color+icon+label; never color-only |
| Tag/badge chip | `MudChip` | `Variant.Text` + `SecondaryLighten` fill, dark text | Site's turquoise tags; decorative counts |
| Badge | `MudBadge` | `Color.Error` dot/count | Pending-work counters on nav items |
| List | `MudList<T>` | `Dense="true"` | Document lists, checklist display |
| Timeline | `MudTimeline` | `TimelinePosition.Start`, dots in semantic colors | Case history/audit (Phase 7) |
| Avatar | `MudAvatar` | `Color.Primary`, initials, `Size.Medium` | Officer identity; citizens get neutral icon avatar |
| Tooltip | `MudTooltip` | `Delay="300"`, `Arrow="true"` | Required on all icon-only buttons |
| Property/definition list | custom `AppPropertyGrid` | 2-col grid: `subtitle2` label / `body1` value | Citizen record facts |
| Statistic card | custom `AppStatisticCard` | `MudCard` + big number (`h2`) + caption + optional accent bar | Dashboards (Phase 7+); yellow accent bar for highlights |

## 4. Inputs & forms

All inputs: `Variant.Outlined`, visible `Label`, `HelperText` where non-obvious, `Margin.Dense` in dense layouts, immediate validation display via FluentValidation adapter.

| Component | MudBlazor | Standard configuration | Notes |
|-----------|-----------|------------------------|-------|
| Text field | `MudTextField<string>` | above + `Clearable="true"` for search-ish fields | |
| Number | `MudNumericField<T>` | `HideSpinButtons="true"` for identifiers | National numbers are **text** (leading zeros) |
| Select | `MudSelect<T>` | `AnchorOrigin.BottomCenter`, dense items | ≤ ~10 options; more → autocomplete |
| Autocomplete | `MudAutocomplete<T>` | `ResetValueOnEmptyText`, `CoerceText`, min 2 chars, `DebounceInterval="300"` | Streets, nationalities, municipalities |
| Date picker | `MudDatePicker` | `Editable="true"`, `DateFormat="dd/MM/yyyy"`, `PickerVariant.Dialog` on mobile | Belgian format everywhere |
| Time picker | `MudTimePicker` | `AmPm="false"` | Appointments |
| Checkbox | `MudCheckBox<bool>` | `Color.Primary`, `Dense="true"` | Checklist confirmations |
| Radio group | `MudRadioGroup<T>` | vertical stack, `Dense` | 2–5 mutually exclusive options (visit reason) |
| Switch | `MudSwitch<bool>` | `Color.Primary` | Immediate-effect toggles only (filters), never form consent |
| File upload | `MudFileUpload<T>` | drag-drop area + `SecondaryLighten` hover ring, list of files below | Wraps existing `DocumentUpload`; show size/type constraints up front |
| Form container | `MudForm` | `@ref` + FluentValidation adapter; `spellcheck="false"` on identifier fields | One `MudForm` per slice form |
| Field label (standalone) | custom `AppFieldLabel` | `subtitle2` + required marker | For composite fields (address block) |

## 5. Feedback & status

| Component | MudBlazor | Standard configuration | Notes |
|-----------|-----------|------------------------|-------|
| Alert | `MudAlert` | `Variant.Text` custom-toned: semantic text color on semantic background token, 1px border, `Dense="true"`, `ShowCloseIcon` when dismissible | Mirrors site's error box exactly (red on pink, bordered) |
| Snackbar | `MudSnackbar` via `ISnackbar` | bottom-center, 5s auto-hide, max 3 visible; errors `RequireInteraction="true"` | Success after every mutation; errors persist |
| Progress (page) | `MudProgressLinear` | `Color.Primary`, `Indeterminate` | Under app bar during navigation-level loads |
| Progress (inline) | `MudProgressCircular` | `Size.Small` | Buttons, small regions |
| Skeleton | `MudSkeleton` | `Animation.Wave`, shapes matching final layout | Tables/cards first load |
| Empty state | custom `AppEmptyState` | 64px outlined icon `TextMuted`, `h3` title, `body2` hint, optional primary action | "No cases yet — open the first one" |
| Error state | custom `AppError` | Error icon, message, retry button | Failed loads; distinct from validation |
| Confirm dialog | custom `AppConfirmDialog` | `MaxWidth.ExtraSmall`, message + optional consequence list; destructive = red confirm button labeled with the verb ("Reject case", never "OK") | |

## 6. Navigation

| Component | MudBlazor | Standard configuration | Notes |
|-----------|-----------|------------------------|-------|
| Nav menu | `MudNavMenu` | `Bordered="true"` active indicator, icons outlined, groups per department | Active: 700 weight + `Primary` + left 3px accent (site's underline-grow translated to vertical) |
| Breadcrumbs | `MudBreadcrumbs` | `Separator="›"`, last item plain text | Site breadcrumb: blue links, current page unlinked. On every page below root. |
| Menu | `MudMenu` | `Dense="true"`, `AnchorOrigin.BottomRight` | Overflow actions ("⋮") in table rows/toolbars |
| Search bar | custom `AppSearchBar` | `MudTextField` + `Adornment.Start` search icon, `Clearable`, `DebounceInterval="300"` | Site search is icon-triggered; ours is persistent on list pages |
| Filter panel | custom `AppFilterPanel` | chips row for active filters + right-side temporary drawer for full filter form | |
| Tabs (nav) | `MudTabs` | see Surfaces | |
| Language/role switcher | existing `RoleSwitcher` pattern | app-bar right cluster | FR/NL switch lands here in Phase 11 |

## 7. Not used / restricted

| Component | Reason |
|-----------|--------|
| `MudCarousel` | Content-marketing pattern; no place in back-office |
| `MudFab` | See Actions |
| `MudRating`, `MudColorPicker` | No use case; add only with a documented need |
| Elevation ≥ 4 | Outside the elevation scale |
| `MudChip` color-only status | Status must always carry a text label |
| Uppercase button text | Site uses sentence case; theme sets `text-transform: none` |
