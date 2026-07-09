using SchaerbeekMunicipality.Domain.Identity;

namespace SchaerbeekMunicipality.Domain.BirthDeclaration;

public sealed class BirthDeclarationParentLink
{
    private BirthDeclarationParentLink()
    {
    }

    public PersonId PersonId { get; private set; }

    public ParentRole Role { get; private set; }

    public static BirthDeclarationParentLink Create(PersonId personId, ParentRole role)
    {
        return new BirthDeclarationParentLink
        {
            PersonId = personId,
            Role = role,
        };
    }
}
