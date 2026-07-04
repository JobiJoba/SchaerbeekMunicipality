using FluentValidation;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Web.Features.Registration.RecordHousingSituation;

public sealed class RecordHousingSituationHandler(
    IRegistrationCaseRepository caseRepository,
    IValidator<RecordHousingSituationRequest> validator)
{
    public async Task<RecordHousingSituationResponse> Handle(
        RegistrationCaseId caseId,
        RecordHousingSituationRequest request,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var registrationCase = await caseRepository.GetByIdAsync(caseId, cancellationToken)
            ?? throw new KeyNotFoundException($"Registration case '{caseId}' was not found.");

        registrationCase.RecordHousingSituation(request.Situation);

        await caseRepository.SaveChangesAsync(cancellationToken);

        return new RecordHousingSituationResponse(
            registrationCase.Id.Value,
            request.Situation);
    }
}
