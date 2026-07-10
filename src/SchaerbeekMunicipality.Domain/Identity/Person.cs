using SchaerbeekMunicipality.Domain.Address;
using SchaerbeekMunicipality.Domain.NationalRegister;

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

    public NationalRegisterPersonId? LinkedRegisterRecordId { get; private set; }

    public BisNumber? BisNumber { get; private set; }

    public NationalRegisterNumber? NationalRegisterNumber { get; private set; }

    public CivilStatusRecord? CivilStatus { get; private set; }

    public string? BirthPlace { get; private set; }

    public string? BirthCountry { get; private set; }

    public BelgianAddress? DomicileAddress { get; private set; }

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

    public static Person CreateFromRegisterRecord(NationalRegisterPerson registerPerson)
    {
        ArgumentNullException.ThrowIfNull(registerPerson);

        return new Person
        {
            Id = PersonId.New(),
            GivenName = registerPerson.GivenName,
            FamilyName = registerPerson.FamilyName,
            BirthDate = registerPerson.BirthDate,
            Nationality = registerPerson.Nationality,
            LinkedRegisterRecordId = registerPerson.Id,
            BisNumber = registerPerson.BisNumber,
            NationalRegisterNumber = registerPerson.NationalRegisterNumber,
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

    public void AssignNationalRegisterNumber(NationalRegisterNumber nationalRegisterNumber)
    {
        if (NationalRegisterNumber is not null)
        {
            throw new InvalidOperationException("Person already has a National Register number.");
        }

        NationalRegisterNumber = nationalRegisterNumber;
    }

    public void ConvertBisToNationalRegister(NationalRegisterNumber nationalRegisterNumber)
    {
        if (BisNumber is null)
        {
            throw new InvalidOperationException("Person does not have a BIS number to convert.");
        }

        if (NationalRegisterNumber is not null)
        {
            throw new InvalidOperationException("Person already has a National Register number.");
        }

        NationalRegisterNumber = nationalRegisterNumber;
    }

    public void RecordCivilStatus(CivilStatusDetails details)
    {
        CivilStatus = CivilStatusRecord.Create(details);
    }

    public void RecordBirthInformation(BirthInformationDetails details)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(details.BirthPlace);

        BirthPlace = details.BirthPlace.Trim();
        BirthCountry = string.IsNullOrWhiteSpace(details.BirthCountry)
            ? null
            : details.BirthCountry.Trim();
    }

    public void UpdateDomicile(BelgianAddress address)
    {
        ArgumentNullException.ThrowIfNull(address);
        DomicileAddress = address;
    }
}
