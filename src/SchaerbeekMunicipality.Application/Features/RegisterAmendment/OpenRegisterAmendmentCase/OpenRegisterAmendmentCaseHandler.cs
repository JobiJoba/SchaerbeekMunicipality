using FluentValidation;
using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.RegisterAmendment;

namespace SchaerbeekMunicipality.Application.Features.RegisterAmendment.OpenRegisterAmendmentCase;

public sealed class OpenRegisterAmendmentCaseHandler(
    IRegisterAmendmentCaseRepository caseRepository,
    IPersonRepository personRepository,
    RegisterAmendmentCaseAuthorization authorization,
    ICurrentOfficer currentOfficer,
    IValidator<OpenRegisterAmendmentCaseRequest> validator,
    TimeProvider timeProvider)
{
    public async Task<OpenRegisterAmendmentCaseResponse> Handle(
        OpenRegisterAmendmentCaseRequest request,
        CancellationToken cancellationToken)
    {
        authorization.EnsureCanCreate(currentOfficer);
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        if (!Enum.TryParse<AmendmentType>(request.AmendmentType, ignoreCase: true, out var amendmentType))
            throw new ArgumentException($"Unknown amendment type '{request.AmendmentType}'.");

        var personId = new PersonId(request.PersonId);
        var person = await personRepository.GetByIdAsync(personId, cancellationToken)
                     ?? throw new KeyNotFoundException($"Person '{personId}' was not found.");

        if (person.NationalRegisterNumber is null)
            throw new InvalidRegisterAmendmentTransitionException(
                "Cannot open a register amendment case for a person without a National Register number.");

        if (await caseRepository.HasOpenCaseForPersonAsync(personId, cancellationToken))
            throw new InvalidRegisterAmendmentTransitionException(
                "This person already has an open register amendment case.");

        var amendmentCase = RegisterAmendmentCase.Open(
            personId,
            amendmentType,
            timeProvider.GetUtcNow());

        await caseRepository.AddAsync(amendmentCase, cancellationToken);
        await caseRepository.SaveChangesAsync(cancellationToken);

        return new OpenRegisterAmendmentCaseResponse(
            amendmentCase.Id.Value,
            person.Id.Value,
            amendmentCase.AmendmentType.ToString(),
            amendmentCase.Status.ToString(),
            amendmentCase.OpenedAt);
    }
}
