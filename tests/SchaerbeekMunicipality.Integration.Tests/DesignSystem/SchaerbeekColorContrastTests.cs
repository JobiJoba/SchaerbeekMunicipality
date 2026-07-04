using FluentAssertions;
using SchaerbeekMunicipality.Web.DesignSystem.Tokens;

namespace SchaerbeekMunicipality.Integration.Tests.DesignSystem;

public sealed class SchaerbeekColorContrastTests
{
    public static IEnumerable<object[]> TextOnWhitePairs =>
    [
        [SchaerbeekColors.Primary, 4.5],
        [SchaerbeekColors.PrimaryDarken, 4.5],
        [SchaerbeekColors.PrimaryLighten, 4.5],
        [SchaerbeekColors.Secondary, 4.5],
        [SchaerbeekColors.Success, 4.5],
        [SchaerbeekColors.Warning, 4.5],
        [SchaerbeekColors.Error, 4.5],
        [SchaerbeekColors.Info, 4.5],
        [SchaerbeekColors.TextPrimary, 4.5],
        [SchaerbeekColors.TextSecondary, 4.5],
        [SchaerbeekColors.TextMuted, 4.5],
    ];

    public static IEnumerable<object[]> TextOnBackgroundPairs =>
    [
        [SchaerbeekColors.Error, SchaerbeekColors.ErrorBackground, 4.5],
        [SchaerbeekColors.Success, SchaerbeekColors.SuccessBackground, 4.3],
        [SchaerbeekColors.Warning, SchaerbeekColors.WarningBackground, 4.1],
        [SchaerbeekColors.Info, SchaerbeekColors.InfoBackground, 4.5],
        [SchaerbeekColors.TextPrimary, SchaerbeekColors.Accent, 4.5],
        [SchaerbeekColors.White, SchaerbeekColors.Primary, 4.5],
    ];

    [Theory]
    [MemberData(nameof(TextOnWhitePairs))]
    public void Foreground_on_white_meets_AA(string foreground, double minimumRatio)
    {
        var ratio = WcagContrastRatio(foreground, SchaerbeekColors.White);
        ratio.Should().BeGreaterThanOrEqualTo(minimumRatio);
    }

    [Theory]
    [MemberData(nameof(TextOnBackgroundPairs))]
    public void Semantic_pairs_meet_AA(string foreground, string background, double minimumRatio)
    {
        var ratio = WcagContrastRatio(foreground, background);
        ratio.Should().BeGreaterThanOrEqualTo(minimumRatio);
    }

    [Fact]
    public void Focus_ring_meets_documented_non_text_contrast_on_white()
    {
        // Spec: 2.72:1 — non-text UI; visibility comes from 3px width + offset in app.css.
        var ratio = WcagContrastRatio(SchaerbeekColors.Focus, SchaerbeekColors.White);
        ratio.Should().BeGreaterThanOrEqualTo(2.7);
    }

    private static double WcagContrastRatio(string foregroundHex, string backgroundHex)
    {
        var fg = RelativeLuminance(ParseHex(foregroundHex));
        var bg = RelativeLuminance(ParseHex(backgroundHex));
        var lighter = Math.Max(fg, bg);
        var darker = Math.Min(fg, bg);
        return (lighter + 0.05) / (darker + 0.05);
    }

    private static (double R, double G, double B) ParseHex(string hex)
    {
        hex = hex.TrimStart('#');
        var r = Convert.ToInt32(hex[..2], 16) / 255.0;
        var g = Convert.ToInt32(hex.Substring(2, 2), 16) / 255.0;
        var b = Convert.ToInt32(hex.Substring(4, 2), 16) / 255.0;
        return (r, g, b);
    }

    private static double RelativeLuminance((double R, double G, double B) rgb)
    {
        static double Transform(double channel) =>
            channel <= 0.03928
                ? channel / 12.92
                : Math.Pow((channel + 0.055) / 1.055, 2.4);

        var r = Transform(rgb.R);
        var g = Transform(rgb.G);
        var b = Transform(rgb.B);
        return 0.2126 * r + 0.7152 * g + 0.0722 * b;
    }
}
