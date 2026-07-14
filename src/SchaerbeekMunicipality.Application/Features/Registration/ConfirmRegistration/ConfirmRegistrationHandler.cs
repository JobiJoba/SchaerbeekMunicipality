using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.Events;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.NationalRegister;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Infrastructure.Events;

namespace SchaerbeekMunicipality.Application.Features.Registration.ConfirmRegistration;

public sealed class ConfirmRegistrationHandler(
    RegistrationCaseGuard caseGuard,
    IRegistrationCaseRepository caseRepository,
    IPersonRepository personRepository,
    INationalRegisterRepository nationalRegisterRepository,
    CaseAuditRecorder auditRecorder,
    IDomainEventDispatcher domainEventDispatcher,
    ICurrentOfficer currentOfficer,
    TimeProvider timeProvider)
{
    public async Task<ConfirmRegistrationResponse> Handle(
        RegistrationCaseId caseId,
        CancellationToken cancellationToken)
    {
        if (!currentOfficer.CanApproveRegistration)
            throw new UnauthorizedAccessException("Only population officers can confirm registration.");

        var registrationCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(ConfirmRegistration),
            cancellationToken);

        var confirmedAt = timeProvider.GetUtcNow();
        var eventDetails = registrationCase.ConfirmRegistration(confirmedAt);

        var person = await personRepository.GetForUpdateAsync(eventDetails.PersonId, cancellationToken)
                     ?? throw new KeyNotFoundException($"Person '{eventDetails.PersonId}' was not found.");

        if (registrationCase.DeclaredAddress is { } declaredAddress) person.UpdateDomicile(declaredAddress);

        var assignedNr = person.NationalRegisterNumber?.Value;

        if (person.NationalRegisterNumber is null)
        {
            var nationalRegisterNumber = NationalRegisterNumber.GenerateStub(person.BirthDate, 99);

            if (await personRepository.IsNationalRegisterNumberAssignedAsync(
                    nationalRegisterNumber,
                    cancellationToken))
                throw new NationalRegisterConflictException(
                    "Generated National Register number is already assigned. Retry confirmation.");

            person.AssignNationalRegisterNumber(nationalRegisterNumber);
            assignedNr = nationalRegisterNumber.Value;

            if (person.LinkedRegisterRecordId is { } registerRecordId)
            {
                var registerPerson = await nationalRegisterRepository.GetByIdAsync(
                    registerRecordId,
                    cancellationToken);

                registerPerson?.AssignNationalRegisterNumber(nationalRegisterNumber);
            }
        }

        await auditRecorder.RecordAsync(
            caseId,
            CaseAuditAction.RegistrationConfirmed,
            $"Register: {eventDetails.RegisterTarget}, NR: {assignedNr}",
            cancellationToken);

        await domainEventDispatcher.DispatchAsync(
            new RegistrationConfirmed(
                eventDetails.CaseId,
                eventDetails.PersonId,
                eventDetails.RegisterTarget,
                eventDetails.ConfirmedAt),
            cancellationToken);

        await caseRepository.SaveChangesAsync(cancellationToken);
        await nationalRegisterRepository.SaveChangesAsync(cancellationToken);

        return new ConfirmRegistrationResponse(
            registrationCase.Id.Value,
            registrationCase.Status.ToString(),
            eventDetails.RegisterTarget.ToString(),
            assignedNr);
    }
}