using FluentValidation;
using SchaerbeekMunicipality.Domain.Address;
using SchaerbeekMunicipality.Domain.ChangeOfAddress;

namespace SchaerbeekMunicipality.Application.Features.ChangeOfAddress.DeclareNewAddress;

public sealed class DeclareNewAddressHandler(
    ChangeOfAddressCaseGuard caseGuard,
    IChangeOfAddressCaseRepository caseRepository,
    IValidator<DeclareNewAddressRequest> validator)
{
    public async Task<DeclareNewAddressResponse> Handle(
        ChangeOfAddressCaseId caseId,
        DeclareNewAddressRequest request,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var changeOfAddressCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(DeclareNewAddress),
            cancellationToken);

        var address = BelgianAddress.Create(
            request.Street,
            request.HouseNumber,
            request.Box,
            request.PostalCode,
            request.Municipality);

        changeOfAddressCase.DeclareNewAddress(
            address,
            request.HousingSituation,
            request.EffectiveDate);

        await caseRepository.SaveChangesAsync(cancellationToken);

        return new DeclareNewAddressResponse(
            changeOfAddressCase.Id.Value,
            address.FormatSingleLine(),
            changeOfAddressCase.Checklist.NewAddressDeclared,
            changeOfAddressCase.Checklist.HousingDocumentRequired);
    }
}