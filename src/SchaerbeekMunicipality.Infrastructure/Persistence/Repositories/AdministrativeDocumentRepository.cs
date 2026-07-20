using Microsoft.EntityFrameworkCore;
using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Domain.DeathDeclaration;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.IdentityDocuments;
using SchaerbeekMunicipality.Domain.RegisterAmendment;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Infrastructure.Persistence.Repositories;

internal sealed class AdministrativeDocumentRepository(MunicipalDbContext dbContext)
    : IAdministrativeDocumentRepository
{
    public async Task<IReadOnlyList<AdministrativeDocument>> ListByRegistrationCaseIdAsync(
        RegistrationCaseId registrationCaseId,
        CancellationToken cancellationToken)
    {
        var documents = await dbContext.AdministrativeDocuments
            .AsNoTracking()
            .Where(d => d.RegistrationCaseId == registrationCaseId)
            .ToListAsync(cancellationToken);

        return documents
            .OrderByDescending(d => d.UploadedAt)
            .ToList();
    }

    public async Task<IReadOnlyList<AdministrativeDocument>> ListByBirthDeclarationCaseIdAsync(
        BirthDeclarationCaseId birthDeclarationCaseId,
        CancellationToken cancellationToken)
    {
        var documents = await dbContext.AdministrativeDocuments
            .AsNoTracking()
            .Where(d => d.BirthDeclarationCaseId == birthDeclarationCaseId)
            .ToListAsync(cancellationToken);

        return documents
            .OrderByDescending(d => d.UploadedAt)
            .ToList();
    }

    public async Task<IReadOnlyList<AdministrativeDocument>> ListByChangeOfAddressCaseIdAsync(
        ChangeOfAddressCaseId changeOfAddressCaseId,
        CancellationToken cancellationToken)
    {
        var documents = await dbContext.AdministrativeDocuments
            .AsNoTracking()
            .Where(d => d.ChangeOfAddressCaseId == changeOfAddressCaseId)
            .ToListAsync(cancellationToken);

        return documents
            .OrderByDescending(d => d.UploadedAt)
            .ToList();
    }

    public async Task<IReadOnlyList<AdministrativeDocument>> ListByDeathDeclarationCaseIdAsync(
        DeathDeclarationCaseId deathDeclarationCaseId,
        CancellationToken cancellationToken)
    {
        var documents = await dbContext.AdministrativeDocuments
            .AsNoTracking()
            .Where(d => d.DeathDeclarationCaseId == deathDeclarationCaseId)
            .ToListAsync(cancellationToken);

        return documents
            .OrderByDescending(d => d.UploadedAt)
            .ToList();
    }

    public async Task<IReadOnlyList<AdministrativeDocument>> ListByDocumentRequestCaseIdAsync(
        DocumentRequestCaseId documentRequestCaseId,
        CancellationToken cancellationToken)
    {
        var documents = await dbContext.AdministrativeDocuments
            .AsNoTracking()
            .Where(d => d.DocumentRequestCaseId == documentRequestCaseId)
            .ToListAsync(cancellationToken);

        return documents
            .OrderByDescending(d => d.UploadedAt)
            .ToList();
    }

    public async Task<IReadOnlyList<AdministrativeDocument>> ListByRegisterAmendmentCaseIdAsync(
        RegisterAmendmentCaseId registerAmendmentCaseId,
        CancellationToken cancellationToken)
    {
        var documents = await dbContext.AdministrativeDocuments
            .AsNoTracking()
            .Where(d => d.RegisterAmendmentCaseId == registerAmendmentCaseId)
            .ToListAsync(cancellationToken);

        return documents
            .OrderByDescending(d => d.UploadedAt)
            .ToList();
    }

    public async Task AddAsync(AdministrativeDocument document, CancellationToken cancellationToken)
    {
        await dbContext.AdministrativeDocuments.AddAsync(document, cancellationToken);
    }

    public Task<AdministrativeDocument?> GetByIdAsync(
        AdministrativeDocumentId id,
        CancellationToken cancellationToken)
    {
        return dbContext.AdministrativeDocuments
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public void Remove(AdministrativeDocument document)
    {
        dbContext.AdministrativeDocuments.Remove(document);
    }
}