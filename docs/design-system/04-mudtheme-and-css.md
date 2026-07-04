# MudTheme & global CSS

Production-ready theme and tokens. The C# below **compiles as-is against MudBlazor 8.15.0** (verified with a throwaway `net10.0` project). Target locations when Phase 3 is implemented:

```
src/SchaerbeekMunicipality.Web/DesignSystem/Theme/SchaerbeekTheme.cs
src/SchaerbeekMunicipality.Web/DesignSystem/Tokens/SchaerbeekTokens.cs   (split of the token classes)
src/SchaerbeekMunicipality.Web/wwwroot/app.css
```

Registration in `MainLayout.razor` replaces the current inline theme:

```razor
<MudThemeProvider Theme="SchaerbeekTheme.Default" />
```

## SchaerbeekTheme.cs

```csharp
using MudBlazor;

namespace SchaerbeekMunicipality.Web.DesignSystem.Theme;

/// <summary>
/// The official Schaerbeek municipal theme, derived from the design language of www.1030.be.
/// Register once in MainLayout: &lt;MudThemeProvider Theme="SchaerbeekTheme.Default" /&gt;.
/// </summary>
public static class SchaerbeekTheme
{
    public static MudTheme Default { get; } = new()
    {
        PaletteLight = BuildLightPalette(),
        PaletteDark = BuildDarkPalette(),
        Typography = BuildTypography(),
        LayoutProperties = BuildLayout(),
        Shadows = BuildShadows(),
        ZIndex = new ZIndex(),
        PseudoCss = new PseudoCss(),
    };

    private static PaletteLight BuildLightPalette() => new()
    {
        // Brand
        Primary = SchaerbeekColors.Primary,
        PrimaryDarken = SchaerbeekColors.PrimaryDarken,
        PrimaryLighten = SchaerbeekColors.PrimaryLighten,
        PrimaryContrastText = SchaerbeekColors.White,
        Secondary = SchaerbeekColors.Secondary,
        SecondaryDarken = SchaerbeekColors.SecondaryDarken,
        SecondaryLighten = SchaerbeekColors.SecondaryLighten,
        SecondaryContrastText = SchaerbeekColors.White,
        Tertiary = SchaerbeekColors.Accent,
        TertiaryContrastText = SchaerbeekColors.TextPrimary,

        // Semantic
        Info = SchaerbeekColors.Info,
        InfoContrastText = SchaerbeekColors.White,
        Success = SchaerbeekColors.Success,
        SuccessContrastText = SchaerbeekColors.White,
        Warning = SchaerbeekColors.Warning,
        WarningContrastText = SchaerbeekColors.White,
        Error = SchaerbeekColors.Error,
        ErrorContrastText = SchaerbeekColors.White,

        // Text
        TextPrimary = SchaerbeekColors.TextPrimary,
        TextSecondary = SchaerbeekColors.TextSecondary,
        TextDisabled = SchaerbeekColors.TextDisabled,

        // Surfaces
        Background = SchaerbeekColors.Background,
        BackgroundGray = SchaerbeekColors.SurfaceSunken,
        Surface = SchaerbeekColors.Surface,

        // App frame — blue bar, white drawer, mirroring the 1030.be header/footer
        AppbarBackground = SchaerbeekColors.Primary,
        AppbarText = SchaerbeekColors.White,
        DrawerBackground = SchaerbeekColors.Surface,
        DrawerText = SchaerbeekColors.TextPrimary,
        DrawerIcon = SchaerbeekColors.Primary,

        // Lines and tables
        LinesDefault = SchaerbeekColors.BorderDefault,
        LinesInputs = SchaerbeekColors.BorderInput,
        Divider = SchaerbeekColors.BorderDefault,
        DividerLight = SchaerbeekColors.BorderLight,
        TableLines = SchaerbeekColors.BorderDefault,
        TableStriped = SchaerbeekColors.SurfaceStriped,
        TableHover = SchaerbeekColors.SurfaceHover,

        // Actions
        ActionDefault = SchaerbeekColors.TextSecondary,
        ActionDisabled = SchaerbeekColors.TextDisabled,
        ActionDisabledBackground = SchaerbeekColors.SurfaceSunken,

        // Greys
        GrayDefault = "#707070",
        GrayLight = "#DADADA",
        GrayLighter = "#F3F3F3",
        GrayDark = "#444444",
        GrayDarker = "#202020",

        HoverOpacity = 0.06,
    };

    // Dark palette is provided for completeness (officer preference / low-light desks).
    // It reuses the brand hues but lightened to keep AA contrast on dark surfaces.
    private static PaletteDark BuildDarkPalette() => new()
    {
        Primary = "#4D9FCC",
        PrimaryContrastText = "#0B1B26",
        Secondary = "#26C6B5",
        SecondaryContrastText = "#0B1B26",
        Tertiary = SchaerbeekColors.Accent,
        TertiaryContrastText = "#0B1B26",
        Info = "#5CA8D8",
        Success = "#7CB342",
        Warning = "#EE9A3B",
        Error = "#E57373",
        Background = "#14181B",
        Surface = "#1E2428",
        AppbarBackground = "#10222E",
        AppbarText = "#E6ECF0",
        DrawerBackground = "#1E2428",
        DrawerText = "#E6ECF0",
        TextPrimary = "#E6ECF0",
        TextSecondary = "#AAB4BC",
        TextDisabled = "#5C666E",
        LinesDefault = "#39424A",
        Divider = "#39424A",
        TableStriped = "#232A2F",
    };

    private static Typography BuildTypography() => new()
    {
        Default = new DefaultTypography
        {
            FontFamily = SchaerbeekTypography.FontFamily,
            FontSize = "1rem",          // 16px — matches 1030.be body copy
            FontWeight = "400",
            LineHeight = "1.5",
            LetterSpacing = "normal",
        },
        H1 = new H1Typography
        {
            FontSize = "2rem",          // 32px / 48px line — 1030.be h1
            FontWeight = "700",
            LineHeight = "1.5",
            LetterSpacing = "normal",
        },
        H2 = new H2Typography
        {
            FontSize = "1.5rem",        // 24px / 36px line — 1030.be h2
            FontWeight = "700",
            LineHeight = "1.5",
            LetterSpacing = "normal",
        },
        H3 = new H3Typography
        {
            FontSize = "1.25rem",       // 20px / 30px line — 1030.be h3
            FontWeight = "700",
            LineHeight = "1.5",
            LetterSpacing = "normal",
        },
        H4 = new H4Typography
        {
            FontSize = "1.125rem",
            FontWeight = "700",
            LineHeight = "1.5",
            LetterSpacing = "normal",
        },
        H5 = new H5Typography
        {
            FontSize = "1rem",
            FontWeight = "700",
            LineHeight = "1.5",
            LetterSpacing = "normal",
        },
        H6 = new H6Typography
        {
            FontSize = "0.875rem",
            FontWeight = "700",
            LineHeight = "1.5",
            LetterSpacing = "0.02em",   // small caps-style section labels
        },
        Subtitle1 = new Subtitle1Typography
        {
            FontSize = "1rem",
            FontWeight = "600",
            LineHeight = "1.5",
        },
        Subtitle2 = new Subtitle2Typography
        {
            FontSize = "0.875rem",
            FontWeight = "600",
            LineHeight = "1.5",
        },
        Body1 = new Body1Typography
        {
            FontSize = "1rem",
            FontWeight = "400",
            LineHeight = "1.5",
        },
        Body2 = new Body2Typography
        {
            FontSize = "0.875rem",      // dense table / metadata text
            FontWeight = "400",
            LineHeight = "1.5",
        },
        Button = new ButtonTypography
        {
            FontSize = "0.9375rem",
            FontWeight = "700",         // 1030.be buttons are always bold
            LineHeight = "1.75",
            LetterSpacing = "0.01em",
            TextTransform = "none",     // sentence case, never SHOUTING — matches the site
        },
        Caption = new CaptionTypography
        {
            FontSize = "0.75rem",
            FontWeight = "400",
            LineHeight = "1.4",
        },
        Overline = new OverlineTypography
        {
            FontSize = "0.75rem",
            FontWeight = "700",
            LineHeight = "1.4",
            LetterSpacing = "0.08em",
            TextTransform = "uppercase",
        },
    };

    private static LayoutProperties BuildLayout() => new()
    {
        // 1030.be mixes square buttons with 0.3rem inputs; 4px is the single
        // compromise radius that keeps the sober government look everywhere.
        DefaultBorderRadius = "4px",
        AppbarHeight = "64px",
        DrawerWidthLeft = "260px",
        DrawerWidthRight = "320px",
        DrawerMiniWidthLeft = "56px",
    };

    private static Shadow BuildShadows()
    {
        var shadows = new Shadow();
        // 1030.be uses two soft ambient shadows; everything else stays flat.
        shadows.Elevation[1] = "0 0 0.5rem rgba(17, 17, 17, 0.10)";       // resting cards
        shadows.Elevation[2] = "0 0.25rem 1rem rgba(17, 17, 17, 0.10)";   // hover / raised
        shadows.Elevation[3] = "0 0.3rem 1.5rem rgba(0, 0, 0, 0.15)";     // overlays, sticky bars
        return shadows;
    }
}
```

