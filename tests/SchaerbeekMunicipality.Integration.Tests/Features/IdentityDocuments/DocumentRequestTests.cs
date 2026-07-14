using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Application.Features.IdentityDocuments.AdvanceDocumentRequestStatus;
using SchaerbeekMunicipality.Application.Features.IdentityDocuments.AttachApplicantPhoto;
using SchaerbeekMunicipality.Application.Features.IdentityDocuments.ClaimDocumentRequestCase;
using SchaerbeekMunicipality.Application.Features.IdentityDocuments.IssueDocument;
using SchaerbeekMunicipality.Application.Features.IdentityDocuments.OpenDocumentRequestCase;
using SchaerbeekMunicipality.Application.Features.IdentityDocuments.RecordFeePayment;
using SchaerbeekMunicipality.Application.Features.IdentityDocuments.RemoveDocument;
using SchaerbeekMunicipality.Domain.Address;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.IdentityDocuments;
using SchaerbeekMunicipality.Domain.NationalRegister;
using SchaerbeekMunicipality.Infrastructure.Persistence;
using SchaerbeekMunicipality.Integration.Tests.Features.Registration;

namespace SchaerbeekMunicipality.Integration.Tests.Features.IdentityDocuments;

public sealed class DocumentRequestTests
{
    [Fact]
    public async Task Open_WithoutNationalRegisterNumber_Throws()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        RegistrationTestHelpers.SetRole(scope.ServiceProvider, OfficerRole.PopulationOfficer);

        var db = scope.ServiceProvider.GetRequiredService<MunicipalDbContext>();
        var person = Person.Create(new IdentityDetails("Test", "Person", new DateOnly(1990, 1, 1), "Belgian"));
        await db.Persons.AddAsync(person);
        await db.SaveChangesAsync();

        var handler = scope.ServiceProvider.GetRequiredService<OpenDocumentRequestCaseHandler>();
        var act = () => handler.Handle(
            new OpenDocumentRequestCaseRequest(person.Id.Value, DocumentRequestType.PassportRenewal),
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidDocumentRequestTransitionException>();
    }

    [Fact]
    public async Task Advance_WithoutPhotoOrFee_Throws()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var services = scope.ServiceProvider;
        RegistrationTestHelpers.SetRole(services, OfficerRole.PopulationOfficer);

        var caseId = await OpenClaimedCaseAsync(services);
        var advanceHandler = services.GetRequiredService<AdvanceDocumentRequestStatusHandler>();

        var act = () => advanceHandler.Handle(caseId, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidDocumentRequestTransitionException>();
    }

    [Fact]
    public async Task Issue_WithoutPhoto_Throws()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var services = scope.ServiceProvider;
        RegistrationTestHelpers.SetRole(services, OfficerRole.PopulationOfficer);

        var caseId = await OpenAndAdvanceToReadyAsync(services);
        var db = services.GetRequiredService<MunicipalDbContext>();
        var documentRequestCase = await db.DocumentRequestCases.FindAsync(caseId);
        documentRequestCase!.PhotoDocumentId.Should().NotBeNull();

        await services.GetRequiredService<RemoveDocumentHandler>().Handle(
            caseId,
            documentRequestCase.PhotoDocumentId!.Value,
            CancellationToken.None);

        var issueHandler = services.GetRequiredService<IssueDocumentHandler>();
        var act = () => issueHandler.Handle(caseId, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidDocumentRequestTransitionException>();
    }

    [Fact]
    public async Task HappyPath_OpenPhotoFeeAdvanceIssue_IssuesStubDocumentNumber()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var services = scope.ServiceProvider;
        RegistrationTestHelpers.SetRole(services, OfficerRole.PopulationOfficer);

        var caseId = await OpenClaimedCaseAsync(services);

        await using var photo = new MemoryStream([0xFF, 0xD8, 0xFF, 0xE0]);
        await services.GetRequiredService<AttachApplicantPhotoHandler>().Handle(
            caseId,
            "photo.jpg",
            photo,
            CancellationToken.None);

        await services.GetRequiredService<RecordFeePaymentHandler>().Handle(
            caseId,
            new RecordFeePaymentRequest("STUB-FEE"),
            CancellationToken.None);

