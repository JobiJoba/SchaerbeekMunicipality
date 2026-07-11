using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.Application.Features.BirthDeclaration.GetBirthDeclarationChecklist;

namespace SchaerbeekMunicipality.Api.Features.BirthDeclaration.GetBirthDeclarationChecklist;

public static class GetBirthDeclarationChecklistEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        GetBirthDeclarationChecklistHandler handler,
        CancellationToken cancellationToken)
    {
        var checklist = await handler.Handle(new BirthDeclarationCaseId(id), cancellationToken);
        return checklist is null ? Results.NotFound() : Results.Ok(checklist);
    }
}
