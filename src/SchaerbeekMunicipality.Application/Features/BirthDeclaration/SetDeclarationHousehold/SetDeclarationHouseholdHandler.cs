using FluentValidation;
using SchaerbeekMunicipality.Domain.Address;
using SchaerbeekMunicipality.Domain.BirthDeclaration;

namespace SchaerbeekMunicipality.Application.Features.BirthDeclaration.SetDeclarationHousehold;

public sealed record SetDeclarationHouseholdResponse(Guid CaseId, string Address, bool HouseholdEstablished);

public sealed class SetDeclarationHouseholdHandler(
    BirthDeclarationCaseGuard caseGuard,
    IBirthDeclarationCaseRepository caseRepository,
    IValidator<SetDeclarationHouseholdRequest> validator)
{
    public async Task<SetDeclarationHouseholdResponse> Handle(
        BirthDeclarationCaseId caseId,
        SetDeclarationHouseholdRequest request,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var birthDeclarationCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(SetDeclarationHousehold),
            cancellationToken);

        var address = BelgianAddress.Create(
            request.Street,
            request.HouseNumber,
            request.Box,
            request.PostalCode,
            request.Municipality);

        birthDeclarationCase.SetHousehold(address);
        await caseRepository.SaveChangesAsync(cancellationToken);

        return new SetDeclarationHouseholdResponse(
            birthDeclarationCase.Id.Value,
            address.FormatSingleLine(),
            birthDeclarationCase.Checklist.HouseholdEstablished);
    }
}
