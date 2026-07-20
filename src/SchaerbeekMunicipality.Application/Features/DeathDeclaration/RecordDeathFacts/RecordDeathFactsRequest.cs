using FluentValidation;
using SchaerbeekMunicipality.Domain.DeathDeclaration;

namespace SchaerbeekMunicipality.Application.Features.DeathDeclaration.RecordDeathFacts;

public sealed record RecordDeathFactsRequest(
    DateOnly DeathDate,
    string DeathPlace,
    bool DeathAbroad,
    InformantRelationship InformantRelationship);

public sealed class RecordDeathFactsValidator : AbstractValidator<RecordDeathFactsRequest>
{
    public RecordDeathFactsValidator()
    {
        RuleFor(x => x.DeathPlace).NotEmpty().MaximumLength(256);
        RuleFor(x => x.InformantRelationship).IsInEnum();
    }
}
