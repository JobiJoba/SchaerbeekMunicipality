using FluentValidation;

namespace SchaerbeekMunicipality.Application.Features.ChangeOfAddress.UpdateHouseholdForMove;

public sealed record UpdateHouseholdForMoveRequest(Guid PersonId);

public sealed class UpdateHouseholdForMoveValidator : AbstractValidator<UpdateHouseholdForMoveRequest>
{
    public UpdateHouseholdForMoveValidator()
    {
        RuleFor(x => x.PersonId).NotEmpty();
    }
}

public sealed record UpdateHouseholdForMoveResponse(Guid CaseId, Guid PersonId);
