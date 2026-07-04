using FluentValidation;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Web.Auth;

namespace SchaerbeekMunicipality.Web.Features.Registration.OpenRegistrationCase;

public sealed class OpenRegistrationCaseHandler(
    IRegistrationCaseRepository repository,
    ICurrentOfficer currentOfficer,
    TimeProvider timeProvider,
    IValidator<OpenRegistrationCaseRequest> validator)
{
    public async Task<OpenRegistrationCaseResponse> Handle(
        OpenRegistrationCaseRequest request,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var assignedOfficer = request.AssignedOfficerId is { } officerId
            ? OfficerId.From(officerId)
            : OfficerId.From(currentOfficer.OfficerId);

        var registrationCase = RegistrationCase.Open(
            request.VisitReason,
            assignedOfficer,
            timeProvider.GetUtcNow());

        await repository.AddAsync(registrationCase, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return new OpenRegistrationCaseResponse(
            registrationCase.Id.Value,
            registrationCase.Status.ToString(),
            registrationCase.VisitReason,
            registrationCase.OpenedAt);
    }
}
