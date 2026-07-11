using FluentValidation;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.Registration.SuspendCase;

public sealed class SuspendCaseValidator : AbstractValidator<SuspendCaseRequest>
{
    public SuspendCaseValidator()
    {
        RuleFor(x => x.Reason).IsInEnum();
        RuleFor(x => x.Notes).MaximumLength(2000);
    }
}
