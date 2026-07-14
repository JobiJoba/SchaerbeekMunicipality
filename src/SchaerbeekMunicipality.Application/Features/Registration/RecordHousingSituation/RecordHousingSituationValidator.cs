using FluentValidation;

namespace SchaerbeekMunicipality.Application.Features.Registration.RecordHousingSituation;

public sealed class RecordHousingSituationValidator : AbstractValidator<RecordHousingSituationRequest>
{
    public RecordHousingSituationValidator()
    {
        RuleFor(x => x.Situation)
            .IsInEnum()
            .WithMessage("A valid housing situation is required.");
    }
}