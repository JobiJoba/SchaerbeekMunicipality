using SchaerbeekMunicipality.Application.Features.DeathDeclaration.GetDeathDeclarationChecklist;
using SchaerbeekMunicipality.Domain.DeathDeclaration;

namespace SchaerbeekMunicipality.Api.Features.DeathDeclaration.GetDeathDeclarationChecklist;

public static class GetDeathDeclarationChecklistEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        GetDeathDeclarationChecklistHandler handler,
        CancellationToken cancellationToken)
    {
        var checklist = await handler.Handle(new DeathDeclarationCaseId(id), cancellationToken);
        return checklist is null ? Results.NotFound() : Results.Ok(checklist);
    }
}
