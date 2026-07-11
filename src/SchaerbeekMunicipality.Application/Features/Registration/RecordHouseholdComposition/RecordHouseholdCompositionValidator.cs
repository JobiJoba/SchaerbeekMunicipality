using FluentValidation;

namespace SchaerbeekMunicipality.Application.Features.Registration.RecordHouseholdComposition;

public sealed class RecordHouseholdCompositionValidator : AbstractValidator<RecordHouseholdCompositionRequest>
{
    public RecordHouseholdCompositionValidator()
    {
        RuleForEach(x => x.Members).SetValidator(new HouseholdMemberRequestValidator());
    }
}
