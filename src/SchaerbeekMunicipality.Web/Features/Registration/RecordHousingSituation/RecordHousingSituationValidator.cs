using FluentValidation;

namespace SchaerbeekMunicipality.Web.Features.Registration.RecordHousingSituation;

public sealed class RecordHousingSituationValidator : AbstractValidator<RecordHousingSituationRequest>
{
    public RecordHousingSituationValidator()
    {
        RuleFor(x => x.Situation)
            .IsInEnum()
            .WithMessage("A valid housing situation is required.");
    }
}