## Design token classes

```csharp
using MudBlazor;

namespace SchaerbeekMunicipality.Web.DesignSystem.Tokens;

/// <summary>
/// Raw color values shared by the palette and the design tokens.
/// Never hardcode these hex values in components — reference the token.
/// </summary>
public static class SchaerbeekColors
{
    // Brand — extracted from www.1030.be
    public const string Primary = "#00537F";         // Schaerbeek blue
    public const string PrimaryDarken = "#003A59";
    public const string PrimaryLighten = "#2E7BA6";
    public const string Secondary = "#087F74";       // turquoise, darkened for AA text on white
    public const string SecondaryDarken = "#06655D";
    public const string SecondaryLighten = "#08B0A0"; // original site turquoise (focus ring, chips)
    public const string Accent = "#FDC300";          // Schaerbeek yellow — highlights only

    // Semantic
    public const string Success = "#2E7D32";
    public const string SuccessBackground = "#E6EFE9";
    public const string Warning = "#B75500";         // 1030.be orange #EE743B darkened for AA
    public const string WarningBackground = "#FBE9DD";
    public const string Error = "#9A0D20";           // 1030.be form error red
    public const string ErrorBackground = "#F9DCE5"; // 1030.be form error background
    public const string Info = "#0B6BA8";
    public const string InfoBackground = "#E1E9F4";

    // Focus — the site outlines every focused element with this turquoise
    public const string Focus = "#08B0A0";

    // Text
    public const string TextPrimary = "#111111";
    public const string TextSecondary = "#444444";
    public const string TextMuted = "#707070";
    public const string TextDisabled = "#AEAEAE";
    public const string TextOnPrimary = "#FFFFFF";

    // Surfaces
    public const string Background = "#F5F6F7";
    public const string Surface = "#FFFFFF";
    public const string SurfaceSunken = "#F3F3F3";
    public const string SurfaceStriped = "#F8F9FA";
    public const string SurfaceHover = "#EDF3F7";

    // Borders
    public const string BorderDefault = "#DADADA";
    public const string BorderLight = "#ECECEC";
    public const string BorderInput = "#AEAEAE";
    public const string BorderStrong = "#707070";

    public const string White = "#FFFFFF";
}

/// <summary>Typography tokens.</summary>
public static class SchaerbeekTypography
{
    public static readonly string[] FontFamily =
        ["Open Sans", "Segoe UI", "Helvetica Neue", "Arial", "sans-serif"];

    public const string WeightRegular = "400";
    public const string WeightSemiBold = "600";
    public const string WeightBold = "700";
}

/// <summary>Spacing scale in pixels. MudBlazor utility classes (pa-*, ma-*, gap-*) step in 4px units.</summary>
public static class SchaerbeekSpacing
{
    public const int Xxs = 2;
    public const int Xs = 4;
    public const int Sm = 8;
    public const int Md = 16;
    public const int Lg = 24;
    public const int Xl = 32;
    public const int Xxl = 48;
    public const int Section = 64;

    /// <summary>Default gutter between grid items (MudGrid Spacing="3" = 12px).</summary>
    public const int GridSpacing = 3;
}

/// <summary>Layout and container tokens.</summary>
public static class SchaerbeekLayout
{
    public const string ContentMaxWidth = "1280px";   // 1030.be container
    public const string FormMaxWidth = "760px";       // readable single-column forms
    public const string DetailPanelWidth = "320px";
    public const MaxWidth PageMaxWidth = MaxWidth.ExtraLarge;
}

/// <summary>Motion tokens — 1030.be favors calm 200–500ms ease transitions.</summary>
public static class SchaerbeekMotion
{
    public const int FastMs = 150;      // hover feedback
    public const int NormalMs = 250;    // expansion, drawer
    public const int SlowMs = 400;      // page-level, dialogs
    public const string Easing = "ease-in-out";
}

/// <summary>Elevation tokens — use these levels only.</summary>
public static class SchaerbeekElevation
{
    public const int Flat = 0;      // default: tables, forms, panels inside pages
    public const int Card = 1;      // resting cards, app bar
    public const int Raised = 2;    // hover states, drawer
    public const int Overlay = 3;   // dialogs, menus, sticky action bars
}
```

