using Microsoft.EntityFrameworkCore;
using SchaerbeekMunicipality.Domain.Address;
using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Domain.Common;
using SchaerbeekMunicipality.Domain.DeathDeclaration;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.IdentityDocuments;
using SchaerbeekMunicipality.Domain.PersonFile;
using SchaerbeekMunicipality.Domain.RegisterAmendment;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Infrastructure.Persistence.Queries;

internal sealed class PersonFileQuery(MunicipalDbContext dbContext) : IPersonFileQuery
{
    public async Task<RegisterTarget?> GetRegisterTargetAsync(
        PersonId personId,
        CancellationToken cancellationToken)
    {
        var cases = await dbContext.RegistrationCases
            .AsNoTracking()
            .Where(c => c.PersonId == personId && c.Status == RegistrationCaseStatus.Registered)
            .ToListAsync(cancellationToken);

        var latestRegistered = cases
            .OrderByDescending(c => c.ClosedAt ?? c.OpenedAt)
            .FirstOrDefault();

        return latestRegistered?.SelectedRegisterTarget;
    }

    public async Task<IReadOnlyList<PersonCaseSummary>> ListCasesByPersonIdAsync(
        PersonId personId,
        CancellationToken cancellationToken)
    {
        var cases = new List<PersonCaseSummary>();

        var registrationCases = await dbContext.RegistrationCases
            .AsNoTracking()
            .Where(c => c.PersonId == personId)
            .ToListAsync(cancellationToken);

        cases.AddRange(registrationCases.Select(c => new PersonCaseSummary(
            c.Id.Value,
            "Registration",
            c.Status.ToDisplayString(),
            c.OpenedAt,
            c.ClosedAt,
            $"/registration/cases/{c.Id.Value}")));

        var birthCases = await dbContext.BirthDeclarationCases
            .AsNoTracking()
            .Include(c => c.ParentLinks)
            .ToListAsync(cancellationToken);

        foreach (var birthCase in birthCases)
        {
            var isParent = birthCase.ParentLinks.Any(link => link.PersonId == personId);
            var isChild = birthCase.ChildPersonId == personId;

            if (!isParent && !isChild) continue;

            cases.Add(new PersonCaseSummary(
                birthCase.Id.Value,
                "Birth declaration",
                birthCase.Status.ToDisplayString(),
                birthCase.OpenedAt,
                birthCase.ClosedAt,
                $"/birth-declarations/{birthCase.Id.Value}"));
        }

        var changeOfAddressCases = await dbContext.ChangeOfAddressCases
            .AsNoTracking()
            .Include(c => c.HouseholdMemberLinks)
            .ToListAsync(cancellationToken);

        foreach (var coaCase in changeOfAddressCases)
        {
            var isSubject = coaCase.PersonId == personId;
            var isCoMover = coaCase.HouseholdMemberLinks.Any(link => link.PersonId == personId);

            if (!isSubject && !isCoMover) continue;

            cases.Add(new PersonCaseSummary(
                coaCase.Id.Value,
                "Change of address",
                coaCase.Status.ToDisplayString(),
                coaCase.OpenedAt,
                coaCase.ClosedAt,
                $"/change-of-address/{coaCase.Id.Value}"));
        }

        var deathCases = await dbContext.DeathDeclarationCases
            .AsNoTracking()
            .Where(c => c.PersonId == personId)
            .ToListAsync(cancellationToken);

        cases.AddRange(deathCases.Select(c => new PersonCaseSummary(
            c.Id.Value,
            "Death declaration",
            c.Status.ToDisplayString(),
            c.OpenedAt,
            c.ClosedAt,
            $"/death-declarations/{c.Id.Value}")));

        var documentCases = await dbContext.DocumentRequestCases
            .AsNoTracking()
            .Where(c => c.PersonId == personId)
            .ToListAsync(cancellationToken);

        cases.AddRange(documentCases.Select(c => new PersonCaseSummary(
            c.Id.Value,
            "Identity document",
            c.Status.ToDisplayString(),
            c.RequestedAt,
            c.IssuedAt ?? c.CancelledAt,
            $"/identity-documents/requests/{c.Id.Value}")));

        var amendmentCases = await dbContext.RegisterAmendmentCases
            .AsNoTracking()
            .Where(c => c.PersonId == personId)
            .ToListAsync(cancellationToken);

        cases.AddRange(amendmentCases.Select(c => new PersonCaseSummary(
            c.Id.Value,
            "Register amendment",
            c.Status.ToDisplayString(),
            c.OpenedAt,
            c.ClosedAt,
            $"/register-amendments/cases/{c.Id.Value}")));

        return cases
            .OrderByDescending(c => c.OpenedAt)
            .ToList();
    }

