using FluentValidation;
using SchaerbeekMunicipality.Domain.ReferenceData;

namespace SchaerbeekMunicipality.Web.Validation;

public static class BelgianAddressRules
{
    public static IRuleBuilderOptions<T, string> BelgianStreet<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder
            .NotEmpty()
            .MaximumLength(256);

    public static IRuleBuilderOptions<T, string> BelgianHouseNumber<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder
            .NotEmpty()
            .MaximumLength(16);

    public static IRuleBuilderOptions<T, string> BelgianPostalCode<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder
            .NotEmpty()
            .Length(4);

    public static IRuleBuilderOptions<T, string> BelgianMunicipality<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder
            .NotEmpty()
            .MaximumLength(128);

    public static IRuleBuilderOptions<T, string> SchaerbeekPostalCode<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder
            .Equal(SchaerbeekCommune.PostalCode)
            .WithMessage($"Postal code must be {SchaerbeekCommune.PostalCode} for registration at Schaerbeek.");

    public static IRuleBuilderOptions<T, string> SchaerbeekMunicipality<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder
            .Must(m => m.Trim().Equals(SchaerbeekCommune.Name, StringComparison.OrdinalIgnoreCase))
            .WithMessage($"Municipality must be {SchaerbeekCommune.Name} for registration at this desk.");
}
