using FluentValidation;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Application.Features.Registration;

namespace SchaerbeekMunicipality.Application.Features.Registration.RecordHousingSituation;

public sealed class RecordHousingSituationHandler(
    RegistrationCaseGuard caseGuard,
    IRegistrationCaseRepository caseRepository,
    IValidator<RecordHousingSituationRequest> validator)
{
    public async Task<RecordHousingSituationResponse> Handle(
        RegistrationCaseId caseId,
        RecordHousingSituationRequest request,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var registrationCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(RecordHousingSituation),
            cancellationToken);

        registrationCase.RecordHousingSituation(request.Situation);

        await caseRepository.SaveChangesAsync(cancellationToken);

        return new RecordHousingSituationResponse(
            registrationCase.Id.Value,
            request.Situation);
    }
}
