using FluentValidation;

namespace SchaerbeekMunicipality.Application.Features.Registration.SearchNationalRegister;

public sealed class SearchNationalRegisterValidator : AbstractValidator<SearchNationalRegisterRequest>
{
    public SearchNationalRegisterValidator()
    {
        RuleFor(x => x)
            .Must(x =>
                !string.IsNullOrWhiteSpace(x.GivenName) ||
                !string.IsNullOrWhiteSpace(x.FamilyName) ||
                x.BirthDate.HasValue)
            .WithMessage("Enter at least one search criterion (given name, family name, or date of birth).");

        RuleFor(x => x.BirthDate)
            .LessThan(DateOnly.FromDateTime(DateTime.Today))
            .When(x => x.BirthDate.HasValue);
    }
}
