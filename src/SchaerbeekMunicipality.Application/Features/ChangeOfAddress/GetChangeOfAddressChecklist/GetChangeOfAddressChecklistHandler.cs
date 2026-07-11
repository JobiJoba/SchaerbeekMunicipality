using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Application.Features.ChangeOfAddress.GetChangeOfAddressCase;

namespace SchaerbeekMunicipality.Application.Features.ChangeOfAddress.GetChangeOfAddressChecklist;

public sealed class GetChangeOfAddressChecklistHandler(
    GetChangeOfAddressCaseHandler getCaseHandler)
{
    public async Task<ChangeOfAddressChecklistResponse?> Handle(
        ChangeOfAddressCaseId caseId,
        CancellationToken cancellationToken)
    {
        var detail = await getCaseHandler.Handle(caseId, cancellationToken);
        return detail is null ? null : ChangeOfAddressChecklistMapper.FromCaseDetail(detail);
    }
}
