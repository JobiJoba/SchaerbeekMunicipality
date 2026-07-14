using FluentValidation;
using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Domain.Identity;

namespace SchaerbeekMunicipality.Application.Features.ChangeOfAddress.UpdateHouseholdForMove;

public sealed class UpdateHouseholdForMoveHandler(
    ChangeOfAddressCaseGuard caseGuard,
    IChangeOfAddressCaseRepository caseRepository,
    IPersonRepository personRepository,
    IValidator<UpdateHouseholdForMoveRequest> validator)
{
    public async Task<UpdateHouseholdForMoveResponse> Handle(
        ChangeOfAddressCaseId caseId,
        UpdateHouseholdForMoveRequest request,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var changeOfAddressCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(UpdateHouseholdForMove),
            cancellationToken);

        var personId = new PersonId(request.PersonId);
        _ = await personRepository.GetByIdAsync(personId, cancellationToken)
            ?? throw new KeyNotFoundException($"Person '{personId}' was not found.");

        changeOfAddressCase.AddHouseholdMember(personId);
        await caseRepository.SaveChangesAsync(cancellationToken);

        return new UpdateHouseholdForMoveResponse(
            changeOfAddressCase.Id.Value,
            personId.Value);
    }
}