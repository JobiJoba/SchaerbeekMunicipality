using FluentValidation;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.Registration.OpenRegistrationCase;

public sealed class OpenRegistrationCaseValidator : AbstractValidator<OpenRegistrationCaseRequest>
{
    public OpenRegistrationCaseValidator()
    {
        RuleFor(x => x.VisitReason)
            .IsInEnum()
            .WithMessage("A valid visit reason is required.");
    }
}
