using FluentValidation;
using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.Registration.OpenRegistrationCase;

public sealed class OpenRegistrationCaseHandler(
    IRegistrationCaseRepository repository,
    RegistrationCaseAuthorization authorization,
    CaseAuditRecorder auditRecorder,
    ICurrentOfficer currentOfficer,
    TimeProvider timeProvider,
    IValidator<OpenRegistrationCaseRequest> validator)
{
    public async Task<OpenRegistrationCaseResponse> Handle(
        OpenRegistrationCaseRequest request,
        CancellationToken cancellationToken)
    {
        authorization.EnsureCanCreate(currentOfficer);
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var registrationCase = RegistrationCase.Open(
            request.VisitReason,
            timeProvider.GetUtcNow());

        await repository.AddAsync(registrationCase, cancellationToken);

        await auditRecorder.RecordAsync(
            registrationCase.Id,
            CaseAuditAction.CaseOpened,
            request.VisitReason.ToString(),
            cancellationToken);

        await repository.SaveChangesAsync(cancellationToken);

        return new OpenRegistrationCaseResponse(
            registrationCase.Id.Value,
            registrationCase.Status.ToString(),
            registrationCase.VisitReason,
            registrationCase.OpenedAt);
    }
}