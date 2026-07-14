using MudBlazor;
using SchaerbeekMunicipality.Web.DesignSystem.Tokens;

namespace SchaerbeekMunicipality.Web.DesignSystem.Theme;

/// <summary>
///     The official Schaerbeek municipal theme, derived from the design language of www.1030.be.
///     Register once in MainLayout: &lt;MudThemeProvider Theme="SchaerbeekTheme.Default" /&gt;.
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
        PseudoCss = new PseudoCss()
    };

    private static PaletteLight BuildLightPalette()
    {
        return new PaletteLight
        {
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
            Info = SchaerbeekColors.Info,
            InfoContrastText = SchaerbeekColors.White,
            Success = SchaerbeekColors.Success,
            SuccessContrastText = SchaerbeekColors.White,
            Warning = SchaerbeekColors.Warning,
            WarningContrastText = SchaerbeekColors.White,
            Error = SchaerbeekColors.Error,
            ErrorContrastText = SchaerbeekColors.White,
            TextPrimary = SchaerbeekColors.TextPrimary,
            TextSecondary = SchaerbeekColors.TextSecondary,
            TextDisabled = SchaerbeekColors.TextDisabled,
            Background = SchaerbeekColors.Background,
            BackgroundGray = SchaerbeekColors.SurfaceSunken,
            Surface = SchaerbeekColors.Surface,
            AppbarBackground = SchaerbeekColors.Primary,
            AppbarText = SchaerbeekColors.White,
            DrawerBackground = SchaerbeekColors.Surface,
            DrawerText = SchaerbeekColors.TextPrimary,
            DrawerIcon = SchaerbeekColors.Primary,
            LinesDefault = SchaerbeekColors.BorderDefault,
            LinesInputs = SchaerbeekColors.BorderInput,
            Divider = SchaerbeekColors.BorderDefault,
            DividerLight = SchaerbeekColors.BorderLight,
            TableLines = SchaerbeekColors.BorderDefault,
            TableStriped = SchaerbeekColors.SurfaceStriped,
            TableHover = SchaerbeekColors.SurfaceHover,
            ActionDefault = SchaerbeekColors.TextSecondary,
            ActionDisabled = SchaerbeekColors.TextDisabled,
            ActionDisabledBackground = SchaerbeekColors.SurfaceSunken,
            GrayDefault = "#707070",
            GrayLight = "#DADADA",
            GrayLighter = "#F3F3F3",
            GrayDark = "#444444",
            GrayDarker = "#202020",
            HoverOpacity = 0.06
        };
    }

    private static PaletteDark BuildDarkPalette()
    {
        return new PaletteDark
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
            TableStriped = "#232A2F"
        };
    }

    private static Typography BuildTypography()
    {
        return new Typography
        {
            Default = new DefaultTypography
            {
                FontFamily = SchaerbeekTypography.FontFamily,
                FontSize = "1rem",
                FontWeight = "400",
                LineHeight = "1.5",
                LetterSpacing = "normal"
            },
            H1 = new H1Typography
                { FontSize = "2rem", FontWeight = "700", LineHeight = "1.5", LetterSpacing = "normal" },
            H2 = new H2Typography
                { FontSize = "1.5rem", FontWeight = "700", LineHeight = "1.5", LetterSpacing = "normal" },
            H3 = new H3Typography
                { FontSize = "1.25rem", FontWeight = "700", LineHeight = "1.5", LetterSpacing = "normal" },
            H4 = new H4Typography
                { FontSize = "1.125rem", FontWeight = "700", LineHeight = "1.5", LetterSpacing = "normal" },
            H5 = new H5Typography
                { FontSize = "1rem", FontWeight = "700", LineHeight = "1.5", LetterSpacing = "normal" },
            H6 = new H6Typography
                { FontSize = "0.875rem", FontWeight = "700", LineHeight = "1.5", LetterSpacing = "0.02em" },
            Subtitle1 = new Subtitle1Typography { FontSize = "1rem", FontWeight = "600", LineHeight = "1.5" },
            Subtitle2 = new Subtitle2Typography { FontSize = "0.875rem", FontWeight = "600", LineHeight = "1.5" },
            Body1 = new Body1Typography { FontSize = "1rem", FontWeight = "400", LineHeight = "1.5" },
            Body2 = new Body2Typography { FontSize = "0.875rem", FontWeight = "400", LineHeight = "1.5" },
            Button = new ButtonTypography
            {
                FontSize = "0.9375rem",
                FontWeight = "700",
                LineHeight = "1.75",
                LetterSpacing = "0.01em",
                TextTransform = "none"
            },
            Caption = new CaptionTypography { FontSize = "0.75rem", FontWeight = "400", LineHeight = "1.4" },
            Overline = new OverlineTypography
            {
                FontSize = "0.75rem",
                FontWeight = "700",
                LineHeight = "1.4",
                LetterSpacing = "0.08em",
                TextTransform = "uppercase"
            }
        };
    }

    private static LayoutProperties BuildLayout()
    {
        return new LayoutProperties
        {
            DefaultBorderRadius = "4px",
            AppbarHeight = "64px",
            DrawerWidthLeft = "260px",
            DrawerWidthRight = "320px",
            DrawerMiniWidthLeft = "56px"
        };
    }

    private static Shadow BuildShadows()
    {
        var shadows = new Shadow();
        shadows.Elevation[1] = "0 0 0.5rem rgba(17, 17, 17, 0.10)";
        shadows.Elevation[2] = "0 0.25rem 1rem rgba(17, 17, 17, 0.10)";
        shadows.Elevation[3] = "0 0.3rem 1.5rem rgba(0, 0, 0, 0.15)";
        return shadows;
    }
}