using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.Domain.Events;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.NationalRegister;
using SchaerbeekMunicipality.Infrastructure.Events;
using SchaerbeekMunicipality.Application.Auth;

namespace SchaerbeekMunicipality.Application.Features.BirthDeclaration.ConfirmBirthDeclaration;

public sealed record ConfirmBirthDeclarationResponse(
    Guid CaseId,
    string Status,
    string NationalRegisterNumber,
    Guid ChildPersonId);

public sealed class ConfirmBirthDeclarationHandler(
    BirthDeclarationCaseGuard caseGuard,
    IBirthDeclarationCaseRepository caseRepository,
    IPersonRepository personRepository,
    IDomainEventDispatcher domainEventDispatcher,
    ICurrentOfficer currentOfficer,
    TimeProvider timeProvider)
{
    public async Task<ConfirmBirthDeclarationResponse> Handle(
        BirthDeclarationCaseId caseId,
        CancellationToken cancellationToken)
    {
        if (!currentOfficer.CanApproveRegistration)
        {
            throw new UnauthorizedAccessException("Only population officers can confirm birth declarations.");
        }

        var birthDeclarationCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(ConfirmBirthDeclaration),
            cancellationToken);

        if (birthDeclarationCase.ChildDateOfBirth is null ||
            string.IsNullOrWhiteSpace(birthDeclarationCase.ChildGivenNames) ||
            string.IsNullOrWhiteSpace(birthDeclarationCase.ChildFamilyName))
        {
            throw new InvalidBirthDeclarationTransitionException(
                "Child details must be recorded before confirmation.");
        }

        var child = Person.Create(new IdentityDetails(
            birthDeclarationCase.ChildGivenNames,
            birthDeclarationCase.ChildFamilyName,
            birthDeclarationCase.ChildDateOfBirth.Value,
            "Belgian"));

        var nationalRegisterNumber = NationalRegisterNumber.GenerateStub(
            child.BirthDate,
            99);

        if (await personRepository.IsNationalRegisterNumberAssignedAsync(
                nationalRegisterNumber,
                cancellationToken))
        {
            throw new NationalRegisterConflictException(
                "Generated National Register number is already assigned. Retry confirmation.");
        }

        child.AssignNationalRegisterNumber(nationalRegisterNumber);
        await personRepository.AddAsync(child, cancellationToken);

        var confirmedAt = timeProvider.GetUtcNow();
        var eventDetails = birthDeclarationCase.ConfirmDeclaration(
            child.Id,
            nationalRegisterNumber.Value,
            confirmedAt);

        await domainEventDispatcher.DispatchAsync(
            new BirthRegistered(
                eventDetails.CaseId,
                eventDetails.ChildPersonId,
                eventDetails.ChildNationalRegisterNumber,
                eventDetails.ConfirmedAt),
            cancellationToken);

        await caseRepository.SaveChangesAsync(cancellationToken);

        return new ConfirmBirthDeclarationResponse(
            birthDeclarationCase.Id.Value,
            birthDeclarationCase.Status.ToString(),
            nationalRegisterNumber.Value,
            child.Id.Value);
    }
}