    public async Task<IReadOnlyList<PersonHouseholdMemberSummary>> GetHouseholdMembersAsync(
        PersonId personId,
        CancellationToken cancellationToken)
    {
        var members = new List<PersonHouseholdMemberSummary>();
        var seen = new HashSet<(string Name, string Role, string Source)>();

        var latestRegistered = (await dbContext.RegistrationCases
                .AsNoTracking()
                .Where(c => c.PersonId == personId && c.Status == RegistrationCaseStatus.Registered)
                .ToListAsync(cancellationToken))
            .OrderByDescending(c => c.ClosedAt ?? c.OpenedAt)
            .FirstOrDefault();

        if (latestRegistered is not null)
        {
            var household = await dbContext.Households
                .AsNoTracking()
                .Include(h => h.Members)
                .FirstOrDefaultAsync(h => h.RegistrationCaseId == latestRegistered.Id, cancellationToken);

            if (household is not null)
                foreach (var member in household.Members)
                    AddMember(
                        members,
                        seen,
                        null,
                        member.GivenName,
                        member.FamilyName,
                        member.BirthDate,
                        member.Role.ToDisplayString(),
                        "Registration household");
        }

        var birthCases = await dbContext.BirthDeclarationCases
            .AsNoTracking()
            .Include(c => c.ParentLinks)
            .ToListAsync(cancellationToken);

        foreach (var birthCase in birthCases.Where(c => c.ParentLinks.Any(link => link.PersonId == personId)))
        {
            var childName = $"{birthCase.ChildGivenNames} {birthCase.ChildFamilyName}".Trim();
            if (!string.IsNullOrWhiteSpace(childName))
                AddMember(
                    members,
                    seen,
                    birthCase.ChildPersonId?.Value,
                    birthCase.ChildGivenNames ?? string.Empty,
                    birthCase.ChildFamilyName ?? string.Empty,
                    birthCase.ChildDateOfBirth,
                    "Child",
                    "Birth declaration");
        }

        foreach (var birthCase in birthCases.Where(c => c.ChildPersonId == personId))
        foreach (var parentLink in birthCase.ParentLinks)
        {
            var parent = await dbContext.Persons
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == parentLink.PersonId, cancellationToken);

            if (parent is not null)
                AddMember(
                    members,
                    seen,
                    parent.Id.Value,
                    parent.GivenName,
                    parent.FamilyName,
                    parent.BirthDate,
                    parentLink.Role.ToDisplayString(),
                    "Birth declaration");
        }

        var coaCases = await dbContext.ChangeOfAddressCases
            .AsNoTracking()
            .Include(c => c.HouseholdMemberLinks)
            .Where(c => c.PersonId == personId || c.HouseholdMemberLinks.Any(link => link.PersonId == personId))
            .ToListAsync(cancellationToken);

        foreach (var coaCase in coaCases)
        {
            if (coaCase.PersonId != personId)
            {
                var subject = await dbContext.Persons
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == coaCase.PersonId, cancellationToken);

                if (subject is not null)
                    AddMember(
                        members,
                        seen,
                        subject.Id.Value,
                        subject.GivenName,
                        subject.FamilyName,
                        subject.BirthDate,
                        "Primary resident",
                        "Change of address");
            }

            foreach (var link in coaCase.HouseholdMemberLinks.Where(link => link.PersonId != personId))
            {
                var coMover = await dbContext.Persons
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == link.PersonId, cancellationToken);

