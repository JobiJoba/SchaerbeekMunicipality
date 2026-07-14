using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Domain.Identity;

namespace SchaerbeekMunicipality.Application.Features.ChangeOfAddress.UpdateHouseholdForMove;

public sealed record UnlinkHouseholdMemberResponse(Guid CaseId, Guid PersonId);

public sealed class UnlinkHouseholdMemberHandler(
    ChangeOfAddressCaseGuard caseGuard,
    IChangeOfAddressCaseRepository caseRepository)
{
    public async Task<UnlinkHouseholdMemberResponse> Handle(
        ChangeOfAddressCaseId caseId,
        PersonId personId,
        CancellationToken cancellationToken)
    {
        var changeOfAddressCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(UnlinkHouseholdMemberHandler),
            cancellationToken);

        changeOfAddressCase.RemoveHouseholdMember(personId);
        await caseRepository.SaveChangesAsync(cancellationToken);

        return new UnlinkHouseholdMemberResponse(
            changeOfAddressCase.Id.Value,
            personId.Value);
    }
}