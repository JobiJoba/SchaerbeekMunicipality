using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace SchaerbeekMunicipality.Domain.Common;

public static class EnumDisplayExtensions
{
    public static string ToDisplayString(this Enum value)
    {
        var type = value.GetType();
        var name = Enum.GetName(type, value);
        if (string.IsNullOrWhiteSpace(name)) return value.ToString();

        var members = type.GetMember(name);
        if (members.Length > 0)
        {
            var member = members[0];
            var display = member.GetCustomAttribute<DisplayAttribute>();
            if (!string.IsNullOrWhiteSpace(display?.Name)) return display.Name;

            var description = member.GetCustomAttribute<DescriptionAttribute>();
            if (!string.IsNullOrWhiteSpace(description?.Description)) return description.Description;
        }

        return HumanizePascalCase(name);
    }

    private static string HumanizePascalCase(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return input;

        var sb = new StringBuilder(input.Length + 8);

        for (var i = 0; i < input.Length; i++)
        {
            var c = input[i];

            if (i > 0 && char.IsUpper(c))
            {
                var prev = input[i - 1];
                var next = i + 1 < input.Length ? input[i + 1] : '\0';

                var prevIsLowerOrDigit = char.IsLower(prev) || char.IsDigit(prev);
                var boundaryBetweenAcronymAndWord = char.IsUpper(prev) && char.IsLower(next);

                if (prevIsLowerOrDigit || boundaryBetweenAcronymAndWord) sb.Append(' ');
            }

            sb.Append(c);
        }

        if (sb.Length > 0) sb[0] = char.ToUpperInvariant(sb[0]);

        return sb.ToString();
    }
}