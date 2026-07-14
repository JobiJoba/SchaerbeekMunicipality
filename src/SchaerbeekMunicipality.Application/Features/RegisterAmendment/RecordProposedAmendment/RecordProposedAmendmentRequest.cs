using SchaerbeekMunicipality.Domain.Identity;

namespace SchaerbeekMunicipality.Application.Features.RegisterAmendment.RecordProposedAmendment;

public sealed record RecordProposedAmendmentRequest(
    string? Reason,
    string? GivenName,
    string? FamilyName,
    string? Nationality,
    CivilStatus? CivilStatus,
    string? SpouseGivenName,
    string? SpouseFamilyName,
    DateOnly? MarriageDate,
    string? MarriagePlace,
    MarriageRecognitionStatus MarriageRecognitionStatus = MarriageRecognitionStatus.NotApplicable);

public sealed record RecordProposedAmendmentResponse(
    Guid CaseId,
    bool ProposedChangesRecorded);
