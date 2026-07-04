using MudBlazor;

namespace SchaerbeekMunicipality.Web.DesignSystem.Tokens;

/// <summary>Raw color values shared by the palette and components. Never hardcode these in pages.</summary>
public static class SchaerbeekColors
{
    public const string Primary = "#00537F";
    public const string PrimaryDarken = "#003A59";
    public const string PrimaryLighten = "#2E7BA6";
    public const string Secondary = "#087F74";
    public const string SecondaryDarken = "#06655D";
    public const string SecondaryLighten = "#08B0A0";
    public const string Accent = "#FDC300";

    public const string Success = "#2E7D32";
    public const string SuccessBackground = "#E6EFE9";
    public const string Warning = "#B75500";
    public const string WarningBackground = "#FBE9DD";
    public const string Error = "#9A0D20";
    public const string ErrorBackground = "#F9DCE5";
    public const string Info = "#0B6BA8";
    public const string InfoBackground = "#E1E9F4";

    public const string Focus = "#08B0A0";

    public const string TextPrimary = "#111111";
    public const string TextSecondary = "#444444";
    public const string TextMuted = "#707070";
    public const string TextDisabled = "#AEAEAE";
    public const string TextOnPrimary = "#FFFFFF";

    public const string Background = "#F5F6F7";
    public const string Surface = "#FFFFFF";
    public const string SurfaceSunken = "#F3F3F3";
    public const string SurfaceStriped = "#F8F9FA";
    public const string SurfaceHover = "#EDF3F7";

    public const string BorderDefault = "#DADADA";
    public const string BorderLight = "#ECECEC";
    public const string BorderInput = "#AEAEAE";
    public const string BorderStrong = "#707070";

    public const string White = "#FFFFFF";
}

public static class SchaerbeekTypography
{
    public static readonly string[] FontFamily =
        ["Open Sans", "Segoe UI", "Helvetica Neue", "Arial", "sans-serif"];

    public const string WeightRegular = "400";
    public const string WeightSemiBold = "600";
    public const string WeightBold = "700";
}

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
    public const int GridSpacing = 3;
}

public static class SchaerbeekLayout
{
    public const string ContentMaxWidth = "1280px";
    public const string FormMaxWidth = "760px";
    public const string DetailPanelWidth = "320px";
    public const MaxWidth PageMaxWidth = MaxWidth.ExtraLarge;
}

public static class SchaerbeekMotion
{
    public const int FastMs = 150;
    public const int NormalMs = 250;
    public const int SlowMs = 400;
    public const string Easing = "ease-in-out";
}

public static class SchaerbeekElevation
{
    public const int Flat = 0;
    public const int Card = 1;
    public const int Raised = 2;
    public const int Overlay = 3;
}
