using SchaerbeekMunicipality.Domain.IdentityDocuments;

namespace SchaerbeekMunicipality.Application.Features.IdentityDocuments;

internal static class DocumentNumberGenerator
{
    public static async Task<string> NextAsync(
        IDocumentRequestCaseRepository repository,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var count = await repository.CountAsync(cancellationToken);
        var year = timeProvider.GetUtcNow().Year;
        return $"BE-{year}-{(count + 1):D4}";
    }
}
