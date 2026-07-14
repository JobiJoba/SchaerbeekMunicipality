using FluentValidation;

namespace SchaerbeekMunicipality.Application.Features.Registration.ResolveDuplicateInvestigation;

public sealed class ResolveDuplicateInvestigationRequestValidator
    : AbstractValidator<ResolveDuplicateInvestigationRequest>
{
    public ResolveDuplicateInvestigationRequestValidator()
    {
        RuleFor(x => x.Notes)
            .MaximumLength(2000);
    }
}