using FluentValidation;

namespace SchaerbeekMunicipality.Application.Features.Registration.RejectCase;

public sealed class RejectCaseValidator : AbstractValidator<RejectCaseRequest>
{
    public RejectCaseValidator()
    {
        RuleFor(x => x.Reason).IsInEnum();
        RuleFor(x => x.Notes).MaximumLength(2000);
    }
}