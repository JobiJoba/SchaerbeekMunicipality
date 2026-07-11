using FluentValidation;

namespace SchaerbeekMunicipality.Web.Features.Registration.RecordHouseholdComposition;

public sealed class RecordHouseholdCompositionValidator : AbstractValidator<RecordHouseholdCompositionRequest>
{
    public RecordHouseholdCompositionValidator()
    {
        RuleForEach(x => x.Members).SetValidator(new HouseholdMemberRequestValidator());
    }
}
