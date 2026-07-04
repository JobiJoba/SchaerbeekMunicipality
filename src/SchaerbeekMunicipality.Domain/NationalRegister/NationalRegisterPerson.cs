namespace SchaerbeekMunicipality.Domain.NationalRegister;

public sealed class NationalRegisterPerson
{
    private NationalRegisterPerson()
    {
    }

    public NationalRegisterPersonId Id { get; private set; }

    public string GivenName { get; private set; } = string.Empty;

    public string FamilyName { get; private set; } = string.Empty;

    public DateOnly BirthDate { get; private set; }

    public string Nationality { get; private set; } = string.Empty;

    public BisNumber? BisNumber { get; private set; }

    public NationalRegisterNumber? NationalRegisterNumber { get; private set; }

    public static NationalRegisterPerson Create(
        NationalRegisterPersonId id,
        string givenName,
        string familyName,
        DateOnly birthDate,
        string nationality,
        BisNumber? bisNumber,
        NationalRegisterNumber? nationalRegisterNumber)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(givenName);
        ArgumentException.ThrowIfNullOrWhiteSpace(familyName);
        ArgumentException.ThrowIfNullOrWhiteSpace(nationality);

        if (bisNumber is null && nationalRegisterNumber is null)
        {
            throw new ArgumentException("A register record must have a BIS or National Register number.");
        }

        return new NationalRegisterPerson
        {
            Id = id,
            GivenName = givenName.Trim(),
            FamilyName = familyName.Trim(),
            BirthDate = birthDate,
            Nationality = nationality.Trim(),
            BisNumber = bisNumber,
            NationalRegisterNumber = nationalRegisterNumber,
        };
    }

    public void AssignNationalRegisterNumber(NationalRegisterNumber number)
    {
        if (NationalRegisterNumber is not null)
        {
            throw new InvalidOperationException("This register record already has a National Register number.");
        }

        NationalRegisterNumber = number;
    }
}
