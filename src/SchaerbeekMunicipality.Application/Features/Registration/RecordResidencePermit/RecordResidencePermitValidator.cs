using FluentValidation;

namespace SchaerbeekMunicipality.Application.Features.Registration.RecordResidencePermit;

public sealed class RecordResidencePermitValidator : AbstractValidator<RecordResidencePermitRequest>
{
    public RecordResidencePermitValidator()
    {
        RuleFor(x => x.PermitType)
            .IsInEnum()
            .WithMessage("A valid permit type is required.");

        RuleFor(x => x.ValidFrom)
            .LessThanOrEqualTo(x => x.ValidUntil)
            .WithMessage("Valid-from date must be on or before valid-until date.");

        RuleFor(x => x.ValidUntil)
            .GreaterThan(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("The residence permit must not be expired.");
    }
}