        var advanceHandler = services.GetRequiredService<AdvanceDocumentRequestStatusHandler>();
        await advanceHandler.Handle(caseId, CancellationToken.None);
        await advanceHandler.Handle(caseId, CancellationToken.None);

        var issued = await services.GetRequiredService<IssueDocumentHandler>().Handle(caseId, CancellationToken.None);

        issued.Status.Should().Be(DocumentRequestCaseStatus.Issued);
        issued.IssuedDocumentNumber.Should().MatchRegex(@"^BE-\d{4}-\d{4}$");
    }

    [Fact]
    public async Task Open_MinorWithoutHouseholdParent_Throws()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var services = scope.ServiceProvider;
        RegistrationTestHelpers.SetRole(services, OfficerRole.PopulationOfficer);

        var db = services.GetRequiredService<MunicipalDbContext>();
        var person = Person.Create(new IdentityDetails("Minor", "Child", new DateOnly(2015, 1, 1), "Belgian"));
        person.AssignNationalRegisterNumber(NationalRegisterNumber.GenerateStub(new DateOnly(2015, 1, 1), 12));
        await db.Persons.AddAsync(person);
        await db.SaveChangesAsync();

        var handler = services.GetRequiredService<OpenDocumentRequestCaseHandler>();
        var act = () => handler.Handle(
            new OpenDocumentRequestCaseRequest(person.Id.Value, DocumentRequestType.IdentityCard),
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidDocumentRequestTransitionException>()
            .WithMessage("*linked parent*");
    }

    private static async Task<DocumentRequestCaseId> OpenClaimedCaseAsync(IServiceProvider services)
    {
        var personId = await CreateRegisteredPersonAsync(services);
        var openHandler = services.GetRequiredService<OpenDocumentRequestCaseHandler>();
        var claimHandler = services.GetRequiredService<ClaimDocumentRequestCaseHandler>();

        var opened = await openHandler.Handle(
            new OpenDocumentRequestCaseRequest(personId, DocumentRequestType.IdentityCardRenewal),
            CancellationToken.None);

        var caseId = DocumentRequestCaseId.From(opened.CaseId);
        await claimHandler.Handle(caseId, CancellationToken.None);
        return caseId;
    }

    private static async Task<DocumentRequestCaseId> OpenAndAdvanceToReadyAsync(IServiceProvider services)
    {
        var caseId = await OpenClaimedCaseAsync(services);

        await using var photo = new MemoryStream([0xFF, 0xD8, 0xFF, 0xE0]);
        await services.GetRequiredService<AttachApplicantPhotoHandler>().Handle(
            caseId,
            "photo.jpg",
            photo,
            CancellationToken.None);

        await services.GetRequiredService<RecordFeePaymentHandler>().Handle(
            caseId,
            new RecordFeePaymentRequest("STUB-FEE"),
            CancellationToken.None);

        var advanceHandler = services.GetRequiredService<AdvanceDocumentRequestStatusHandler>();
        await advanceHandler.Handle(caseId, CancellationToken.None);
        await advanceHandler.Handle(caseId, CancellationToken.None);
        return caseId;
    }

    private static async Task<Guid> CreateRegisteredPersonAsync(IServiceProvider services)
    {
        var registerRepo = services.GetRequiredService<INationalRegisterRepository>();
        var jean = await registerRepo.GetByIdAsync(NationalRegisterSeeder.JeanDupontId, CancellationToken.None);
        jean.Should().NotBeNull();

        var personRepo = services.GetRequiredService<IPersonRepository>();
        var existing = await personRepo.GetByRegisterRecordIdAsync(jean.Id, CancellationToken.None);
        if (existing is not null) return existing.Id.Value;

        var person = Person.CreateFromRegisterRecord(jean);
        if (person.NationalRegisterNumber is null && jean.NationalRegisterNumber is { } nr)
            person.AssignNationalRegisterNumber(nr);

        person.UpdateDomicile(BelgianAddress.Create("Rue de la Paix", "1", null, "1030", "Schaerbeek"));
        await personRepo.AddAsync(person, CancellationToken.None);
        await services.GetRequiredService<MunicipalDbContext>().SaveChangesAsync();
        return person.Id.Value;
    }
}