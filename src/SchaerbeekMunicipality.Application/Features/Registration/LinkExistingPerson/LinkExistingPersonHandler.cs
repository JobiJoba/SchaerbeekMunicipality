using FluentValidation;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.NationalRegister;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.Registration.LinkExistingPerson;

public sealed class LinkExistingPersonHandler(
    RegistrationCaseGuard caseGuard,
    IRegistrationCaseRepository caseRepository,
    IPersonRepository personRepository,
    INationalRegisterRepository nationalRegisterRepository,
    RegistrationExceptionEvaluator exceptionEvaluator,
    IValidator<LinkExistingPersonRequest> validator)
{
    public async Task<LinkExistingPersonResponse> Handle(
        RegistrationCaseId caseId,
        LinkExistingPersonRequest request,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var registrationCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(LinkExistingPerson),
            cancellationToken);

        var registerPersonId = NationalRegisterPersonId.From(request.RegisterPersonId);
        var registerPerson = await nationalRegisterRepository.GetByIdAsync(registerPersonId, cancellationToken)
                             ?? throw new KeyNotFoundException(
                                 $"National Register record '{registerPersonId}' was not found.");

        await EnsureCanLinkAsync(registerPerson, cancellationToken);

        var person = registrationCase.LinkExistingPerson(registerPerson);
        registrationCase.RefreshRegisterDeterminability(person.Nationality);

        await personRepository.AddAsync(person, cancellationToken);
        await exceptionEvaluator.EvaluateAndApplyAsync(registrationCase, cancellationToken);
        await caseRepository.SaveChangesAsync(cancellationToken);

        return new LinkExistingPersonResponse(
            registrationCase.Id.Value,
            person.Id.Value,
            registrationCase.Checklist.IdentityEstablished,
            person.BisNumber?.Value,
            person.NationalRegisterNumber?.Value,
            person.LinkedRegisterRecordId is not null);
    }

    private async Task EnsureCanLinkAsync(
        NationalRegisterPerson registerPerson,
        CancellationToken cancellationToken)
    {
        if (await personRepository.IsRegisterRecordLinkedAsync(registerPerson.Id, cancellationToken))
            throw new NationalRegisterConflictException(
                "This National Register record is already linked to an active person file.");

        if (registerPerson.NationalRegisterNumber is { } nationalRegisterNumber
            && await personRepository.IsNationalRegisterNumberAssignedAsync(
                nationalRegisterNumber,
                cancellationToken))
            throw new NationalRegisterConflictException(
                "The National Register number is already assigned to another person.");
    }
}