using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Application.Features.PersonFile.GetPersonFile;
using SchaerbeekMunicipality.Application.Features.RegisterAmendment.ApplyRegisterAmendment;
using SchaerbeekMunicipality.Application.Features.RegisterAmendment.ApproveRegisterAmendment;
using SchaerbeekMunicipality.Application.Features.RegisterAmendment.AttachDocument;
using SchaerbeekMunicipality.Application.Features.RegisterAmendment.ClaimRegisterAmendmentCase;
using SchaerbeekMunicipality.Application.Features.RegisterAmendment.OpenRegisterAmendmentCase;
using SchaerbeekMunicipality.Application.Features.RegisterAmendment.RecordProposedAmendment;
using SchaerbeekMunicipality.Application.Features.RegisterAmendment.SubmitRegisterAmendmentForReview;
using SchaerbeekMunicipality.Domain.Address;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.NationalRegister;
using SchaerbeekMunicipality.Domain.RegisterAmendment;
using SchaerbeekMunicipality.Infrastructure.Persistence;
using SchaerbeekMunicipality.Integration.Tests.Features.Registration;

namespace SchaerbeekMunicipality.Integration.Tests.Features.RegisterAmendment;

public sealed class RegisterAmendmentTests
{
    [Fact]
    public async Task HappyPath_IdentityCorrection_UpdatesPersonAndHistory()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var services = scope.ServiceProvider;
        RegistrationTestHelpers.SetRole(services, OfficerRole.PopulationOfficer);

        var personId = await CreateRegisteredPersonAsync(services);
        var openHandler = services.GetRequiredService<OpenRegisterAmendmentCaseHandler>();
        var claimHandler = services.GetRequiredService<ClaimRegisterAmendmentCaseHandler>();
        var recordHandler = services.GetRequiredService<RecordProposedAmendmentHandler>();
        var attachHandler = services.GetRequiredService<AttachDocumentHandler>();
        var submitHandler = services.GetRequiredService<SubmitRegisterAmendmentForReviewHandler>();
        var approveHandler = services.GetRequiredService<ApproveRegisterAmendmentHandler>();
        var applyHandler = services.GetRequiredService<ApplyRegisterAmendmentHandler>();
        var getPersonHandler = services.GetRequiredService<GetPersonFileHandler>();

        var opened = await openHandler.Handle(
            new OpenRegisterAmendmentCaseRequest(personId, AmendmentType.IdentityCorrection.ToString()),
            CancellationToken.None);
        var caseId = RegisterAmendmentCaseId.From(opened.CaseId);
        await claimHandler.Handle(caseId, CancellationToken.None);

        await recordHandler.Handle(
            caseId,
            new RecordProposedAmendmentRequest("Legal name change", "Jean", "Martin", null, null, null, null, null, null, MarriageRecognitionStatus.NotApplicable),
            CancellationToken.None);

        await using var docStream = new MemoryStream("fake pdf"u8.ToArray());
        await attachHandler.Handle(caseId, DocumentType.Other, "judgment.pdf", docStream, CancellationToken.None);
        await submitHandler.Handle(caseId, CancellationToken.None);
        await approveHandler.Handle(caseId, new ApproveRegisterAmendmentRequest("Verified"), CancellationToken.None);
        await applyHandler.Handle(caseId, CancellationToken.None);

