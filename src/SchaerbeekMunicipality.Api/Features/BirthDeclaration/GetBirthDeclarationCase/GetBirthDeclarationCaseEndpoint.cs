using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.Application.Features.BirthDeclaration.GetBirthDeclarationCase;

namespace SchaerbeekMunicipality.Api.Features.BirthDeclaration.GetBirthDeclarationCase;

public static class GetBirthDeclarationCaseEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        GetBirthDeclarationCaseHandler handler,
        CancellationToken cancellationToken)
    {
        var caseId = new BirthDeclarationCaseId(id);
        var detail = await handler.Handle(caseId, cancellationToken);
        return detail is null ? Results.NotFound() : Results.Ok(detail);
    }
}
