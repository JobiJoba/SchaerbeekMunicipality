# Design tokens

Single source of truth for every visual decision. Tokens are strongly typed C# static classes in `DesignSystem/Tokens/` (code in [04-mudtheme-and-css.md](./04-mudtheme-and-css.md)); this document is the reference for their values and usage rules.

**Rule:** application code never contains a raw hex value, pixel padding, or elevation number. It references a token or uses a wrapper component that does.

---

## 1. Color palette

All contrast ratios computed per WCAG 2.1. "AA text" = ≥ 4.5:1 normal text, ≥ 3:1 large text (18.66px bold / 24px). "AA non-text" = ≥ 3:1 for UI components and focus indicators.

### 1.1 Brand colors

| Token | HEX | RGB | HSL | MudBlazor property | Usage | Contrast | WCAG |
|-------|-----|-----|-----|--------------------|-------|----------|------|
| `Primary` | `#00537F` | 0, 83, 127 | 201°, 100%, 25% | `Palette.Primary`, `AppbarBackground` | App bar, primary buttons, links, active nav, table header text | 8.26:1 on white; white on it 8.26:1 | AA + AAA text |
| `PrimaryDarken` | `#003A59` | 0, 58, 89 | 201°, 100%, 17% | `Palette.PrimaryDarken` | Hover/pressed primary, visited emphasis | 11.9:1 on white | AAA |
| `PrimaryLighten` | `#2E7BA6` | 46, 123, 166 | 202°, 57%, 42% | `Palette.PrimaryLighten` | Selected row tint source, charts | 4.6:1 on white | AA text |
| `Secondary` | `#087F74` | 8, 127, 116 | 174°, 88%, 26% | `Palette.Secondary` | Secondary actions, selected filters | 5.4:1 on white | AA text |
| `SecondaryLighten` | `#08B0A0` | 8, 176, 160 | 174°, 91%, 36% | `Palette.SecondaryLighten` | **Focus ring**, decorative tag fills (site original) | 2.72:1 on white | AA **non-text only** — never text |
| `Accent` | `#FDC300` | 253, 195, 0 | 46°, 100%, 50% | `Palette.Tertiary` | Brand highlight: active wizard step, KPI accents, "new" markers | 11.67:1 with `#111` text on it | AA as fill with dark text; never text itself |

### 1.2 Semantic colors

