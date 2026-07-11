# Schaerbeek Design System

An enterprise-grade design system and UI kit for the Schaerbeek Municipality back-office application, reverse-engineered from the design language of the official municipal website [www.1030.be](https://www.1030.be/fr) and re-expressed as MudBlazor configuration.

**This is not a clone.** The public website serves citizens reading content; this application serves officers processing complex administrative workflows (large forms, tables, document management, citizen records). The system preserves the *visual identity and usability philosophy* while adapting density and interaction patterns to back-office work.

Implementation is planned as [Roadmap Phase 3](../ROADMAP.md); delivery plan in [phases/phase-3-design-system.md](../phases/phase-3-design-system.md).

## Documents

| # | Document | Contents |
|---|----------|----------|
| — | This file | Executive summary, UX philosophy, visual identity analysis |
| 1 | [01-design-tokens.md](./01-design-tokens.md) | Color palette, typography, spacing, elevation, borders, motion, breakpoints, iconography, z-index |
| 2 | [02-accessibility.md](./02-accessibility.md) | Accessibility review of the source site, WCAG targets, rules for this application |
| 3 | [03-component-catalogue.md](./03-component-catalogue.md) | Component inventory and MudBlazor mapping with standard parameters |
| 4 | [04-mudtheme-and-css.md](./04-mudtheme-and-css.md) | Production-ready `SchaerbeekTheme` (compiles against MudBlazor 8.15) and minimal `app.css` |
| 5 | [05-ui-kit.md](./05-ui-kit.md) | `App*` wrapper component specifications (API, composition, when to use) |
| 6 | [06-page-templates.md](./06-page-templates.md) | Page templates: list, details, form, wizard, dashboard, error, … |
| 7 | [07-architecture-and-guidelines.md](./07-architecture-and-guidelines.md) | Folder structure, naming conventions, developer guidelines, do's & don'ts, future recommendations |

Cross-feature municipal UI (`BelgianAddressFields`, `NationalRegisterSearchForm`, …) is documented separately in [MUNICIPAL-UI.md](../MUNICIPAL-UI.md).

---

## 1. Executive summary

The Schaerbeek municipal website presents a sober, high-contrast, typography-driven government identity: one dominant institutional blue (`#00537F`), a warm yellow brand accent (`#FDC300`), generous whitespace, square-ish geometry, bold Open Sans headings, and an unusually strong accessibility posture (visible turquoise focus outlines on *every* element, "Langage facile" and sign-language entry points, FR/NL parity).

This design system translates those decisions into:

- **Design tokens** — strongly typed C# constants (`SchaerbeekColors`, `SchaerbeekSpacing`, …) that are the single source of truth.
- **A production `MudTheme`** — palettes (light + dark), an Open Sans type scale, layout properties, and a flattened three-level shadow scale.
- **A wrapper UI kit** — `App*` components so application pages compose semantics (`AppPageHeader`, `AppStatusChip`) rather than raw MudBlazor parameters.
- **Page templates** — repeatable skeletons for list, detail, form, wizard, and dashboard pages so hundreds of screens stay consistent.
- **Guidelines** — accessibility rules, naming conventions, and do's & don'ts that keep the system coherent as it grows.

Adjustments made deliberately for an internal tool (documented per token):

- Site colors that fail WCAG AA as text (turquoise `#08B0A0` at 2.7:1, orange `#EE743B` at 2.9:1) are kept for *non-text* roles (focus ring, tag fills) and **darkened variants** are used for text/semantic roles.
- The site's fully square buttons (`border-radius: 0`) are softened to a 4px radius as the single global radius — retaining the sober geometry while avoiding fighting MudBlazor's component set.
- Density is higher than the public site (tables, forms) because officers use these screens all day.

## 2. UX philosophy

Observed patterns on 1030.be and the intent we infer behind them — these principles govern every design decision in this system:

1. **Trust through sobriety.** One blue, one accent, no gradients, minimal shadows, no decorative animation. A government interface must look official and calm, not promotional. → *Our rule:* color signals meaning (status, action, error) — never decoration.
2. **Readability first.** 16px body text, 1.5 line height, bold 700 headings with clear size steps, dark `#111` text on white. Content pages are essentially typographic. → *Our rule:* never shrink text below 14px except captions; contrast ≥ 4.5:1 always.
3. **Accessibility as identity, not compliance.** A 3px turquoise focus outline is applied globally (`body *:focus`), there's a skip link, "Langage facile" (plain language) and sign-language versions, strict FR/NL parity. Accessibility is treated as part of the brand. → *Our rule:* every interactive element has a visible focus state; keyboard-only operation is a release criterion.
4. **Low cognitive load through task orientation.** The homepage leads with six task tiles (contact, appointments, procedures, parking, waste, transparency) before any news. Citizens come to *do* something. → *Our rule:* pages lead with the primary task; dashboards lead with work queues, not statistics.
5. **Consistency over novelty.** Every card, list, and news item uses the same structure: image, bold title, short teaser, single link with an underline-grow hover. → *Our rule:* one pattern per problem; new patterns require a documented reason.
6. **Large hit targets.** Buttons have a 3rem (48px) minimum height and generous horizontal padding; nav items are full-width rows. → *Our rule:* 40px minimum interactive height in dense back-office contexts, 48px for primary actions.
7. **Calm motion.** Transitions are `all .5s` / `.2–.3s ease-in-out` — hover feedback and underline growth, nothing attention-seeking. → *Our rule:* 150–400ms, ease-in-out, motion communicates state change only.
8. **Bilingual, plural identity.** "Schaerbeek – Schaarbeek" everywhere. → *Our rule:* never hard-code user-facing strings inside kit components; take them as parameters (localization-ready).

## 3. Visual identity analysis

Extracted from the live site (home, news, procedures, contact/opening hours, search, 404) and its compiled stylesheet, July 2026.

### Identity anchors

| Element | Observation | System translation |
|---------|-------------|--------------------|
| Institutional blue `#00537F` | 42 uses in the stylesheet — header, footer, links, buttons, breadcrumbs; defined as an RGBA custom-property "main color" | `Palette.Primary`, app bar, links, primary buttons |
| Yellow `#FDC300` | Logo/brand accent, sparingly in graphics | Tertiary/accent token — highlights, active wizard step, never text |
| Turquoise `#08B0A0` | Global focus outline + "tag" chips | Focus ring token; darkened `#087F74` as Secondary for text roles |
| Near-black `#111111` on white | Body text, headings | `TextPrimary` |
| Grey ramp `#F3F3F3 → #ECECEC → #DADADA → #AEAEAE → #707070 → #444` | Backgrounds, borders, muted text | Neutral tokens (surfaces, borders, secondary text) |
| Error red `#9A0D20` on pink `#F9DCE5` | Form validation messages | Error + ErrorBackground tokens (6.7:1 on its background) |
| Open Sans 400/600/700 | The only content typeface (icon fonts aside) | Theme-wide font family with system fallbacks |
| Square geometry | Buttons `border-radius: 0`; inputs/date picker `0.3rem`; a few pills for tags | Single 4px global radius; pills only for status chips |
| Soft ambient shadows | `0 0 .5rem` and `0 .25rem 1rem` at ~10% black; most surfaces flat with 1px borders | 3-level elevation scale; default elevation 0 + borders |
| Flat blue footer / white header | Structural color blocking rather than shadows | Blue app bar, white drawer with border, blue footer band |
| Underline-grow link hover | 3px underline animates to full width in 0.2s | Reserved for top-navigation elements; standard links use plain underline |
| 1280–1320px container | `max-width: 1280px` content wrapper | `MaxWidth.ExtraLarge` page container, 1280px token |

### What we deliberately do differently

| Public site | This application | Why |
|-------------|------------------|-----|
| Content-marketing layout (hero images, news grids) | Work-queue layout (tables, forms, detail panels) | Different job: reading vs processing |
| Fully square buttons | 4px radius everywhere | One radius token; MudBlazor-native look without heavy CSS overrides |
| Turquoise/orange as fills with white text (AA fail) | Darkened variants for text; originals kept for non-text roles | WCAG AA is non-negotiable for an all-day work tool |
| Custom `icons-1030` icon font | Material Icons Outlined | Licensing, MudBlazor integration, breadth |
| `transition: all .5s` on many elements | 150–400ms scoped transitions | .5s feels sluggish at back-office interaction frequency |
| No dark mode | Optional dark palette shipped | Officer preference, low-light desks — cheap to provide via `PaletteDark` |
