using System.Security.Cryptography;
using Microsoft.Extensions.Hosting;
using SchaerbeekMunicipality.Domain.Documents;

namespace SchaerbeekMunicipality.Infrastructure.Storage;

internal sealed class LocalFileDocumentStorage(IHostEnvironment environment) : IDocumentStorage
{
    private readonly string _rootPath = Path.Combine(environment.ContentRootPath, "uploads");

    public async Task<StoredDocument> SaveAsync(
        Stream content,
        string fileName,
        CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(_rootPath);

        var safeFileName = Path.GetFileName(fileName);
        var uniqueName = $"{Guid.NewGuid():N}_{safeFileName}";
        var fullPath = Path.Combine(_rootPath, uniqueName);

        await using (var fileStream = File.Create(fullPath))
        {
            await content.CopyToAsync(fileStream, cancellationToken);
        }

        var hash = await ComputeSha256Async(fullPath, cancellationToken);
        var relativePath = Path.Combine("uploads", uniqueName);

        return new StoredDocument(relativePath, hash);
    }

    public Task DeleteAsync(string storagePath, CancellationToken cancellationToken)
    {
        var fullPath = Path.Combine(environment.ContentRootPath, storagePath);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

    public Task<Stream> OpenReadAsync(string storagePath, CancellationToken cancellationToken)
    {
        var fullPath = Path.Combine(environment.ContentRootPath, storagePath);
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException("The stored document file was not found.", fullPath);
        }

        Stream stream = File.OpenRead(fullPath);
        return Task.FromResult(stream);
    }

    private static async Task<string> ComputeSha256Async(string path, CancellationToken cancellationToken)
    {
        await using var stream = File.OpenRead(path);
        var hashBytes = await SHA256.HashDataAsync(stream, cancellationToken);
        return Convert.ToHexString(hashBytes);
    }
}
