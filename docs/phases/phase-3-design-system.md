# Phase 3 — Design system & UI kit

**Status:** Planned (specification complete)

**Goal:** Turn the visual identity and UX philosophy of [www.1030.be](https://www.1030.be/fr) — the official Schaerbeek municipal website — into an enterprise-grade, reusable MudBlazor design system, so all subsequent phases build screens from one branded UI kit instead of ad-hoc MudBlazor defaults.

This is **not** a pixel-perfect clone of the public website. It is a reverse-engineering of its design decisions (color, type, spacing, motion, accessibility posture) re-expressed as MudBlazor configuration for an internal back-office application.

The full specification lives in [`docs/design-system/`](../design-system/README.md). This document records the delivery plan and scope.

---

## Why this phase exists

- Phase 1 shipped functional pages with an inline two-color `MudTheme` in `MainLayout.razor`. Every new page currently re-decides paddings, elevations, and component parameters.
- Phases 4+ add wizards, search dialogs, police portals, dashboards, and certificates — dozens of screens. Without a design system, drift is guaranteed.
- The application represents a municipality that already has a strong public digital identity. Officers should recognize the same brand at their desks.

## Research base

The design language was extracted from the live site (July 2026): home, news, procedures (démarches), opening hours/contact, search, 404, and the compiled stylesheet (`main-*.css`). Key observations are documented in [design-system/README.md](../design-system/README.md) — visual identity analysis and UX philosophy.

## Scope

### In scope

1. **`DesignSystem/` folder in the Web project** — theme, tokens, wrapper components, templates (see [folder structure](../design-system/07-architecture-and-guidelines.md)).
2. **`SchaerbeekTheme`** — production `MudTheme`: light + dark palettes, Open Sans typography scale, layout properties, custom shadow scale. Replaces the inline theme in `MainLayout.razor`. Code: [04-mudtheme-and-css.md](../design-system/04-mudtheme-and-css.md) (verified to compile against MudBlazor 8.15).
3. **Design tokens** — `SchaerbeekColors`, `SchaerbeekTypography`, `SchaerbeekSpacing`, `SchaerbeekLayout`, `SchaerbeekElevation`, `SchaerbeekMotion` static classes; single source of truth shared by theme and components.
4. **Wrapper component library (`App*`)** — first wave: `AppPage`, `AppPageHeader`, `AppSection`, `AppCard`, `AppDataTable`, `AppStatusChip`, `AppEmptyState`, `AppLoading`, `AppAlert`, `AppConfirmDialog`, `AppSaveCancelBar`, `AppSearchBar`, `AppFormSection`, `AppInfoBox`. Specs: [05-ui-kit.md](../design-system/05-ui-kit.md).
5. **Layout rebrand** — `MainLayout` app bar, drawer, and typography aligned to the identity; skip link; turquoise focus ring.
6. **`app.css` rewrite** — minimal: font loading, focus ring, validation colors aligned to the palette, print rules. Everything else must come from the theme.
7. **Retrofit Phase 1 pages** — case list, case detail, new-case dialog, document upload restyled through the UI kit.
8. **Style-guide page** — `/design-system` dev-only page showcasing tokens and every wrapper (living documentation and manual regression aid).

### Out of scope

- New business functionality (no domain, handler, or endpoint changes).
- FR/NL localization (Phase 11 candidate; the kit must not hard-code user-facing strings, however).
- Public-citizen-facing layouts (this system targets back-office density).
- Custom icon font — Material Icons (outlined) are the standard; the site's proprietary `icons-1030` font is not reused.

## Slices

| # | Slice | Deliverable |
|---|-------|-------------|
| 3.1 | Theme & tokens | `DesignSystem/Theme`, `DesignSystem/Tokens`, `MainLayout` switch, `app.css` rewrite |
| 3.2 | Core wrappers | `AppPage`, `AppPageHeader`, `AppSection`, `AppCard`, `AppAlert`, `AppInfoBox`, `AppLoading`, `AppEmptyState` |
| 3.3 | Data wrappers | `AppDataTable`, `AppStatusChip`, `AppSearchBar`, `AppFilterPanel` |
| 3.4 | Form & dialog wrappers | `AppFormSection`, `AppSaveCancelBar`, `AppConfirmDialog`, dialog defaults |
| 3.5 | Retrofit | Phase 1 pages moved onto the kit; style-guide page |

Each slice is demoable and independently mergeable.

## Tests

- **bUnit** component tests: wrappers render the expected MudBlazor structure, forward parameters, and apply token-driven defaults (new test project or existing integration project with bUnit package).
- **Token contrast tests**: plain xUnit tests asserting WCAG AA contrast ratios for every foreground/background token pair used by the palette (the ratios are pure math — cheap and permanent).
- **Existing suites stay green**: no behavior change; integration tests act as the regression net for the retrofit slice.

## Demo script

1. Run AppHost, open the app: branded app bar and drawer in Schaerbeek blue with the yellow accent, Open Sans everywhere.
2. `/registration/cases`: list page built from `AppPage` + `AppPageHeader` + `AppDataTable` + `AppStatusChip`; empty database shows `AppEmptyState`.
3. Case detail: identity form inside `AppFormSection` with `AppSaveCancelBar`; validation errors in the brand error red on pink background.
4. `/design-system`: token swatches, typography scale, and one example of every wrapper.
5. Keyboard-only walk-through: visible turquoise focus ring on every interactive element, skip link works.

## Risks & notes

- **MudBlazor version coupling** — theme classes (`PaletteLight`, `Typography` subtypes) evolve between major versions; theme code is centralized so upgrades touch one file.
- **Retrofit regressions** — mitigated by slicing retrofit last and keeping integration tests green.
- **Scope creep** — the wrapper catalogue in the spec is the ceiling, not the floor; only wrappers needed by existing pages plus the documented first wave are built in this phase. Later phases add wrappers on demand (e.g. `AppWizard` when Phase 4 needs it).

## Carries forward

- `AppWizard` / stepper template (needed by Phase 4 address & household).
- `AppTimeline` (case history, Phase 7 review).
- Dashboard templates and statistic cards (Phase 7 review dashboard, Phase 11 reporting).
- Dark palette activation toggle (palette ships in this phase; the user-facing switch is optional later).
