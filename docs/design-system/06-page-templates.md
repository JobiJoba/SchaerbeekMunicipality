# Page templates

Repeatable skeletons for every screen type the municipal back office needs. A template is a *composition recipe* of UI-kit wrappers — most are conventions documented here; only layouts (`MainLayout`, future `WizardLayout`) are actual `.razor` layout files.

Every template starts from the same frame:

```razor
<AppPage Title="..." Breadcrumbs="_breadcrumbs">
    <AppPageHeader Title="..." Subtitle="...">
        <Actions>@* primary action *@</Actions>
    </AppPageHeader>
    @* template body *@
</AppPage>
```

---

## 1. List page (work queue)

The most common screen. Example: `/registration/cases`.

```razor
<AppPage Title="Registration cases">
    <AppPageHeader Title="Registration cases" Subtitle="Open and track first-registration cases.">
        <Actions>
            <MudButton Variant="Variant.Filled" Color="Color.Primary"
                       StartIcon="@Icons.Material.Outlined.Add" OnClick="OpenNewCaseDialog">
                New case
            </MudButton>
        </Actions>
    </AppPageHeader>

    <AppToolbar>
        <AppSearchBar @bind-Value="_search" OnSearch="Reload" Placeholder="Search by name or case number…" />
        <MudSpacer />
        <AppFilterPanel Filters="_filters" OnChanged="Reload" />
    </AppToolbar>

    <AppDataTable T="CaseListItem" Items="_items" Loading="_loading"
                  Caption="Registration cases awaiting action" OnRowClick="GoToDetail">
        <HeaderContent>...</HeaderContent>
        <RowTemplate Context="item">
            ... <AppStatusChip Status="item.Status" /> ...
        </RowTemplate>
        <EmptyState>
            <AppEmptyState Title="No cases yet" Description="Open the first registration case to get started."
                           Icon="@Icons.Material.Outlined.FolderOpen">
                <Action>...</Action>
            </AppEmptyState>
        </EmptyState>
    </AppDataTable>
</AppPage>
```

Rules: one primary action in the header; search left, filters right; status always via `AppStatusChip`; row click navigates but the case number is also a real link.

## 2. Details page

Example: case detail. Header carries identity + status; intake steps and documents split into a two-column grid on large viewports (Phase 4.1).

```
AppPageHeader (Title = case number, StatusChip, Actions = workflow verbs)
AppInfoBox (optional procedural guidance)
MudGrid:
  Left (lg=7)  → intake steps (identity, residence, address, household, civil status)
  Right (lg=5) → RegistrationCaseDocumentPanel (sticky upload + list + preview)
```

Future phases may add `MudTabs` for history and review facets once a record has more than two aspects:

```
MudTabs: Overview | Identity | Documents | History
  Overview  → AppPropertyGrid in AppCards (2-column MudGrid)
  Identity  → form template (see below) or read-only AppPropertyGrid
  Documents → AppDocumentPreview list + upload zone
  History   → AppTimeline (Phase 7)
```

Rules: workflow actions (Approve, Send to police) top-right in the header, destructive ones behind `AppConfirmDialog`; secondary facts never above primary task content.

## 3. Create form (dialog or page)

- **≤ 5 fields, single concern** → dialog (`NewCaseDialog` pattern): `MudDialog` Small, `MudForm` + validator, `AppSaveCancelBar` in the actions slot.
- **Larger** → dedicated page:

```razor
<AppPage Title="Record identity">
    <AppPageHeader Title="Record identity" Subtitle="Case 2026-00042 — Jane Doe" />
    <MudContainer MaxWidth="MaxWidth.Medium" Class="px-0 mx-0"> @* FormMaxWidth 760px *@
        <MudForm @ref="_form" Model="_model" Validation="_validator.ValidateValue">
            <AppFormSection Title="Identity" Description="As shown on the presented document.">
                <MudItem xs="12" md="6"><MudTextField Label="First name" Required="true" ... /></MudItem>
                ...
            </AppFormSection>
            <AppFormSection Title="Birth details">...</AppFormSection>
        </MudForm>
        <AppSaveCancelBar OnSave="Save" OnCancel="Back" Saving="_saving" />
    </MudContainer>
</AppPage>
```

