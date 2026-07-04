using FluentValidation;
using SchaerbeekMunicipality.Domain.Address;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Web.Features.Registration.DeclareAddress;

public sealed class DeclareAddressHandler(
    IRegistrationCaseRepository caseRepository,
    IValidator<DeclareAddressRequest> validator)
{
    public async Task<DeclareAddressResponse> Handle(
        RegistrationCaseId caseId,
        DeclareAddressRequest request,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var registrationCase = await caseRepository.GetByIdAsync(caseId, cancellationToken)
            ?? throw new KeyNotFoundException($"Registration case '{caseId}' was not found.");

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
