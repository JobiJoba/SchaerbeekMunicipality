using FluentValidation;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Web.Features.Registration.ApproveCase;

public sealed class ApproveCaseValidator : AbstractValidator<ApproveCaseRequest>
{
    public ApproveCaseValidator()
    {
        RuleFor(x => x.RegisterTarget).IsInEnum();
    }
}
