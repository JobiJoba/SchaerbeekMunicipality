using FluentValidation;

namespace SchaerbeekMunicipality.Application.Features.Registration.ApproveCase;

public sealed class ApproveCaseValidator : AbstractValidator<ApproveCaseRequest>
{
    public ApproveCaseValidator()
    {
        RuleFor(x => x.RegisterTarget).IsInEnum();
    }
}