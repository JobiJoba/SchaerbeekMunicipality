# Accessibility

Target: **WCAG 2.1 AA** for the whole application, with AAA contrast wherever it comes free (most of the palette already exceeds 7:1). Accessibility is part of the Schaerbeek identity — the public site treats it as a first-class feature, and an internal tool used 8 hours a day by diverse staff deserves at least the same standard.

## 1. Review of the source site

What 1030.be does well (and we adopt):

| Practice | Observation | Adoption |
|----------|-------------|----------|
| Global visible focus | `body *:focus { outline: 3px solid #08B0A0 }` — an unmissable turquoise ring on everything | Same ring, plus `outline-offset: 2px`, via app.css (see [04](./04-mudtheme-and-css.md)) |
| Skip link | "Passer au contenu principal" → `#main` | `MainLayout` renders a skip link as its first focusable element |
| Semantic landmarks | `<header>`, `<main>`, `<nav class="breadcrumb">`, `<footer>` | Wrappers emit correct landmarks (`AppPage` → `main`, `AppBreadcrumb` → `nav`) |
| High-contrast text | `#111` on white (18.9:1); links `#00537F` (8.3:1) | Tokenized as `TextPrimary` / `Primary` |
| Error presentation | Red `#9A0D20` text on pink `#F9DCE5` background with border — color *plus* container change | `AppAlert`/validation styling reuses the exact pair |
| Plain-language commitment | FALC ("Langage facile") and sign-language versions | Out of scope technically, but informs microcopy guidance: short sentences, no jargon in UI text |
| Large hit targets | Buttons min-height 3rem (48px) | 48px primary actions, 40px minimum elsewhere |

Weaknesses observed (and corrected in our system):

| Issue | Site behavior | Our rule |
|-------|---------------|----------|
| Some `outline: none` resets | Carousel (slick) arrows and a few widgets suppress focus | Never remove an outline without an equal-or-better replacement; lint for `outline: none` |
| Turquoise/orange as text-adjacent fills | `#08B0A0` (2.72:1) and `#EE743B` (2.92:1) with white text on tags | Darkened AA variants (`#087F74`, `#B75500`) for any text role; originals restricted to non-text uses |
| Sparse ARIA | Mostly `aria-label`/`aria-hidden`; little live-region use (content site, low need) | Back-office is dynamic: explicit live regions and ARIA patterns required (below) |
| `transition: all` | Broad transitions ignore user motion preferences | `prefers-reduced-motion` media query disables non-essential motion globally |

## 2. Rules for this application

### Keyboard

- Every workflow completable keyboard-only; this is a **release criterion** per phase demo.
- Tab order follows visual order; never use positive `tabindex`.
- Dialogs trap focus (MudBlazor default), return focus to the trigger on close, close on Escape.
- Tables: row actions reachable by Tab; provide a "focus first row" shortcut on list pages later if queues grow.
- Drawer navigation: arrow keys within `MudNavMenu` (built-in), `Enter` activates.

### Focus indicator

- 3px solid `#08B0A0`, `outline-offset: 2px`, on `:focus-visible` — global, no exceptions.
- Components with their own focus styling (Mud inputs) keep it *in addition to* meeting 3:1 contrast against the surrounding surface.

### Forms and validation

- Every input has a visible `Label` (MudBlazor parameter) — placeholder is never the label.
- Required fields marked with a suffix and `Required="true"` (adds `aria-required`).
- Errors: shown at the field (`ErrorText`, red `#9A0D20`) *and* summarized in an `AppAlert` at the top of long forms; the alert is a `role="alert"` live region.
- Error text is specific and actionable ("National number must contain 11 digits"), never "invalid value".
- Helper text via `HelperText` — persistent, not tooltip-only.
- Never rely on color alone: error state = color + icon + message; success chip = color + label text.

### Screen readers

- One `h1` per page (the `AppPageHeader` title); heading levels never skip.
- Icon-only buttons require `aria-label` (enforced: `AppIconButton` makes it a required parameter).
- Status chips render text, not just color (`AppStatusChip` requires a label).
- Async updates (save confirmations, list refreshes) announced via snackbar (MudBlazor snackbar renders `role="status"`) or an explicit `aria-live="polite"` region in the page template.
- Tables: `MudTable` emits proper `<th>` scope; always set a caption or `aria-label` describing the table ("Registration cases awaiting action").
- Loading: `AppLoading` exposes `role="status"` with visually hidden text "Loading…".

### Contrast (enforced by tests)

Token pairs asserted ≥ 4.5:1 in unit tests (see Phase 3 plan): `TextPrimary`/`Surface`, `TextSecondary`/`Surface`, `TextMuted`/`Surface`, `Primary`/`Surface`, white/`Primary`, `Error`/`ErrorBackground`, `Warning`/`Surface`, `Success`/`Surface`, `Info`/`Surface`, white on all four semantic fills, `TextPrimary`/`Accent`.

### Language & content

- UI language is English for now (learning project) but no user-facing string may be hard-coded inside a design-system component — everything is a `RenderFragment` or `string` parameter, keeping FR/NL localization (Phase 11) mechanical.
- Dates, national numbers, and addresses displayed in Belgian conventions (dd/mm/yyyy) with `aria-label` full forms where truncated.

### Motion

- `prefers-reduced-motion: reduce` → transitions and animations effectively disabled (app.css rule).
- Nothing flashes more than 3 times/second (nothing flashes at all, in practice).

## 3. Testing accessibility

| Layer | Tool / method |
|-------|---------------|
| Static | bUnit assertions on wrapper markup (labels, roles, aria attributes present) |
| Tokens | xUnit contrast-ratio tests on the palette |
| Manual per phase | Keyboard-only demo walkthrough; browser dev-tools accessibility tree spot check |
| Future | Playwright + axe-core scan on the style-guide page (`/design-system`) — recommended when E2E tests arrive |