| Token | HEX | RGB | HSL | MudBlazor property | Usage | Contrast | WCAG |
|-------|-----|-----|-----|--------------------|-------|----------|------|
| `Success` | `#2E7D32` | 46, 125, 50 | 123°, 46%, 34% | `Palette.Success` | Completed checklist items, positive chips | 5.13:1 on white | AA |
| `SuccessBackground` | `#E6EFE9` | 230, 239, 233 | 140°, 22%, 92% | — | Success alert/chip background | — | pair with `Success` text |
| `Warning` | `#B75500` | 183, 85, 0 | 28°, 100%, 36% | `Palette.Warning` | Pending verifications, expiring permits (site orange `#EE743B` darkened: 2.92:1 → 4.87:1) | 4.87:1 on white | AA |
| `WarningBackground` | `#FBE9DD` | 251, 233, 221 | 24°, 79%, 93% | — | Warning alert background | — | pair with `Warning` text |
| `Error` | `#9A0D20` | 154, 13, 32 | 352°, 84%, 33% | `Palette.Error` | Validation errors, rejections, destructive actions (site's exact form-error red) | 8.56:1 on white; 6.69:1 on `ErrorBackground` | AA + AAA |
| `ErrorBackground` | `#F9DCE5` | 249, 220, 229 | 341°, 71%, 92% | — | Error alert/field background (site original) | — | pair with `Error` text |
| `Info` | `#0B6BA8` | 11, 107, 168 | 203°, 88%, 35% | `Palette.Info` | Informational alerts, in-progress states | 5.70:1 on white | AA |
| `InfoBackground` | `#E1E9F4` | 225, 233, 244 | 215°, 46%, 92% | — | Info alert background (from site's `#E1E4F4` input-focus tint) | — | pair with `Info` text |

### 1.3 Neutrals — text

| Token | HEX | MudBlazor property | Usage | Contrast on white | WCAG |
|-------|-----|--------------------|-------|-------------------|------|
| `TextPrimary` | `#111111` | `Palette.TextPrimary` | Body, headings (site standard) | 18.88:1 | AAA |
| `TextSecondary` | `#444444` | `Palette.TextSecondary` | Labels, table headers, captions | 9.74:1 | AAA |
| `TextMuted` | `#707070` | `Palette.GrayDefault` | Placeholder, timestamps, helper text | 4.95:1 | AA |
| `TextDisabled` | `#AEAEAE` | `Palette.TextDisabled` | Disabled controls | 2.32:1 | exempt (disabled) |
| `TextOnPrimary` | `#FFFFFF` | `Palette.PrimaryContrastText` | Text on blue/semantic fills | 8.26:1 on Primary | AAA |

### 1.4 Neutrals — surfaces and borders

| Token | HEX | MudBlazor property | Usage |
|-------|-----|--------------------|-------|
| `Background` | `#F5F6F7` | `Palette.Background` | Page canvas behind cards (slightly cool, keeps white surfaces readable) |
| `Surface` | `#FFFFFF` | `Palette.Surface` | Cards, dialogs, drawer, tables |
| `SurfaceSunken` | `#F3F3F3` | `Palette.BackgroundGray` | Read-only field fills, zebra rows on the site, disabled backgrounds |
| `SurfaceStriped` | `#F8F9FA` | `Palette.TableStriped` | Table zebra striping (subtler than site's `#F3F3F3` for dense grids) |
| `SurfaceHover` | `#EDF3F7` | `Palette.TableHover` | Row hover — blue-tinted to echo the brand |
| `BorderDefault` | `#DADADA` | `Palette.LinesDefault`, `Divider` | Card borders, dividers, download-button borders on site |
| `BorderLight` | `#ECECEC` | `Palette.DividerLight` | Hairlines inside components |
| `BorderInput` | `#AEAEAE` | `Palette.LinesInputs` | Input outlines at rest |
| `BorderStrong` | `#707070` | table header border | Site uses a 3px `#707070` bottom border under table headers |

### 1.5 Interaction colors

| Token | Value | Usage |
|-------|-------|-------|
| `Focus` | `#08B0A0`, 3px solid outline, 2px offset | Global focus indicator — the site's signature accessibility trait. 2.72:1 on white exceeds the 3:1 non-text requirement marginally; the 3px width and offset guarantee visibility. On dark fills, the outline sits outside the element on the light canvas. |
| Hover (tonal) | `Palette.HoverOpacity = 0.06` | MudBlazor applies primary at 6% for hover washes |
| Selected row | `PrimaryLighten` @ 12% | List/table selection |
| Disabled fill | `SurfaceSunken` + `TextDisabled` | All disabled controls |

## 2. Typography

**Family:** `"Open Sans", "Segoe UI", "Helvetica Neue", Arial, sans-serif` — the site's single content typeface with pragmatic fallbacks. Self-host WOFF2 (400, 600, 700 + italics) in `wwwroot/fonts/`; no CDN dependency for an internal tool.

**Weights:** 400 (body), 600 (subtitles, emphasis), 700 (headings, buttons, labels — the site uses 700 heavily). Avoid 300 (site never uses light weights; they read as unofficial).

### Type scale

Site anchors: h1 32px/48px, h2 24px/36px, h3 20px/30px, body 16px/1.5, small 14px, caption 12px — all bold 700 headings. Mapped to MudBlazor `Typo`:

| Typo | Size | Weight | Line height | Letter spacing | Use |
|------|------|--------|-------------|----------------|-----|
| `h1` | 2rem (32px) | 700 | 1.5 | normal | Page title — exactly one per page |
| `h2` | 1.5rem (24px) | 700 | 1.5 | normal | Page section |
| `h3` | 1.25rem (20px) | 700 | 1.5 | normal | Card/panel title |
| `h4` | 1.125rem (18px) | 700 | 1.5 | normal | Sub-panel, dialog title |
| `h5` | 1rem (16px) | 700 | 1.5 | normal | Form section heading |
| `h6` | 0.875rem (14px) | 700 | 1.5 | 0.02em | Small group label |
| `subtitle1` | 1rem | 600 | 1.5 | normal | Emphasized body, definition labels |
| `subtitle2` | 0.875rem | 600 | 1.5 | normal | Table headers, dense labels |
| `body1` | 1rem (16px) | 400 | 1.5 | normal | Default text, form inputs |
| `body2` | 0.875rem (14px) | 400 | 1.5 | normal | Table cells, metadata, secondary text |
| `button` | 0.9375rem (15px) | 700 | 1.75 | 0.01em | **No uppercase** (`text-transform: none`) — sentence case like the site |
| `caption` | 0.75rem (12px) | 400 | 1.4 | normal | Timestamps, footnotes |
| `overline` | 0.75rem | 700 | 1.4 | 0.08em, uppercase | Eyebrow labels above titles |

**Paragraph spacing:** 1rem after paragraphs and headings (site: `margin-block-end: 1rem`); 2rem above a new `h2` section.

**Navigation:** drawer items `body1` at 400, active item 700 in `Primary`. **Tables:** headers `subtitle2` in `TextSecondary`, cells `body2`.

## 3. Spacing system

Base-8 with a 4px half-step — matches both the site's rem rhythm (0.25/0.5/1/1.25/2/3rem) and MudBlazor utility classes (1 unit = 4px).

**Scale (px):** `2, 4, 8, 12, 16, 20, 24, 32, 40, 48, 64, 80, 96`

| Context | Value | MudBlazor |
|---------|-------|-----------|
| Page padding (desktop) | 24px | `MudMainContent` + `pa-6` |
| Page padding (mobile) | 16px | `pa-4` |
| Content max width | 1280px | `MaxWidth.ExtraLarge` (site container: 1280px) |
| Form max width | 760px | token `FormMaxWidth` — readable single column |
| Section vertical gap | 32px | `mb-8` |
| Card padding | 16px | `MudCardContent` default / `pa-4` |
| Card grid gutter | 12px | `MudGrid Spacing="3"` |
| Dialog padding | 24px content, 16px actions | `MudDialog` defaults + `pa-6`/`pa-4` |
| Form row gap | 16px | `gap-4` / grid `Spacing="4"` |
| Form section gap | 32px | `mb-8` |
| Table cell padding | 12px × 16px (dense) | `MudTable Dense="true"` (site uses a luxurious 24px — too airy for work queues) |
| Toolbar height / gap | 56px / 8px | `MudToolBar Dense` + `gap-2` |
| Inline icon–text gap | 8px | `gap-2` |

## 4. Elevation system

The site is nearly flat: 1px borders carry most separation; two soft ambient shadows appear on cards/hover; one stronger shadow on overlays. Elevation scale:

| Level | Token | Shadow | Use |
|-------|-------|--------|-----|
| 0 | `Flat` | none + 1px `BorderDefault` | **Default.** Tables, forms, panels, sections |
| 1 | `Card` | `0 0 0.5rem rgba(17,17,17,.10)` | Resting cards, app bar |
| 2 | `Raised` | `0 0.25rem 1rem rgba(17,17,17,.10)` | Card hover, drawer, popovers |
| 3 | `Overlay` | `0 0.3rem 1.5rem rgba(0,0,0,.15)` | Dialogs, menus, sticky save bar (site's cookie-banner shadow) |

Never use MudBlazor elevations 4–25. The theme overrides levels 1–3; anything higher is a code-review flag.

## 5. Border system

| Element | Radius | Thickness |
|---------|--------|-----------|
| Global default (`LayoutProperties.DefaultBorderRadius`) | **4px** | — |
| Buttons | 4px (site: 0 — softened, see README) | 0 (filled) / 1px (outlined) |
| Inputs | 4px (site: 0.3rem ≈ 5px) | 1px rest, 2px focus |
| Cards / papers | 4px | 1px `BorderDefault` |
| Dialogs | 4px | none (elevation 3) |
| Status chips | 12px (pill-ish, echoes site tag/pill mix) | 0 |
| Tables | 4px outer, 0 inner | 1px rows; 2px `BorderStrong` under header (site: 3px) |
| Avatars | 50% | — |

## 6. Iconography

- **Family:** Material Icons **Outlined** (`Icons.Material.Outlined.*`) — closest to the site's thin line icons (`icons-1030` font); filled variants only for active nav states and filled status chips.
- **Sizes:** `Size.Small` 20px (table row actions, chips), `Size.Medium` 24px (default: buttons, nav, alerts), `Size.Large` 32px (empty states, dialog headers). Empty-state illustrations: 64px.
- **When icons:** status reinforcement (always with text), universally understood actions (search, close, edit, delete, download), nav items, alert severity.
- **When text only:** primary workflow actions ("Approve case", "Send to police") — the site prefers labeled buttons; icon-only buttons require `aria-label` and a tooltip and are allowed only in table rows and toolbars.

## 7. Motion system

Site: `transition: all .5s` on links/buttons, `.2s–.3s ease-in-out` on menus and underline effects. Adopted (slightly faster for back-office rhythm):

| Token | Value | Use |
|-------|-------|-----|
| `FastMs` | 150ms | Hover/press feedback, focus ring |
| `NormalMs` | 250ms | Expansion panels, drawer, tabs, menus |
| `SlowMs` | 400ms | Dialog open/close, page-level fades |
| `Easing` | `ease-in-out` | Everything |

Rules: no motion carries information alone; no looping decoration; loading = `MudProgressLinear` (page/section) or `MudProgressCircular` (buttons, inline); skeletons (`MudSkeleton`) for tables/cards on first load; snackbars slide from bottom-center, auto-hide 5s, errors require manual dismiss; respect `prefers-reduced-motion` (handled in app.css).

## 8. Responsive design

Site breakpoints cluster at 576/640/768/992/1024/1200. Mapped to MudBlazor defaults (xs <600, sm ≥600, md ≥960, lg ≥1280, xl ≥1920):

| Range | Target | Layout behavior |
|-------|--------|-----------------|
| lg+ (≥1280) | Desktop (primary target) | Drawer open (`Responsive`), full tables, side detail panels |
| md (960–1279) | Laptop | Drawer toggleable, detail panels stack below |
| sm (600–959) | Tablet | Drawer overlays (temporary), tables shed secondary columns, forms single-column |
| xs (<600) | Phone (consultation only) | Tables become card lists (`MudHidden` swap), toolbar actions collapse into menu |

Typography does not scale down (site keeps 16px body on mobile). Page padding steps 24→16px below md. Officers use desktops; mobile is read-mostly — never hide primary actions on desktop to satisfy phone layouts.

## 9. Z-index

MudBlazor defaults (`new ZIndex()`): Drawer 1100 < AppBar 1300 < Dialog 1400 < Popover 1500 < Snackbar 1600 < Tooltip 1700. No custom stacking contexts in application code; if something needs a z-index, it belongs in the design system, not the page.

## 10. Container widths

| Token | Value | Use |
|-------|-------|-----|
| `ContentMaxWidth` | 1280px | Page container (`MaxWidth.ExtraLarge`) |
| `FormMaxWidth` | 760px | Single-column create/edit forms |
| `DetailPanelWidth` | 320px | Right-hand details/filter drawer |
| Dialog widths | S 444px / M 600px / L 800px | `MaxWidth.ExtraSmall/Small/Medium` on `MudDialog` |
