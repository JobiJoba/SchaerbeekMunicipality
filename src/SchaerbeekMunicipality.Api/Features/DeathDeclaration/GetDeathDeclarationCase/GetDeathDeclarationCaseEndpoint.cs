using SchaerbeekMunicipality.Application.Features.DeathDeclaration.GetDeathDeclarationCase;
using SchaerbeekMunicipality.Domain.DeathDeclaration;

namespace SchaerbeekMunicipality.Api.Features.DeathDeclaration.GetDeathDeclarationCase;

public static class GetDeathDeclarationCaseEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        GetDeathDeclarationCaseHandler handler,
        CancellationToken cancellationToken)
    {
        var caseId = new DeathDeclarationCaseId(id);
        var detail = await handler.Handle(caseId, cancellationToken);
        return detail is null ? Results.NotFound() : Results.Ok(detail);
    }
}