                if (coMover is not null)
                    AddMember(
                        members,
                        seen,
                        coMover.Id.Value,
                        coMover.GivenName,
                        coMover.FamilyName,
                        coMover.BirthDate,
                        "Co-mover",
                        "Change of address");
            }
        }

        return await FilterOutDeceasedAsync(members, personId, cancellationToken);
    }

    private async Task<IReadOnlyList<PersonHouseholdMemberSummary>> FilterOutDeceasedAsync(
        List<PersonHouseholdMemberSummary> members,
        PersonId personId,
        CancellationToken cancellationToken)
    {
        var deceased = await dbContext.Persons
            .AsNoTracking()
            .Where(p => p.DateOfDeath != null && p.Id != personId)
            .ToListAsync(cancellationToken);

        if (deceased.Count == 0) return members;

        var deceasedIds = deceased.Select(p => p.Id.Value).ToHashSet();
        var deceasedIdentities = deceased
            .Select(p => ($"{p.GivenName} {p.FamilyName}".Trim().ToLowerInvariant(), (DateOnly?)p.BirthDate))
            .ToHashSet();

        return members
            .Where(m => !IsDeceasedMember(m, deceasedIds, deceasedIdentities))
            .ToList();
    }

    private static bool IsDeceasedMember(
        PersonHouseholdMemberSummary member,
        HashSet<Guid> deceasedIds,
        HashSet<(string Name, DateOnly? BirthDate)> deceasedIdentities)
    {
        if (member.PersonId is { } memberPersonId && deceasedIds.Contains(memberPersonId)) return true;

        var name = $"{member.GivenName} {member.FamilyName}".Trim().ToLowerInvariant();
        return member.BirthDate is not null && deceasedIdentities.Contains((name, member.BirthDate));
    }

    public async Task<IReadOnlyList<PersonAddressHistoryEntry>> ListAddressHistoryAsync(
        PersonId personId,
        CancellationToken cancellationToken)
    {
        var person = await dbContext.Persons
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == personId, cancellationToken);

        var entries = new List<PersonAddressHistoryEntry>();

        var registrationCases = (await dbContext.RegistrationCases
                .AsNoTracking()
                .Where(c => c.PersonId == personId && c.DeclaredAddress != null)
                .ToListAsync(cancellationToken))
            .OrderBy(c => c.OpenedAt)
            .ToList();

        foreach (var registrationCase in registrationCases)
            if (registrationCase.DeclaredAddress is { } address)
                entries.Add(ToAddressEntry(
                    address,
                    registrationCase.HousingSituation?.ToString(),
                    registrationCase.ClosedAt ?? registrationCase.OpenedAt,
                    false,
                    "Registration"));

        var coaCases = (await dbContext.ChangeOfAddressCases
                .AsNoTracking()
                .Where(c => c.PersonId == personId)
                .ToListAsync(cancellationToken))
            .OrderBy(c => c.OpenedAt)
            .ToList();

        foreach (var coaCase in coaCases)
        {
            if (coaCase.PreviousAddress is { } previous)
                entries.Add(ToAddressEntry(
                    previous,
                    null,
                    coaCase.OpenedAt,
                    false,
                    "Change of address (previous)"));

            if (coaCase.NewAddress is { } next)
                entries.Add(ToAddressEntry(
                    next,
                    coaCase.HousingSituation?.ToString(),
                    coaCase.ConfirmedAt ?? coaCase.OpenedAt,
                    false,
                    "Change of address (new)"));
        }

        if (person?.DomicileAddress is { } domicile)
        {
            var latestCoa = coaCases.LastOrDefault(c => c.Status == ChangeOfAddressCaseStatus.Confirmed);
            var latestRegistration =
                registrationCases.LastOrDefault(c => c.Status == RegistrationCaseStatus.Registered);
            var housing = latestCoa?.HousingSituation?.ToString()
                          ?? latestRegistration?.HousingSituation?.ToString();

            entries.Add(ToAddressEntry(
                domicile,
                housing,
                latestCoa?.ConfirmedAt ?? latestRegistration?.ClosedAt,
                true,
                "Current domicile"));
        }

        return entries
            .OrderByDescending(e => e.IsCurrent)
            .ThenByDescending(e => e.EffectiveFrom)
            .ToList();
    }

    public async Task<IReadOnlyList<PersonHistoryEvent>> ListHistoryEventsAsync(
        PersonId personId,
        CancellationToken cancellationToken)
    {
        var events = new List<PersonHistoryEvent>();

        var registrationCaseIds = await dbContext.RegistrationCases
            .AsNoTracking()
            .Where(c => c.PersonId == personId)
            .Select(c => c.Id)
            .ToListAsync(cancellationToken);

        if (registrationCaseIds.Count > 0)
        {
            var auditEntries = await dbContext.CaseAuditEntries
                .AsNoTracking()
                .Where(e => registrationCaseIds.Contains(e.CaseId))
                .ToListAsync(cancellationToken);

            events.AddRange(auditEntries.Select(e => new PersonHistoryEvent(
                e.Action.ToDisplayString(),
                e.OccurredAt,
                e.Details,
                "Registration audit")));
        }

        var birthCases = await dbContext.BirthDeclarationCases
            .AsNoTracking()
            .Include(c => c.ParentLinks)
            .Where(c => c.ChildPersonId == personId || c.ParentLinks.Any(link => link.PersonId == personId))
            .ToListAsync(cancellationToken);

        foreach (var birthCase in birthCases)
        {
            events.Add(new PersonHistoryEvent(
                "Birth declaration opened",
                birthCase.OpenedAt,
                null,
                "Birth declaration"));

            if (birthCase.ConfirmedAt is { } confirmedAt)
                events.Add(new PersonHistoryEvent(
                    "Birth declaration confirmed",
                    confirmedAt,
                    birthCase.ChildNationalRegisterNumber,
                    "Birth declaration"));

            if (birthCase.ClosedAt is { } closedAt && birthCase.Status == BirthDeclarationCaseStatus.Rejected)
                events.Add(new PersonHistoryEvent(
                    "Birth declaration rejected",
                    closedAt,
                    birthCase.RejectionReason is null ? null : birthCase.RejectionReason.Value.ToDisplayString(),
                    "Birth declaration"));
        }

        var coaCases = await dbContext.ChangeOfAddressCases
            .AsNoTracking()
            .Where(c => c.PersonId == personId)
            .ToListAsync(cancellationToken);

        foreach (var coaCase in coaCases)
        {
            events.Add(new PersonHistoryEvent(
                "Change of address opened",
                coaCase.OpenedAt,
                null,
                "Change of address"));

            if (coaCase.ConfirmedAt is { } confirmedAt)
                events.Add(new PersonHistoryEvent(
                    "Address change confirmed",
                    confirmedAt,
                    FormatAddress(coaCase.NewAddress),
                    "Change of address"));

            if (coaCase.ClosedAt is { } closedAt && coaCase.Status == ChangeOfAddressCaseStatus.Rejected)
                events.Add(new PersonHistoryEvent(
                    "Change of address rejected",
                    closedAt,
                    coaCase.RejectionReason is null ? null : coaCase.RejectionReason.Value.ToDisplayString(),
                    "Change of address"));
        }

        var deathCases = await dbContext.DeathDeclarationCases
            .AsNoTracking()
            .Where(c => c.PersonId == personId)
            .ToListAsync(cancellationToken);

        foreach (var deathCase in deathCases)
        {
            events.Add(new PersonHistoryEvent(
                "Death declaration opened",
                deathCase.OpenedAt,
                null,
                "Death declaration"));

            if (deathCase.ConfirmedAt is { } deathConfirmedAt)
                events.Add(new PersonHistoryEvent(
                    "Radiation confirmed",
                    deathConfirmedAt,
                    deathCase.DeathDate is { } deathDate ? $"Date of death: {deathDate:yyyy-MM-dd}" : null,
                    "Death declaration"));

            if (deathCase.ClosedAt is { } deathClosedAt &&
                deathCase.Status == DeathDeclarationCaseStatus.Rejected)
                events.Add(new PersonHistoryEvent(
                    "Death declaration rejected",
                    deathClosedAt,
                    deathCase.RejectionReason is null ? null : deathCase.RejectionReason.Value.ToDisplayString(),
                    "Death declaration"));
        }

        var documentCases = await dbContext.DocumentRequestCases
            .AsNoTracking()
            .Where(c => c.PersonId == personId)
            .ToListAsync(cancellationToken);

        foreach (var documentCase in documentCases)
        {
            events.Add(new PersonHistoryEvent(
                $"{documentCase.RequestType.ToDisplayString()} request submitted",
                documentCase.RequestedAt,
                null,
                "Identity document"));

            if (documentCase.IssuedAt is { } issuedAt)
                events.Add(new PersonHistoryEvent(
                    $"{documentCase.RequestType.ToDisplayString()} issued",
                    issuedAt,
                    documentCase.IssuedDocumentNumber,
                    "Identity document"));

            if (documentCase.CancelledAt is { } cancelledAt)
                events.Add(new PersonHistoryEvent(
                    $"{documentCase.RequestType.ToDisplayString()} request cancelled",
                    cancelledAt,
                    documentCase.CancellationReason,
                    "Identity document"));
        }

        var certificates = await dbContext.CertificateRequests
            .AsNoTracking()
            .Where(c => c.PersonId == personId)
            .ToListAsync(cancellationToken);

        events.AddRange(certificates.Select(c => new PersonHistoryEvent(
            $"{c.CertificateType.ToDisplayString()} issued",
            c.IssuedAt,
            c.ReferenceNumber,
            "Certificate")));

        var amendmentCases = await dbContext.RegisterAmendmentCases
            .AsNoTracking()
            .Where(c => c.PersonId == personId)
            .ToListAsync(cancellationToken);

        foreach (var amendmentCase in amendmentCases)
        {
            events.Add(new PersonHistoryEvent(
                "Register amendment opened",
                amendmentCase.OpenedAt,
                amendmentCase.AmendmentType.ToDisplayString(),
                "Register amendment"));

            if (amendmentCase.SubmittedAt is { } submittedAt)
                events.Add(new PersonHistoryEvent(
                    "Amendment submitted for review",
                    submittedAt,
                    amendmentCase.BuildChangeSummary(),
                    "Register amendment"));

            if (amendmentCase.ApprovedAt is { } approvedAt)
                events.Add(new PersonHistoryEvent(
                    "Amendment approved",
                    approvedAt,
                    amendmentCase.DecisionNotes,
                    "Register amendment"));

            if (amendmentCase.AppliedAt is { } appliedAt)
                events.Add(new PersonHistoryEvent(
                    "Amendment applied",
                    appliedAt,
                    amendmentCase.BuildChangeSummary(),
                    "Register amendment"));

            if (amendmentCase.ClosedAt is { } closedAt &&
                amendmentCase.Status == RegisterAmendmentCaseStatus.Rejected)
                events.Add(new PersonHistoryEvent(
                    "Amendment rejected",
                    closedAt,
                    amendmentCase.RejectionReason?.ToDisplayString(),
                    "Register amendment"));
        }

        return events
            .OrderByDescending(e => e.Timestamp)
            .ToList();
    }

    public async Task<IReadOnlyList<PersonFileDocumentSummary>> ListDocumentsByPersonIdAsync(
        PersonId personId,
        CancellationToken cancellationToken)
    {
        var documents = new List<PersonFileDocumentSummary>();
        var cases = await ListCasesByPersonIdAsync(personId, cancellationToken);
        var caseLookup = cases.ToDictionary(c => c.CaseId);

        // Nullable IDs so Contains matches the converted nullable columns (Npgsql T vs T? bug).
        var registrationCaseIds = cases
            .Where(c => c.Workflow == "Registration")
            .Select(c => (RegistrationCaseId?)new RegistrationCaseId(c.CaseId))
            .ToList();

        if (registrationCaseIds.Count > 0)
        {
            var registrationDocuments = await dbContext.AdministrativeDocuments
                .AsNoTracking()
                .Where(d => registrationCaseIds.Contains(d.RegistrationCaseId))
                .ToListAsync(cancellationToken);

            documents.AddRange(registrationDocuments.Select(d => ToDocumentSummary(
                d.Id.Value,
                d.RegistrationCaseId!.Value.Value,
                "Registration",
                d.DocumentType,
                d.FileName,
                d.UploadedAt,
                caseLookup)));
        }

        var birthCaseIds = cases
            .Where(c => c.Workflow == "Birth declaration")
            .Select(c => (BirthDeclarationCaseId?)new BirthDeclarationCaseId(c.CaseId))
            .ToList();

        if (birthCaseIds.Count > 0)
        {
            var birthDocuments = await dbContext.AdministrativeDocuments
                .AsNoTracking()
                .Where(d => birthCaseIds.Contains(d.BirthDeclarationCaseId))
                .ToListAsync(cancellationToken);

            documents.AddRange(birthDocuments.Select(d => ToDocumentSummary(
                d.Id.Value,
                d.BirthDeclarationCaseId!.Value.Value,
                "Birth declaration",
                d.DocumentType,
                d.FileName,
                d.UploadedAt,
                caseLookup)));
        }

        var changeOfAddressCaseIds = cases
            .Where(c => c.Workflow == "Change of address")
            .Select(c => (ChangeOfAddressCaseId?)new ChangeOfAddressCaseId(c.CaseId))
            .ToList();

        if (changeOfAddressCaseIds.Count > 0)
        {
            var changeOfAddressDocuments = await dbContext.AdministrativeDocuments
                .AsNoTracking()
                .Where(d => changeOfAddressCaseIds.Contains(d.ChangeOfAddressCaseId))
                .ToListAsync(cancellationToken);

            documents.AddRange(changeOfAddressDocuments.Select(d => ToDocumentSummary(
                d.Id.Value,
                d.ChangeOfAddressCaseId!.Value.Value,
                "Change of address",
                d.DocumentType,
                d.FileName,
                d.UploadedAt,
                caseLookup)));
        }

        var deathCaseIds = cases
            .Where(c => c.Workflow == "Death declaration")
            .Select(c => (DeathDeclarationCaseId?)new DeathDeclarationCaseId(c.CaseId))
            .ToList();

        if (deathCaseIds.Count > 0)
        {
            var deathDocuments = await dbContext.AdministrativeDocuments
                .AsNoTracking()
                .Where(d => deathCaseIds.Contains(d.DeathDeclarationCaseId))
                .ToListAsync(cancellationToken);

            documents.AddRange(deathDocuments.Select(d => ToDocumentSummary(
                d.Id.Value,
                d.DeathDeclarationCaseId!.Value.Value,
                "Death declaration",
                d.DocumentType,
                d.FileName,
                d.UploadedAt,
                caseLookup)));
        }

        var documentRequestCaseIds = cases
            .Where(c => c.Workflow == "Identity document")
            .Select(c => (DocumentRequestCaseId?)new DocumentRequestCaseId(c.CaseId))
            .ToList();

        if (documentRequestCaseIds.Count > 0)
        {
            var identityDocuments = await dbContext.AdministrativeDocuments
                .AsNoTracking()
                .Where(d => documentRequestCaseIds.Contains(d.DocumentRequestCaseId))
                .ToListAsync(cancellationToken);

            documents.AddRange(identityDocuments.Select(d => ToDocumentSummary(
                d.Id.Value,
                d.DocumentRequestCaseId!.Value.Value,
                "Identity document",
                d.DocumentType,
                d.FileName,
                d.UploadedAt,
                caseLookup)));
        }

        var amendmentCaseIds = cases
            .Where(c => c.Workflow == "Register amendment")
            .Select(c => (RegisterAmendmentCaseId?)new RegisterAmendmentCaseId(c.CaseId))
            .ToList();

        if (amendmentCaseIds.Count > 0)
        {
            var amendmentDocuments = await dbContext.AdministrativeDocuments
                .AsNoTracking()
                .Where(d => amendmentCaseIds.Contains(d.RegisterAmendmentCaseId))
                .ToListAsync(cancellationToken);

            documents.AddRange(amendmentDocuments.Select(d => ToDocumentSummary(
                d.Id.Value,
                d.RegisterAmendmentCaseId!.Value.Value,
                "Register amendment",
                d.DocumentType,
                d.FileName,
                d.UploadedAt,
                caseLookup)));
        }

        return documents
            .OrderByDescending(d => d.UploadedAt)
            .ToList();
    }

    private static PersonFileDocumentSummary ToDocumentSummary(
        Guid documentId,
        Guid caseId,
        string workflow,
        DocumentType documentType,
        string fileName,
        DateTimeOffset uploadedAt,
        IReadOnlyDictionary<Guid, PersonCaseSummary> caseLookup)
    {
        return new PersonFileDocumentSummary(
            documentId,
            caseId,
            workflow,
            documentType,
            fileName,
            uploadedAt,
            caseLookup[caseId].DetailPath);
    }

    private static void AddMember(
        List<PersonHouseholdMemberSummary> members,
        HashSet<(string Name, string Role, string Source)> seen,
        Guid? personId,
        string givenName,
        string familyName,
        DateOnly? birthDate,
        string role,
        string source)
    {
        var key = ($"{givenName} {familyName}".Trim(), role, source);
        if (!seen.Add(key)) return;

        members.Add(new PersonHouseholdMemberSummary(
            personId,
            givenName,
            familyName,
            birthDate,
            role,
            source));
    }

    private static PersonAddressHistoryEntry ToAddressEntry(
        BelgianAddress address,
        string? housingSituation,
        DateTimeOffset? effectiveFrom,
        bool isCurrent,
        string source)
    {
        return new PersonAddressHistoryEntry(
            address.Street,
            address.HouseNumber,
            address.Box,
            address.PostalCode,
            address.Municipality,
            housingSituation,
            effectiveFrom,
            isCurrent,
            source);
    }

    private static string? FormatAddress(BelgianAddress? address)
    {
        return address is null
            ? null
            : $"{address.Street} {address.HouseNumber}, {address.PostalCode} {address.Municipality}";
    }
}