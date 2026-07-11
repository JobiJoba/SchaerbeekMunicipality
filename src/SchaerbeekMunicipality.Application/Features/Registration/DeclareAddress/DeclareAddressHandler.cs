using FluentValidation;
using SchaerbeekMunicipality.Domain.Address;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Application.Features.Registration;

namespace SchaerbeekMunicipality.Application.Features.Registration.DeclareAddress;

public sealed class DeclareAddressHandler(
    RegistrationCaseGuard caseGuard,
    IRegistrationCaseRepository caseRepository,
    IValidator<DeclareAddressRequest> validator)
{
    public async Task<DeclareAddressResponse> Handle(
        RegistrationCaseId caseId,
        DeclareAddressRequest request,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var registrationCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(DeclareAddress),
            cancellationToken);

        registrationCase.DeclareAddress(new AddressDetails(
            request.Street,
            request.HouseNumber,
            request.Box,
            request.PostalCode,
            request.Municipality));

        await caseRepository.SaveChangesAsync(cancellationToken);

        return new DeclareAddressResponse(
            registrationCase.Id.Value,
            registrationCase.Checklist.AddressDeclared,
            registrationCase.DeclaredAddress!.FormatSingleLine());
    }
}
