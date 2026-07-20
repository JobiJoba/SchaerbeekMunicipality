using FluentValidation;
using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.Household;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.IdentityDocuments;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.IdentityDocuments.OpenDocumentRequestCase;

public sealed record OpenDocumentRequestCaseRequest(Guid PersonId, DocumentRequestType RequestType);

public sealed record OpenDocumentRequestCaseResponse(
    Guid CaseId,
    Guid PersonId,
    DocumentRequestType RequestType,
    string Status,
    DateTimeOffset RequestedAt);

public sealed class OpenDocumentRequestCaseValidator : AbstractValidator<OpenDocumentRequestCaseRequest>
{
    public OpenDocumentRequestCaseValidator()
    {
        RuleFor(x => x.PersonId).NotEmpty();
        RuleFor(x => x.RequestType).IsInEnum();
    }
}

public sealed class OpenDocumentRequestCaseHandler(
    IDocumentRequestCaseRepository caseRepository,
    IPersonRepository personRepository,
    IRegistrationCaseRepository registrationCaseRepository,
    IHouseholdRepository householdRepository,
    DocumentRequestCaseAuthorization authorization,
    ICurrentOfficer currentOfficer,
    IValidator<OpenDocumentRequestCaseRequest> validator,
    TimeProvider timeProvider)
{
    public async Task<OpenDocumentRequestCaseResponse> Handle(
        OpenDocumentRequestCaseRequest request,
        CancellationToken cancellationToken)
    {
        authorization.EnsureCanCreate(currentOfficer);
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var personId = new PersonId(request.PersonId);
        var person = await personRepository.GetByIdAsync(personId, cancellationToken)
                     ?? throw new KeyNotFoundException($"Person '{personId}' was not found.");

        if (person.NationalRegisterNumber is null)
            throw new InvalidDocumentRequestTransitionException(
                "Cannot open a document request for a person without a National Register number.");

        if (person.IsDeceased)
            throw new InvalidDocumentRequestTransitionException(
                "Cannot open a document request for a deceased person.");

        await EnsureMinorHasLinkedParentAsync(person, cancellationToken);

        var documentRequestCase = DocumentRequestCase.Open(
            personId,
            request.RequestType,
            timeProvider.GetUtcNow());

        await caseRepository.AddAsync(documentRequestCase, cancellationToken);
        await caseRepository.SaveChangesAsync(cancellationToken);

        return new OpenDocumentRequestCaseResponse(
            documentRequestCase.Id.Value,
            person.Id.Value,
            documentRequestCase.RequestType,
            documentRequestCase.Status.ToString(),
            documentRequestCase.RequestedAt);
    }

    private async Task EnsureMinorHasLinkedParentAsync(Person person, CancellationToken cancellationToken)
    {
        var referenceDate = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        if (!DocumentRequestRules.IsMinor(person.BirthDate, referenceDate)) return;

        var registrationCase = await registrationCaseRepository.GetLatestRegisteredByPersonIdAsync(
            person.Id,
            cancellationToken);

        if (registrationCase is null)
            throw new InvalidDocumentRequestTransitionException(
                "Minor applicants must have a linked parent in the household.");

        var household = await householdRepository.GetByCaseIdAsync(registrationCase.Id, cancellationToken);
        if (household is null ||
            !household.Members.Any(m => m.Role is HouseholdMemberRole.Head or HouseholdMemberRole.Spouse))
            throw new InvalidDocumentRequestTransitionException(
                "Minor applicants must have a linked parent in the household.");
    }
}