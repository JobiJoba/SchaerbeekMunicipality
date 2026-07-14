namespace SchaerbeekMunicipality.Domain.RegisterAmendment;

public sealed class RegisterAmendmentCaseChecklist
{
    private RegisterAmendmentCaseChecklist()
    {
    }

    public bool ProposedChangesRecorded { get; private set; }

    public bool SupportingDocumentAttached { get; private set; }

    public static RegisterAmendmentCaseChecklist Empty()
    {
        return new RegisterAmendmentCaseChecklist();
    }

    internal void MarkProposedChangesRecorded()
    {
        ProposedChangesRecorded = true;
    }

    internal void MarkSupportingDocumentAttached()
    {
        SupportingDocumentAttached = true;
    }

    internal void ClearSupportingDocumentAttached()
    {
        SupportingDocumentAttached = false;
    }
}
