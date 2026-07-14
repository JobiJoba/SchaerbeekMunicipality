using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.RegisterAmendment;

namespace SchaerbeekMunicipality.Application.Features.RegisterAmendment.RecordProposedAmendment;

public sealed class RecordProposedAmendmentHandler(
    RegisterAmendmentCaseGuard caseGuard,
    IRegisterAmendmentCaseRepository caseRepository)
{
    public async Task<RecordProposedAmendmentResponse> Handle(
        RegisterAmendmentCaseId caseId,
        RecordProposedAmendmentRequest request,
        CancellationToken cancellationToken)
    {
        var amendmentCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(RecordProposedAmendment),
            cancellationToken);

        amendmentCase.SetReason(request.Reason);

        switch (amendmentCase.AmendmentType)
        {
            case AmendmentType.IdentityCorrection:
                if (string.IsNullOrWhiteSpace(request.GivenName) || string.IsNullOrWhiteSpace(request.FamilyName))
                    throw new InvalidRegisterAmendmentTransitionException(
                        "Given name and family name are required for identity correction.");

                amendmentCase.RecordIdentityCorrection(request.GivenName, request.FamilyName);
                break;

            case AmendmentType.NationalityChange:
                if (string.IsNullOrWhiteSpace(request.Nationality))
                    throw new InvalidRegisterAmendmentTransitionException(
                        "Nationality is required for nationality change amendments.");

                amendmentCase.RecordNationalityChange(request.Nationality);
                break;

            case AmendmentType.CivilStatusUpdate:
                if (request.CivilStatus is null)
                    throw new InvalidRegisterAmendmentTransitionException(
                        "Civil status is required for civil status update amendments.");

                amendmentCase.RecordCivilStatusUpdate(new CivilStatusDetails(
                    request.CivilStatus.Value,
                    request.SpouseGivenName,
                    request.SpouseFamilyName,
                    request.MarriageDate,
                    request.MarriagePlace,
                    request.MarriageRecognitionStatus));
                break;

            default:
                throw new InvalidRegisterAmendmentTransitionException(
                    $"Unsupported amendment type '{amendmentCase.AmendmentType}'.");
        }

        await caseRepository.SaveChangesAsync(cancellationToken);

        return new RecordProposedAmendmentResponse(
            amendmentCase.Id.Value,
            amendmentCase.Checklist.ProposedChangesRecorded);
    }
}
