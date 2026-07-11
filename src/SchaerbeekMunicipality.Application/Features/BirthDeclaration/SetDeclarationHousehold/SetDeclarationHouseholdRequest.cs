using FluentValidation;
using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.Application.Validation;

namespace SchaerbeekMunicipality.Application.Features.BirthDeclaration.SetDeclarationHousehold;

public sealed record SetDeclarationHouseholdRequest(
    string Street,
    string HouseNumber,
    string? Box,
    string PostalCode,
    string Municipality);

public sealed class SetDeclarationHouseholdValidator : AbstractValidator<SetDeclarationHouseholdRequest>
{
    public SetDeclarationHouseholdValidator()
    {
        RuleFor(x => x.Street).BelgianStreet();
        RuleFor(x => x.HouseNumber).BelgianHouseNumber();
        RuleFor(x => x.PostalCode).BelgianPostalCode();
        RuleFor(x => x.Municipality).BelgianMunicipality();
    }
}
