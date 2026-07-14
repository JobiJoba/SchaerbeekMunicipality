namespace SchaerbeekMunicipality.Domain.IdentityDocuments;

public static class DocumentRequestRules
{
    public const int MinorAgeThreshold = 18;

    public static bool IsMinor(DateOnly birthDate, DateOnly referenceDate)
    {
        return birthDate > referenceDate.AddYears(-MinorAgeThreshold);
    }

    public static void EnsureCanAdvance(
        DocumentRequestCaseStatus currentStatus,
        bool photoAttached,
        bool feePaid)
    {
        if (currentStatus is DocumentRequestCaseStatus.Issued or DocumentRequestCaseStatus.Cancelled)
            throw new InvalidDocumentRequestTransitionException(
                $"Cannot advance status from '{currentStatus}'.");

        if (currentStatus == DocumentRequestCaseStatus.ReadyForCollection)
            throw new InvalidDocumentRequestTransitionException(
                "Use issue to complete a request that is ready for collection.");

        if (!photoAttached)
            throw new InvalidDocumentRequestTransitionException(
                "Cannot advance status without an attached applicant photo.");

        if (!feePaid)
            throw new InvalidDocumentRequestTransitionException(
                "Cannot advance status before the fee has been recorded as paid.");
    }

    public static DocumentRequestCaseStatus NextStatus(DocumentRequestCaseStatus currentStatus)
    {
        return currentStatus switch
        {
            DocumentRequestCaseStatus.Submitted => DocumentRequestCaseStatus.InProduction,
            DocumentRequestCaseStatus.InProduction => DocumentRequestCaseStatus.ReadyForCollection,
            _ => throw new InvalidDocumentRequestTransitionException(
                $"Cannot advance status from '{currentStatus}'.")
        };
    }

    public static void EnsureCanIssue(DocumentRequestCaseStatus status, bool photoAttached, bool feePaid)
    {
        if (status != DocumentRequestCaseStatus.ReadyForCollection)
            throw new InvalidDocumentRequestTransitionException(
                "Document can only be issued when the request is ready for collection.");

        if (!photoAttached)
            throw new InvalidDocumentRequestTransitionException(
                "Cannot issue a document without an attached applicant photo.");

        if (!feePaid)
            throw new InvalidDocumentRequestTransitionException(
                "Cannot issue a document before the fee has been recorded as paid.");
    }

    public static void EnsureCanCancel(DocumentRequestCaseStatus status)
    {
        if (status is DocumentRequestCaseStatus.Issued or DocumentRequestCaseStatus.Cancelled)
            throw new InvalidDocumentRequestTransitionException(
                $"Cannot cancel a request in '{status}' status.");
    }
}