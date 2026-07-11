namespace SchaerbeekMunicipality.Web.Features.Registration.Components;

public sealed class IdentityFormModel
{
    public string GivenName { get; set; } = string.Empty;
    public string FamilyName { get; set; } = string.Empty;
    public DateOnly? BirthDate { get; set; }
    public string Nationality { get; set; } = string.Empty;
}
