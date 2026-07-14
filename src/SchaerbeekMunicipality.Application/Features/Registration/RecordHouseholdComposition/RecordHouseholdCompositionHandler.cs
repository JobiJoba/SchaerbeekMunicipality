using FluentValidation;
using SchaerbeekMunicipality.Domain.Household;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.Registration.RecordHouseholdComposition;

public sealed class RecordHouseholdCompositionHandler(
    RegistrationCaseGuard caseGuard,
    IHouseholdRepository householdRepository,
    IValidator<RecordHouseholdCompositionRequest> validator)
{
    public async Task<RecordHouseholdCompositionResponse> Handle(
        RegistrationCaseId caseId,
        RecordHouseholdCompositionRequest request,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var registrationCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(RecordHouseholdComposition),
            cancellationToken);

        registrationCase.EnsureIntakeDataEditable(nameof(RecordHouseholdComposition));

        if (!registrationCase.Checklist.IdentityEstablished)
            throw new InvalidRegistrationTransitionException(
                "Identity must be recorded before household composition can be captured.");

        if (registrationCase.DeclaredAddress is null)
            throw new InvalidRegistrationTransitionException(
                "Address must be declared before recording household composition.");

        var memberDetails = request.Members
            .Select(m => new HouseholdMemberDetails(
                m.GivenName,
                m.FamilyName,
                m.BirthDate,
                m.Role))
            .ToList();

        var existingHousehold = await householdRepository.GetByCaseIdAsync(caseId, cancellationToken);

        Household household;
        if (existingHousehold is null)
        {
            household = Household.Create(caseId);
            household.SetComposition(memberDetails);
            await householdRepository.AddAsync(household, cancellationToken);
        }
        else
        {
            existingHousehold.SetComposition(memberDetails);
            household = existingHousehold;
        }

        await householdRepository.SaveChangesAsync(cancellationToken);

        return new RecordHouseholdCompositionResponse(
            caseId.Value,
            household.Members
                .Select(m => new HouseholdMemberResponse(
                    m.Id.Value,
                    m.GivenName,
                    m.FamilyName,
                    m.BirthDate,
                    m.Role))
                .ToList());
    }
}