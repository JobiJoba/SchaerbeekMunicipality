using FluentValidation;

namespace SchaerbeekMunicipality.Application.Features.Registration.SetResidenceCategory;

public sealed class SetResidenceCategoryValidator : AbstractValidator<SetResidenceCategoryRequest>
{
    public SetResidenceCategoryValidator()
    {
        RuleFor(x => x.Category)
            .IsInEnum()
            .WithMessage("A valid residence category is required.");
    }
}
