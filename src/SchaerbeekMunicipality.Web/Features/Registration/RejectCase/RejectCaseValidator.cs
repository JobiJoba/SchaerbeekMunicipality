using FluentValidation;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Web.Features.Registration.RejectCase;

public sealed class RejectCaseValidator : AbstractValidator<RejectCaseRequest>
{
    public RejectCaseValidator()
    {
        RuleFor(x => x.Reason).IsInEnum();
        RuleFor(x => x.Notes).MaximumLength(2000);
    }
}
