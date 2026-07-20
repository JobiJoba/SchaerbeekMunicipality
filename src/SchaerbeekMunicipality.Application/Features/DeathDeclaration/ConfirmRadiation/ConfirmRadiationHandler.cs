using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.DeathDeclaration;
using SchaerbeekMunicipality.Domain.Events;
using SchaerbeekMunicipality.Domain.Household;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Infrastructure.Events;

namespace SchaerbeekMunicipality.Application.Features.DeathDeclaration.ConfirmRadiation;

public sealed record ConfirmRadiationResponse(
    Guid CaseId,
    string Status,
    Guid PersonId,
    DateOnly DeathDate);

public sealed class ConfirmRadiationHandler(
    DeathDeclarationCaseGuard caseGuard,
    IDeathDeclarationCaseRepository caseRepository,
    IPersonRepository personRepository,
    IHouseholdRepository householdRepository,
    IDomainEventDispatcher domainEventDispatcher,
    ICurrentOfficer currentOfficer,
    TimeProvider timeProvider)
{
    public async Task<ConfirmRadiationResponse> Handle(
        DeathDeclarationCaseId caseId,
        CancellationToken cancellationToken)
    {
        if (!currentOfficer.CanApproveRegistration)
            throw new UnauthorizedAccessException("Only population officers can confirm death declarations.");

        var deathDeclarationCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(ConfirmRadiation),
            cancellationToken);

        if (deathDeclarationCase.DeathDate is null)
            throw new InvalidDeathDeclarationTransitionException(
                "Death facts must be recorded before confirmation.");

        var person = await personRepository.GetForUpdateAsync(deathDeclarationCase.PersonId, cancellationToken)
                     ?? throw new KeyNotFoundException($"Person '{deathDeclarationCase.PersonId}' was not found.");

        person.MarkDeceased(deathDeclarationCase.DeathDate.Value);
        person.ClearDomicile();

        var households = await householdRepository.ListByMemberIdentityAsync(
            person.GivenName,
            person.FamilyName,
            person.BirthDate,
            cancellationToken);

        foreach (var household in households)
            household.RemoveMemberMatching(person.GivenName, person.FamilyName, person.BirthDate);

        var confirmedAt = timeProvider.GetUtcNow();
        var eventDetails = deathDeclarationCase.ConfirmRadiation(confirmedAt);

        await domainEventDispatcher.DispatchAsync(
            new PersonRadiated(
                eventDetails.CaseId,
                eventDetails.PersonId,
                eventDetails.DeathDate,
                eventDetails.OccurredAt),
            cancellationToken);

        await caseRepository.SaveChangesAsync(cancellationToken);

        return new ConfirmRadiationResponse(
            deathDeclarationCase.Id.Value,
            deathDeclarationCase.Status.ToString(),
            person.Id.Value,
            eventDetails.DeathDate);
    }
}
