using FluentValidation;

namespace SchaerbeekMunicipality.Application.Features.Registration.RecordImmigrationDecision;

public sealed class RecordImmigrationDecisionValidator : AbstractValidator<RecordImmigrationDecisionRequest>
{
    public RecordImmigrationDecisionValidator()
    {
        RuleFor(x => x.ReferenceNumber)
            .NotEmpty()
            .WithMessage("Decision reference number is required.");

        RuleFor(x => x.DecisionDate)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Decision date cannot be in the future.");
    }
}
