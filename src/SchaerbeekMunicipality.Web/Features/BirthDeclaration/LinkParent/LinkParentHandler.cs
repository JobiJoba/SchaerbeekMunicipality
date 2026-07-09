using FluentValidation;
using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.NationalRegister;

namespace SchaerbeekMunicipality.Web.Features.BirthDeclaration.LinkParent;

public sealed record LinkParentResponse(
    Guid CaseId,
    Guid PersonId,
    ParentRole Role,
    bool AtLeastOneParentLinked);

public sealed class LinkParentHandler(
    BirthDeclarationCaseGuard caseGuard,
    IBirthDeclarationCaseRepository caseRepository,
    IPersonRepository personRepository,
    INationalRegisterRepository nationalRegisterRepository,
    IValidator<LinkParentRequest> validator)
{
    public async Task<LinkParentResponse> Handle(
        BirthDeclarationCaseId caseId,
        LinkParentRequest request,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var birthDeclarationCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(LinkParent),
            cancellationToken);

        var registerPersonId = NationalRegisterPersonId.From(request.RegisterPersonId);
        var registerPerson = await nationalRegisterRepository.GetByIdAsync(registerPersonId, cancellationToken)
            ?? throw new KeyNotFoundException($"National Register record '{registerPersonId}' was not found.");

        var person = await FindOrCreatePersonAsync(registerPerson, cancellationToken);
        birthDeclarationCase.LinkParent(person.Id, request.Role);

        await caseRepository.SaveChangesAsync(cancellationToken);

        return new LinkParentResponse(
            birthDeclarationCase.Id.Value,
            person.Id.Value,
            request.Role,
            birthDeclarationCase.Checklist.AtLeastOneParentLinked);
    }

    private async Task<Person> FindOrCreatePersonAsync(
        NationalRegisterPerson registerPerson,
        CancellationToken cancellationToken)
    {
        var existing = await personRepository.GetByRegisterRecordIdAsync(registerPerson.Id, cancellationToken);
        if (existing is not null)
        {
            return existing;
        }

        var person = Person.CreateFromRegisterRecord(registerPerson);
        await personRepository.AddAsync(person, cancellationToken);
        return person;
    }
}
