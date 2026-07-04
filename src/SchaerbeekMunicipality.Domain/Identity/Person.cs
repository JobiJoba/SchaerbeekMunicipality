namespace SchaerbeekMunicipality.Domain.Identity;

public sealed class Person
{
    private Person()
    {
    }

    public PersonId Id { get; private set; }

    public string GivenName { get; private set; } = string.Empty;

    public string FamilyName { get; private set; } = string.Empty;

    public DateOnly BirthDate { get; private set; }

    public string Nationality { get; private set; } = string.Empty;

    public CivilStatusRecord? CivilStatus { get; private set; }

    public static Person Create(IdentityDetails details)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(details.GivenName);
        ArgumentException.ThrowIfNullOrWhiteSpace(details.FamilyName);
        ArgumentException.ThrowIfNullOrWhiteSpace(details.Nationality);

        return new Person
        {
            Id = PersonId.New(),
            GivenName = details.GivenName.Trim(),
            FamilyName = details.FamilyName.Trim(),
            BirthDate = details.BirthDate,
            Nationality = details.Nationality.Trim(),
        };
    }

    public void Update(IdentityDetails details)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(details.GivenName);
        ArgumentException.ThrowIfNullOrWhiteSpace(details.FamilyName);
        ArgumentException.ThrowIfNullOrWhiteSpace(details.Nationality);

        GivenName = details.GivenName.Trim();
        FamilyName = details.FamilyName.Trim();
        BirthDate = details.BirthDate;
        Nationality = details.Nationality.Trim();
    }

    public void RecordCivilStatus(CivilStatusDetails details)
    {
        CivilStatus = CivilStatusRecord.Create(details);
    }
}
