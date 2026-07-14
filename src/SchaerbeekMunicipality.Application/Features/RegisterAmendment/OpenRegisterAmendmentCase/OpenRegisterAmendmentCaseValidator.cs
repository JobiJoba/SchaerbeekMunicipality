using FluentValidation;

namespace SchaerbeekMunicipality.Application.Features.RegisterAmendment.OpenRegisterAmendmentCase;

public sealed class OpenRegisterAmendmentCaseValidator : AbstractValidator<OpenRegisterAmendmentCaseRequest>
{
    public OpenRegisterAmendmentCaseValidator()
    {
        RuleFor(r => r.PersonId).NotEmpty();
        RuleFor(r => r.AmendmentType).NotEmpty();
    }
}