Rules: forms max 760px wide, two columns collapsing to one below md; validation summary as `AppAlert Severity.Error` on submit failure; success = snackbar + navigate.

## 4. Edit form

Same as create, plus: pre-populated model; `AppSaveCancelBar` disabled until dirty; unsaved-changes confirmation (`AppConfirmDialog`) on navigation; read-only fields rendered as `SurfaceSunken` disabled inputs (national number after assignment) with a hint why.

## 5. Wizard (Phase 4+, `AppWizard`)

Multi-step case processing (identity → residence → address → household). `MudStepper` shell: step list with semantic state colors (done = Success, active = Primary + yellow accent underline, error = Error); one `AppFormSection`-based step body; sticky `AppActionBar` with Back / Save draft / Continue. Each step saves via its own slice handler — leaving mid-wizard never loses data.

## 6. Search results (Phase 5, NR search)

Search dialog/page: `AppSearchBar` + structured criteria (name, birth date), results as `AppDataTable` with match score, each row expandable to `AppPersonCard`; explicit actions "Link existing person" / "Create new person"; zero-results state uses `AppEmptyState` with "create new" action; possible-duplicate warning is a persistent `AppAlert Severity.Warning` on the case afterwards.

## 7. Dashboard (Phase 7 review dashboard)

```
AppPageHeader ("Good morning, Officer …")
MudGrid: 4× AppStatisticCard (My open cases / Awaiting police / Ready for decision / Suspended)
AppSection "Needs my attention" → AppDataTable (top 10 actionable, pre-filtered)
AppSection "Quick actions" → AppQuickActions grid
```

Rules: statistics link to the corresponding filtered list; queues before charts; yellow accent bar only on the single most important KPI.

## 8. Document viewer (Phase 4.1 partial; Phase 8 full)

**Phase 4.1** ships a practical subset on the case detail page: sticky right column with document list + inline preview (`RegistrationCaseDocumentPanel`, `DocumentPreviewContent`). PDFs use an iframe; images use `<img>`; other types fall back to download only. Fullscreen via `DocumentPreviewDialog`.

The **full** template below (print toolbar, dedicated `AppDocumentPreview` wrapper, left-list/right-preview swap) remains planned for Phase 8:

Two-pane details layout: left = document list (`AppDocumentPreview`), right = preview area (PDF iframe/image) inside `AppCard` with toolbar (download, print). Falls back to metadata + download when preview is impossible. Print uses the global print CSS.

## 9. Citizen profile / person file (Phase 11 read model)

Header = person identity (`MudAvatar` initials, name, national number, `AppStatusChip` register type) + `AppPropertyGrid` facts; tabs: Household | Addresses | Cases | Certificates. Composes the details-page template — no new primitives.

## 10. Administration & settings pages

Simple stacked `AppSection`s of `AppCard`s, each card one concern (seed data, notification log, feature toggles later). Left-aligned, `MaxWidth.Medium`; no dashboards, no tabs until a page exceeds ~4 cards.

## 11. Login / role selection

Current fake-auth `RoleSwitcher` stays in the app bar. If a real login page arrives (Phase 10+): centered `MudPaper Elevation=1` card 444px on `Background` canvas, municipal wordmark, form, primary button full-width — mirrors the sober site identity, no hero imagery.

## 12. Error page & not found

`Error.razor` / `NotFound.razor` re-skinned with the kit (site 404 is a plain message + search + home link):

```
AppPage → AppEmptyState
  Icon: Outlined.ErrorOutline (500) / Outlined.SearchOff (404)
  Title: "Something went wrong" / "Page not found"
  Description: request id (500) / "Check the address or return to your cases" (404)
  Action: "Back to home" filled primary
```

Never expose stack traces; log correlation id shown as caption.

## 13. Empty page (scaffold)

The starting point for any new page — `AppPage` + `AppPageHeader` + one `AppSection` — documented in [07-architecture-and-guidelines.md](./07-architecture-and-guidelines.md#building-a-new-page) as the copy-paste snippet.
