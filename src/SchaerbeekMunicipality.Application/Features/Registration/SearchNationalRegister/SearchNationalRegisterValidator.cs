using FluentValidation;

namespace SchaerbeekMunicipality.Application.Features.Registration.SearchNationalRegister;

public sealed class SearchNationalRegisterValidator : AbstractValidator<SearchNationalRegisterRequest>
{
    public SearchNationalRegisterValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);

        RuleFor(x => x.BirthDate)
            .LessThan(DateOnly.FromDateTime(DateTime.Today))
            .When(x => x.BirthDate.HasValue);
    }
}