        var personFile = await getPersonHandler.Handle(new PersonId(personId), CancellationToken.None);
        personFile.Header.FamilyName.Should().Be("Martin");
        personFile.History.Should().Contain(h => h.Title == "Amendment applied");
    }

    [Fact]
    public async Task Apply_WithoutApproval_Throws()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var services = scope.ServiceProvider;
        RegistrationTestHelpers.SetRole(services, OfficerRole.PopulationOfficer);

        var personId = await CreateRegisteredPersonAsync(services);
        var openHandler = services.GetRequiredService<OpenRegisterAmendmentCaseHandler>();
        var applyHandler = services.GetRequiredService<ApplyRegisterAmendmentHandler>();

        var opened = await openHandler.Handle(
            new OpenRegisterAmendmentCaseRequest(personId, AmendmentType.IdentityCorrection.ToString()),
            CancellationToken.None);

        var act = () => applyHandler.Handle(RegisterAmendmentCaseId.From(opened.CaseId), CancellationToken.None);
        await act.Should().ThrowAsync<InvalidRegisterAmendmentTransitionException>();
    }

    [Fact]
    public async Task OpenSecondCase_WhileFirstOpen_Throws()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var services = scope.ServiceProvider;
        RegistrationTestHelpers.SetRole(services, OfficerRole.PopulationOfficer);

        var personId = await CreateRegisteredPersonAsync(services);
        var openHandler = services.GetRequiredService<OpenRegisterAmendmentCaseHandler>();

        await openHandler.Handle(
            new OpenRegisterAmendmentCaseRequest(personId, AmendmentType.IdentityCorrection.ToString()),
            CancellationToken.None);

        var act = () => openHandler.Handle(
            new OpenRegisterAmendmentCaseRequest(personId, AmendmentType.NationalityChange.ToString()),
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidRegisterAmendmentTransitionException>();
    }

    [Fact]
    public async Task Approve_AsReceptionOfficer_ThrowsUnauthorized()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var services = scope.ServiceProvider;
        RegistrationTestHelpers.SetRole(services, OfficerRole.PopulationOfficer);

        var personId = await CreateRegisteredPersonAsync(services);
        var caseId = await CreateSubmittedCaseAsync(services, personId);

        RegistrationTestHelpers.SetRole(services, OfficerRole.ReceptionOfficer);
        var approveHandler = services.GetRequiredService<ApproveRegisterAmendmentHandler>();

        var act = () => approveHandler.Handle(
            caseId,
            new ApproveRegisterAmendmentRequest(null),
            CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    private static async Task<RegisterAmendmentCaseId> CreateSubmittedCaseAsync(
        IServiceProvider services,
        Guid personId)
    {
        var openHandler = services.GetRequiredService<OpenRegisterAmendmentCaseHandler>();
        var recordHandler = services.GetRequiredService<RecordProposedAmendmentHandler>();
        var attachHandler = services.GetRequiredService<AttachDocumentHandler>();
        var submitHandler = services.GetRequiredService<SubmitRegisterAmendmentForReviewHandler>();
        var claimHandler = services.GetRequiredService<ClaimRegisterAmendmentCaseHandler>();

        var opened = await openHandler.Handle(
            new OpenRegisterAmendmentCaseRequest(personId, AmendmentType.IdentityCorrection.ToString()),
            CancellationToken.None);
        var caseId = RegisterAmendmentCaseId.From(opened.CaseId);
        await claimHandler.Handle(caseId, CancellationToken.None);

        await recordHandler.Handle(
            caseId,
            new RecordProposedAmendmentRequest(null, "Jean", "Martin", null, null, null, null, null, null, MarriageRecognitionStatus.NotApplicable),
            CancellationToken.None);

        await using var docStream = new MemoryStream("fake pdf"u8.ToArray());
        await attachHandler.Handle(caseId, DocumentType.Other, "judgment.pdf", docStream, CancellationToken.None);
        await submitHandler.Handle(caseId, CancellationToken.None);
        return caseId;
    }

    private static async Task<Guid> CreateRegisteredPersonAsync(IServiceProvider services)
    {
        var registerRepo = services.GetRequiredService<INationalRegisterRepository>();
        var jean = await registerRepo.GetByIdAsync(NationalRegisterSeeder.JeanDupontId, CancellationToken.None);
        jean.Should().NotBeNull();

        var personRepo = services.GetRequiredService<IPersonRepository>();
        var existing = await personRepo.GetByRegisterRecordIdAsync(jean!.Id, CancellationToken.None);
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