> Note: when splitting tokens into `DesignSystem/Tokens/`, the theme file needs `using SchaerbeekMunicipality.Web.DesignSystem.Tokens;`. `SchaerbeekLayout` requires `using MudBlazor;` for the `MaxWidth` enum.

## app.css

Complete replacement for the current `wwwroot/app.css`. Only rules that **cannot** be expressed through `MudTheme` are included; every rule is annotated with its reason.

```css
/* ============================================================
   Schaerbeek Design System — global CSS
   Everything expressible via MudTheme lives in SchaerbeekTheme.cs.
   Each rule below exists because MudTheme has no knob for it.
   ============================================================ */

/* 1. Font loading. MudTheme can name a font family but cannot load one.
      Self-hosted Open Sans (400/600/700) — no CDN dependency for an
      internal government tool. */
@font-face {
    font-family: "Open Sans";
    src: url("fonts/open-sans-v43-latin-regular.woff2") format("woff2");
    font-weight: 400;
    font-style: normal;
    font-display: swap;
}

@font-face {
    font-family: "Open Sans";
    src: url("fonts/open-sans-v43-latin-600.woff2") format("woff2");
    font-weight: 600;
    font-style: normal;
    font-display: swap;
}

@font-face {
    font-family: "Open Sans";
    src: url("fonts/open-sans-v43-latin-700.woff2") format("woff2");
    font-weight: 700;
    font-style: normal;
    font-display: swap;
}

/* 2. Global focus indicator — the signature 1030.be accessibility trait
      (site: body *:focus { outline: 3px solid #08B0A0 }). We scope it to
      :focus-visible so mouse clicks don't flash rings, and add offset so
      the ring never melts into fills. MudTheme has no focus-ring concept. */
:focus-visible {
    outline: 3px solid #08B0A0;
    outline-offset: 2px;
    border-radius: 2px;
}

/* 3. Skip link — first focusable element in MainLayout; visually hidden
      until focused. Standard technique; no MudBlazor equivalent. */
.skip-link {
    position: absolute;
    left: -999px;
    top: 0;
    z-index: 2000;
    padding: 8px 16px;
    background: #00537F;
    color: #FFFFFF;
    text-decoration: none;
    font-weight: 700;
}

.skip-link:focus {
    left: 8px;
    top: 8px;
}

/* 4. Blazor form validation classes — Blazor emits these outside MudBlazor's
      control. Aligned to the design-system error tokens (site's exact
      validation red/pink) instead of the template defaults. */
.valid.modified:not([type=checkbox]) {
    outline: 1px solid #2E7D32;
}

.invalid {
    outline: 1px solid #9A0D20;
}

.validation-message {
    color: #9A0D20;
    font-size: 0.875rem;
    padding-top: 4px;
}

/* 5. Blazor framework error UI (unhandled exception bar + error boundary).
      Framework-generated markup, not themeable via MudTheme. Restyled to
      the design system's error tokens instead of Bootstrap yellow. */
#blazor-error-ui {
    display: none;
    position: fixed;
    bottom: 0;
    left: 0;
    right: 0;
    z-index: 2100;
    background: #F9DCE5;
    color: #9A0D20;
    border-top: 2px solid #9A0D20;
    padding: 12px 24px;
    font-weight: 600;
}

#blazor-error-ui .dismiss {
    cursor: pointer;
    position: absolute;
    right: 16px;
    top: 12px;
}

.blazor-error-boundary {
    background: #F9DCE5;
    color: #9A0D20;
    border: 1px solid #9A0D20;
    border-radius: 4px;
    padding: 16px;
}

.blazor-error-boundary::after {
    content: "An error has occurred.";
}

/* 6. Reduced motion — accessibility requirement; MudTheme cannot express
      media queries. Disables non-essential animation for users who ask. */
@media (prefers-reduced-motion: reduce) {
    *,
    *::before,
    *::after {
        animation-duration: 0.01ms !important;
        animation-iteration-count: 1 !important;
        transition-duration: 0.01ms !important;
        scroll-behavior: auto !important;
    }
}

/* 7. Print — certificates and case summaries are printed at the desk
      (Phase 8). Hide app chrome; keep content. CSS-only concern. */
@media print {
    .mud-appbar,
    .mud-drawer,
    .app-footer,
    .no-print {
        display: none !important;
    }

    .mud-main-content {
        padding: 0 !important;
    }
}
```

### Removed from the current app.css (and why)

| Current rule | Fate |
|--------------|------|
| `h1:focus { outline: none }` | Removed — violates the focus rule; the skip-link target uses `:focus-visible` semantics instead |
| Bootstrap `.form-floating`, `.darker-border-checkbox` rules | Removed — Bootstrap remnants from the Blazor template; all inputs are MudBlazor |
| `#26b050` / `#e50000` validation colors | Replaced by token colors (`#2E7D32` / `#9A0D20`) |
| Base64 yellow-triangle error boundary | Replaced by token-styled boundary |
| `.registration-cases-header` | Removed — superseded by the `AppPageHeader` wrapper (Phase 3 retrofit) |
