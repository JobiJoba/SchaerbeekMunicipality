using FluentValidation;
using SchaerbeekMunicipality.Domain.BirthDeclaration;

namespace SchaerbeekMunicipality.Web.Features.BirthDeclaration.SetDeclarationHousehold;

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
        RuleFor(x => x.Street).NotEmpty().MaximumLength(256);
        RuleFor(x => x.HouseNumber).NotEmpty().MaximumLength(16);
        RuleFor(x => x.PostalCode).NotEmpty().Length(4);
        RuleFor(x => x.Municipality).NotEmpty().MaximumLength(128);
    }
}
