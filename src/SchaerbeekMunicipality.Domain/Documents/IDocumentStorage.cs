namespace SchaerbeekMunicipality.Domain.Documents;

public interface IDocumentStorage
{
    Task<StoredDocument> SaveAsync(
        Stream content,
        string fileName,
        CancellationToken cancellationToken);
}

public sealed record StoredDocument(string StoragePath, string ContentHash);
