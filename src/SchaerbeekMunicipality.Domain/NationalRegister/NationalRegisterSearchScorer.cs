namespace SchaerbeekMunicipality.Domain.NationalRegister;

public static class NationalRegisterSearchScorer
{
    public const int ExactMatchScore = 100;
    public const int BirthDateAndNameScore = 80;
    public const int NameOnlyScore = 50;
    public const int PartialNameScore = 40;
    public const int MinimumReportableScore = 40;
    public const int BrowseAllScore = 0;
    public const string BrowseAllReason = "All records";

    public static IReadOnlyList<NationalRegisterMatch> ScoreMatches(
        NationalRegisterSearchCriteria criteria,
        IReadOnlyList<NationalRegisterPerson> candidates)
    {
        if (!criteria.HasAnyCriterion)
        {
            return candidates
                .OrderBy(c => c.FamilyName, StringComparer.OrdinalIgnoreCase)
                .ThenBy(c => c.GivenName, StringComparer.OrdinalIgnoreCase)
                .Select(candidate => new NationalRegisterMatch(
                    candidate.Id,
                    candidate.GivenName,
                    candidate.FamilyName,
                    candidate.BirthDate,
                    candidate.Nationality,
                    candidate.BisNumber?.Value,
                    candidate.NationalRegisterNumber?.Value,
                    BrowseAllScore,
                    BrowseAllReason))
                .ToList();
        }

        var matches = new List<NationalRegisterMatch>();

        foreach (var candidate in candidates)
        {
            var (score, reason) = ScoreCandidate(criteria, candidate);
            if (score < MinimumReportableScore)
            {
                continue;
            }

            matches.Add(new NationalRegisterMatch(
                candidate.Id,
                candidate.GivenName,
                candidate.FamilyName,
                candidate.BirthDate,
                candidate.Nationality,
                candidate.BisNumber?.Value,
                candidate.NationalRegisterNumber?.Value,
                score,
                reason));
        }

        return matches
            .OrderByDescending(m => m.MatchScore)
            .ThenBy(m => m.FamilyName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(m => m.GivenName, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static (int Score, string Reason) ScoreCandidate(
        NationalRegisterSearchCriteria criteria,
        NationalRegisterPerson candidate)
    {
        var hasGiven = HasValue(criteria.GivenName);
        var hasFamily = HasValue(criteria.FamilyName);
        var hasBirth = criteria.BirthDate.HasValue;

        if (!MeetsProvidedCriteria(criteria, candidate, hasGiven, hasFamily, hasBirth))
        {
            return (0, string.Empty);
        }

        var givenExact = !hasGiven || NamesMatch(criteria.GivenName!, candidate.GivenName);
        var familyExact = !hasFamily || NamesMatch(criteria.FamilyName!, candidate.FamilyName);
        var birthMatch = !hasBirth || criteria.BirthDate == candidate.BirthDate;

        if (hasGiven && hasFamily && hasBirth && givenExact && familyExact && birthMatch)
        {
            return (ExactMatchScore, "Exact name and date of birth match.");
        }

        if (hasBirth && birthMatch && hasGiven && hasFamily)
        {
            if (givenExact && familyExact)
            {
                return (BirthDateAndNameScore, "Same date of birth with matching name.");
            }

            if (familyExact && NamesSimilar(criteria.GivenName!, candidate.GivenName, criteria.FamilyName!, candidate.FamilyName))
            {
                return (BirthDateAndNameScore, "Same date of birth with similar name.");
            }
        }

        if (hasBirth && birthMatch && (hasGiven || hasFamily))
        {
            return (BirthDateAndNameScore, "Same date of birth with similar name.");
        }

        if (hasGiven && hasFamily && givenExact && familyExact)
        {
            return (NameOnlyScore, "Exact name match.");
        }

        if (hasGiven && hasFamily)
        {
            return (NameOnlyScore, "Similar name match.");
        }

        if (hasBirth && !hasGiven && !hasFamily && birthMatch)
        {
            return (PartialNameScore, "Date of birth matches.");
        }

        if (hasGiven && !hasFamily && !hasBirth)
        {
            return (PartialNameScore, "Given name matches.");
        }

        if (!hasGiven && hasFamily && !hasBirth)
        {
            return (PartialNameScore, "Family name matches.");
        }

        return (PartialNameScore, "Partial match.");
    }

    private static bool MeetsProvidedCriteria(
        NationalRegisterSearchCriteria criteria,
        NationalRegisterPerson candidate,
        bool hasGiven,
        bool hasFamily,
        bool hasBirth)
    {
        if (hasGiven && !NamePartialMatch(criteria.GivenName!, candidate.GivenName)
            && !(hasBirth && criteria.BirthDate == candidate.BirthDate
                 && hasFamily && NamesMatch(criteria.FamilyName!, candidate.FamilyName)
                 && NamesSimilar(criteria.GivenName!, candidate.GivenName, criteria.FamilyName!, candidate.FamilyName)))
        {
            return false;
        }

        if (hasFamily && !NamePartialMatch(criteria.FamilyName!, candidate.FamilyName))
        {
            return false;
        }

        if (hasBirth && criteria.BirthDate != candidate.BirthDate)
        {
            return false;
        }

        return true;
    }

    private static bool HasValue(string? value) => !string.IsNullOrWhiteSpace(value);

    private static bool NamesMatch(string left, string right) =>
        string.Equals(NormalizeName(left), NormalizeName(right), StringComparison.Ordinal);

    private static bool NamePartialMatch(string criteria, string candidateValue)
    {
        var normalizedCriteria = NormalizeName(criteria);
        var normalizedCandidate = NormalizeName(candidateValue);

        return normalizedCandidate.Contains(normalizedCriteria, StringComparison.Ordinal)
               || normalizedCriteria.Contains(normalizedCandidate, StringComparison.Ordinal)
               || normalizedCandidate.StartsWith(normalizedCriteria, StringComparison.Ordinal)
               || normalizedCriteria.StartsWith(normalizedCandidate, StringComparison.Ordinal);
    }

    private static bool NamesSimilar(
        string criteriaGiven,
        string candidateGiven,
        string criteriaFamily,
        string candidateFamily)
    {
        if (NormalizeName(criteriaFamily) != NormalizeName(candidateFamily))
        {
            return false;
        }

        var normalizedGivenCriteria = NormalizeName(criteriaGiven);
        var normalizedGivenCandidate = NormalizeName(candidateGiven);

        if (normalizedGivenCriteria.StartsWith(normalizedGivenCandidate, StringComparison.Ordinal)
            || normalizedGivenCandidate.StartsWith(normalizedGivenCriteria, StringComparison.Ordinal))
        {
            return true;
        }

        if (normalizedGivenCandidate.TrimEnd('.').Length <= 2
            && normalizedGivenCriteria.StartsWith(normalizedGivenCandidate.TrimEnd('.'), StringComparison.Ordinal))
        {
            return true;
        }

        return false;
    }

    private static string NormalizeName(string value) =>
        value.Trim().ToUpperInvariant();
}
