using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Application.Features.BirthDeclaration.AttachDocument;
using SchaerbeekMunicipality.Application.Features.BirthDeclaration.ClaimBirthDeclarationCase;
using SchaerbeekMunicipality.Application.Features.BirthDeclaration.LinkParent;
using SchaerbeekMunicipality.Application.Features.BirthDeclaration.OpenBirthDeclarationCase;
using SchaerbeekMunicipality.Application.Features.BirthDeclaration.RecordChildDetails;
using SchaerbeekMunicipality.Application.Features.BirthDeclaration.SetDeclarationHousehold;
using SchaerbeekMunicipality.Application.Features.ChangeOfAddress.ClaimChangeOfAddressCase;
using SchaerbeekMunicipality.Application.Features.ChangeOfAddress.DeclareNewAddress;
using SchaerbeekMunicipality.Application.Features.ChangeOfAddress.OpenChangeOfAddressCase;
using SchaerbeekMunicipality.Application.Features.IdentityDocuments.AdvanceDocumentRequestStatus;
using SchaerbeekMunicipality.Application.Features.IdentityDocuments.AttachApplicantPhoto;
using SchaerbeekMunicipality.Application.Features.IdentityDocuments.ClaimDocumentRequestCase;
using SchaerbeekMunicipality.Application.Features.IdentityDocuments.OpenDocumentRequestCase;
using SchaerbeekMunicipality.Application.Features.IdentityDocuments.RecordFeePayment;
using SchaerbeekMunicipality.Application.Features.Registration.ApproveCase;
using SchaerbeekMunicipality.Application.Features.Registration.ClaimRegistrationCase;
using SchaerbeekMunicipality.Application.Features.Registration.DeclareAddress;
using SchaerbeekMunicipality.Application.Features.Registration.OpenRegistrationCase;
using SchaerbeekMunicipality.Application.Features.Registration.RecordBirthInformation;
using SchaerbeekMunicipality.Application.Features.Registration.RecordIdentity;
using SchaerbeekMunicipality.Application.Features.Registration.RecordPoliceResult;
using SchaerbeekMunicipality.Application.Features.Registration.RequestPoliceVerification;
using SchaerbeekMunicipality.Application.Features.Registration.SetResidenceCategory;
using SchaerbeekMunicipality.Domain.Address;
using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.IdentityDocuments;
using SchaerbeekMunicipality.Domain.Immigration;
using SchaerbeekMunicipality.Domain.NationalRegister;
using SchaerbeekMunicipality.Domain.Police;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Infrastructure.Persistence;

namespace SchaerbeekMunicipality.Application.DemoData;

/// <summary>
///     Seeds in-progress and decision-ready cases for each municipal workflow so local demos
///     start with representative data at multiple stages.
/// </summary>
public static class DemoWorkflowCaseSeeder
{
    internal const string MarkerPrefix = "[Demo]";

    public static async Task SeedAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var provider = scope.ServiceProvider;
        var dbContext = provider.GetRequiredService<MunicipalDbContext>();

        if (await IsAlreadySeededAsync(dbContext, cancellationToken)) return;

        var officer = provider.GetRequiredService<ICurrentOfficer>();
        officer.SetRole(OfficerRole.PopulationOfficer);

        await SeedRegistrationCasesAsync(provider, cancellationToken);
        await SeedBirthDeclarationCasesAsync(provider, cancellationToken);
        await SeedChangeOfAddressCasesAsync(provider, cancellationToken);
        await SeedDocumentRequestCasesAsync(provider, cancellationToken);
    }

    private static async Task<bool> IsAlreadySeededAsync(
        MunicipalDbContext dbContext,
        CancellationToken cancellationToken)
    {
        return await dbContext.BirthDeclarationCases
            .AsNoTracking()
            .AnyAsync(
                c => c.ChildGivenNames != null && c.ChildGivenNames.StartsWith(MarkerPrefix),
                cancellationToken);
    }

    private static async Task SeedRegistrationCasesAsync(
        IServiceProvider services,
        CancellationToken cancellationToken)
    {
        var openHandler = services.GetRequiredService<OpenRegistrationCaseHandler>();
        var claimHandler = services.GetRequiredService<ClaimRegistrationCaseHandler>();
        var recordIdentityHandler = services.GetRequiredService<RecordIdentityHandler>();
        var categoryHandler = services.GetRequiredService<SetResidenceCategoryHandler>();
        var declareHandler = services.GetRequiredService<DeclareAddressHandler>();
        var requestPoliceHandler = services.GetRequiredService<RequestPoliceVerificationHandler>();
        var recordPoliceHandler = services.GetRequiredService<RecordPoliceResultHandler>();
        var approveHandler = services.GetRequiredService<ApproveCaseHandler>();

        var officer = services.GetRequiredService<ICurrentOfficer>();

        officer.SetRole(OfficerRole.ReceptionOfficer);
        await openHandler.Handle(
            new OpenRegistrationCaseRequest(VisitReason.FirstRegistration, null),
            cancellationToken);

        officer.SetRole(OfficerRole.PopulationOfficer);

        var intakeCaseId = await OpenAndClaimRegistrationCaseAsync(
            services,
            openHandler,
            claimHandler,
            VisitReason.FirstRegistration,
            cancellationToken);

        await recordIdentityHandler.Handle(
            intakeCaseId,
            new RecordIdentityRequest($"{MarkerPrefix} Sofia", "Nguyen", new DateOnly(2000, 11, 8), "Vietnamese"),
            cancellationToken);

        var policeCaseId = await OpenAndClaimRegistrationCaseAsync(
            services,
            openHandler,
            claimHandler,
            VisitReason.EuCitizenRegistration,
            cancellationToken);

        await recordIdentityHandler.Handle(
            policeCaseId,
            new RecordIdentityRequest($"{MarkerPrefix} Lucas", "Mercier", new DateOnly(1990, 4, 3), "Belgian"),
            cancellationToken);

        await categoryHandler.Handle(
            policeCaseId,
            new SetResidenceCategoryRequest(ResidenceCategory.EuCitizen),
            cancellationToken);

        await SatisfyRegistrationDocumentRequirementsAsync(services, policeCaseId, cancellationToken);

        await declareHandler.Handle(
            policeCaseId,
            new DeclareAddressRequest("Avenue Rogier", "12", null, "1030", "Schaerbeek"),
            cancellationToken);

        await requestPoliceHandler.Handle(policeCaseId, cancellationToken);

        var reviewCaseId = await OpenAndClaimRegistrationCaseAsync(
            services,
            openHandler,
            claimHandler,
            VisitReason.FirstRegistration,
            cancellationToken);

        await recordIdentityHandler.Handle(
            reviewCaseId,
            new RecordIdentityRequest($"{MarkerPrefix} Emma", "Lambert", new DateOnly(1988, 6, 12), "Belgian"),
            cancellationToken);

        await categoryHandler.Handle(
            reviewCaseId,
            new SetResidenceCategoryRequest(ResidenceCategory.EuCitizen),
            cancellationToken);

        await SatisfyRegistrationDocumentRequirementsAsync(services, reviewCaseId, cancellationToken);

        await declareHandler.Handle(
            reviewCaseId,
            new DeclareAddressRequest("Chaussée de Louvain", "10", null, "1030", "Schaerbeek"),
            cancellationToken);

        var policeRequest = await requestPoliceHandler.Handle(reviewCaseId, cancellationToken);

        await RecordPoliceResultAsync(
            services,
            recordPoliceHandler,
            policeRequest.RequestId,
            PoliceVerificationResult.Confirmed,
            cancellationToken);

        var approvedCaseId = await OpenAndClaimRegistrationCaseAsync(
            services,
            openHandler,
            claimHandler,
            VisitReason.FirstRegistration,
            cancellationToken);

        await recordIdentityHandler.Handle(
            approvedCaseId,
            new RecordIdentityRequest($"{MarkerPrefix} Anne", "Confirm", new DateOnly(1991, 2, 18), "Belgian"),
            cancellationToken);

        await categoryHandler.Handle(
            approvedCaseId,
            new SetResidenceCategoryRequest(ResidenceCategory.EuCitizen),
            cancellationToken);

        await SatisfyRegistrationDocumentRequirementsAsync(services, approvedCaseId, cancellationToken);

        await declareHandler.Handle(
            approvedCaseId,
            new DeclareAddressRequest("Rue Josaphat", "8", null, "1030", "Schaerbeek"),
            cancellationToken);

        var approvedPoliceRequest = await requestPoliceHandler.Handle(approvedCaseId, cancellationToken);

        await RecordPoliceResultAsync(
            services,
            recordPoliceHandler,
            approvedPoliceRequest.RequestId,
            PoliceVerificationResult.Confirmed,
            cancellationToken);

        await approveHandler.Handle(
            approvedCaseId,
            new ApproveCaseRequest(RegisterTarget.PopulationRegister),
            cancellationToken);
    }

    private static async Task SeedBirthDeclarationCasesAsync(
        IServiceProvider services,
        CancellationToken cancellationToken)
    {
        var openHandler = services.GetRequiredService<OpenBirthDeclarationCaseHandler>();
        var claimHandler = services.GetRequiredService<ClaimBirthDeclarationCaseHandler>();
        var recordChildHandler = services.GetRequiredService<RecordChildDetailsHandler>();
        var linkParentHandler = services.GetRequiredService<LinkParentHandler>();
        var attachDocumentHandler = services.GetRequiredService<AttachDocumentHandler>();
        var setHouseholdHandler = services.GetRequiredService<SetDeclarationHouseholdHandler>();
        var registerRepo = services.GetRequiredService<INationalRegisterRepository>();

        var intakeOpened = await openHandler.Handle(cancellationToken);
        var intakeCaseId = new BirthDeclarationCaseId(intakeOpened.CaseId);
        await claimHandler.Handle(intakeCaseId, cancellationToken);

        await recordChildHandler.Handle(
            intakeCaseId,
            new RecordChildDetailsRequest(
                $"{MarkerPrefix} Baby",
                "Intake",
                NewbornSex.Female,
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
                null,
                "CHU Saint-Pierre"),
            cancellationToken);

        var reviewOpened = await openHandler.Handle(cancellationToken);
        var reviewCaseId = new BirthDeclarationCaseId(reviewOpened.CaseId);
        await claimHandler.Handle(reviewCaseId, cancellationToken);

        await recordChildHandler.Handle(
            reviewCaseId,
            new RecordChildDetailsRequest(
                $"{MarkerPrefix} Amélie",
                "Dupont",
                NewbornSex.Female,
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
                null,
                "CHU Saint-Pierre"),
            cancellationToken);

        var jeanDupont = await registerRepo.GetByIdAsync(NationalRegisterSeeder.JeanDupontId, cancellationToken)
                         ?? throw new InvalidOperationException("Seed parent Jean Dupont was not found.");

        await linkParentHandler.Handle(
            reviewCaseId,
            new LinkParentRequest(jeanDupont.Id.Value, ParentRole.Father),
            cancellationToken);

        await using var pdf = new MemoryStream("%PDF-1.4 birth"u8.ToArray());
        await attachDocumentHandler.Handle(
            reviewCaseId,
            "medical.pdf",
            pdf,
            cancellationToken);

        await setHouseholdHandler.Handle(
            reviewCaseId,
            new SetDeclarationHouseholdRequest("Chaussée de Louvain", "10", null, "1030", "Schaerbeek"),
            cancellationToken);
    }

    private static async Task SeedChangeOfAddressCasesAsync(
        IServiceProvider services,
        CancellationToken cancellationToken)
    {
        var openHandler = services.GetRequiredService<OpenChangeOfAddressCaseHandler>();
        var claimHandler = services.GetRequiredService<ClaimChangeOfAddressCaseHandler>();
        var declareHandler = services.GetRequiredService<DeclareNewAddressHandler>();
        var requestPoliceHandler =
            services
                .GetRequiredService<
                    Features.ChangeOfAddress.RequestPoliceVerification.RequestPoliceVerificationHandler>();

        var jeanDupontId = await EnsureRegisteredPersonAsync(
            services,
            NationalRegisterSeeder.JeanDupontId,
            BelgianAddress.Create("Rue de la Paix", "1", null, "1030", "Schaerbeek"),
            cancellationToken);

        var openedIntake = await openHandler.Handle(
            new OpenChangeOfAddressCaseRequest(jeanDupontId),
            cancellationToken);
        await claimHandler.Handle(new ChangeOfAddressCaseId(openedIntake.CaseId), cancellationToken);

        var sofiaNguyenId = await EnsureRegisteredPersonAsync(
            services,
            NationalRegisterSeeder.SofiaNguyenId,
            BelgianAddress.Create("Avenue Rogier", "42", null, "1030", "Schaerbeek"),
            cancellationToken);

        var openedPolice = await openHandler.Handle(
            new OpenChangeOfAddressCaseRequest(sofiaNguyenId),
            cancellationToken);
        var policeCaseId = new ChangeOfAddressCaseId(openedPolice.CaseId);
        await claimHandler.Handle(policeCaseId, cancellationToken);

        await declareHandler.Handle(
            policeCaseId,
            new DeclareNewAddressRequest(
                "Boulevard Lambermont",
                "15",
                null,
                "1030",
                "Schaerbeek",
                HousingSituation.Owner,
                new DateOnly(2026, 9, 1)),
            cancellationToken);

        await requestPoliceHandler.Handle(policeCaseId, cancellationToken);
    }

    private static async Task SeedDocumentRequestCasesAsync(
        IServiceProvider services,
        CancellationToken cancellationToken)
    {
        var openHandler = services.GetRequiredService<OpenDocumentRequestCaseHandler>();
        var claimHandler = services.GetRequiredService<ClaimDocumentRequestCaseHandler>();
        var attachPhotoHandler = services.GetRequiredService<AttachApplicantPhotoHandler>();
        var recordFeeHandler = services.GetRequiredService<RecordFeePaymentHandler>();
        var advanceHandler = services.GetRequiredService<AdvanceDocumentRequestStatusHandler>();

        var jeanDupontId = await EnsureRegisteredPersonAsync(
            services,
            NationalRegisterSeeder.JeanDupontId,
            BelgianAddress.Create("Rue de la Paix", "1", null, "1030", "Schaerbeek"),
            cancellationToken);

        var submittedOpened = await openHandler.Handle(
            new OpenDocumentRequestCaseRequest(jeanDupontId, DocumentRequestType.IdentityCardRenewal),
            cancellationToken);
        await claimHandler.Handle(DocumentRequestCaseId.From(submittedOpened.CaseId), cancellationToken);

        var sofiaNguyenId = await EnsureRegisteredPersonAsync(
            services,
            NationalRegisterSeeder.SofiaNguyenId,
            BelgianAddress.Create("Avenue Rogier", "42", null, "1030", "Schaerbeek"),
            cancellationToken);

        var productionOpened = await openHandler.Handle(
            new OpenDocumentRequestCaseRequest(sofiaNguyenId, DocumentRequestType.PassportRenewal),
            cancellationToken);
        var productionCaseId = DocumentRequestCaseId.From(productionOpened.CaseId);
        await claimHandler.Handle(productionCaseId, cancellationToken);

        await AttachPhotoAndFeeAsync(
            services,
            attachPhotoHandler,
            recordFeeHandler,
            productionCaseId,
            cancellationToken);

        await advanceHandler.Handle(productionCaseId, cancellationToken);

        var readyOpened = await openHandler.Handle(
            new OpenDocumentRequestCaseRequest(jeanDupontId, DocumentRequestType.Passport),
            cancellationToken);
        var readyCaseId = DocumentRequestCaseId.From(readyOpened.CaseId);
        await claimHandler.Handle(readyCaseId, cancellationToken);

        await AttachPhotoAndFeeAsync(
            services,
            attachPhotoHandler,
            recordFeeHandler,
            readyCaseId,
            cancellationToken);

        await advanceHandler.Handle(readyCaseId, cancellationToken);
        await advanceHandler.Handle(readyCaseId, cancellationToken);
    }

    private static async Task<RegistrationCaseId> OpenAndClaimRegistrationCaseAsync(
        IServiceProvider services,
        OpenRegistrationCaseHandler openHandler,
        ClaimRegistrationCaseHandler claimHandler,
        VisitReason visitReason,
        CancellationToken cancellationToken)
    {
        var opened = await openHandler.Handle(
            new OpenRegistrationCaseRequest(visitReason, null),
            cancellationToken);

        var caseId = new RegistrationCaseId(opened.CaseId);
        await claimHandler.Handle(caseId, cancellationToken);
        return caseId;
    }

    private static async Task SatisfyRegistrationDocumentRequirementsAsync(
        IServiceProvider services,
        RegistrationCaseId caseId,
        CancellationToken cancellationToken)
    {
        var birthHandler = services.GetRequiredService<RecordBirthInformationHandler>();
        var attachHandler = services.GetRequiredService<Features.Registration.AttachDocument.AttachDocumentHandler>();

        await birthHandler.Handle(
            caseId,
            new RecordBirthInformationRequest("Brussels", "Belgium"),
            cancellationToken);

        await using var passport = new MemoryStream([0x25, 0x50, 0x44, 0x46]);
        await attachHandler.Handle(
            caseId,
            DocumentType.Passport,
            "passport.pdf",
            passport,
            cancellationToken);

        await using var birthCertificate = new MemoryStream([0x25, 0x50, 0x44, 0x46]);
        await attachHandler.Handle(
            caseId,
            DocumentType.BirthCertificate,
            "birth-certificate.pdf",
            birthCertificate,
            cancellationToken);
    }

    private static async Task RecordPoliceResultAsync(
        IServiceProvider services,
        RecordPoliceResultHandler recordPoliceHandler,
        Guid requestId,
        PoliceVerificationResult result,
        CancellationToken cancellationToken)
    {
        var officer = services.GetRequiredService<ICurrentOfficer>();
        officer.SetRole(OfficerRole.PoliceClerk);

        await recordPoliceHandler.Handle(
            PoliceVerificationRequestId.From(requestId),
            new RecordPoliceResultRequest(result, "Demo seed"),
            cancellationToken);

        officer.SetRole(OfficerRole.PopulationOfficer);
    }

    private static async Task AttachPhotoAndFeeAsync(
        IServiceProvider services,
        AttachApplicantPhotoHandler attachPhotoHandler,
        RecordFeePaymentHandler recordFeeHandler,
        DocumentRequestCaseId caseId,
        CancellationToken cancellationToken)
    {
        await using var photo = new MemoryStream([0xFF, 0xD8, 0xFF, 0xE0]);
        await attachPhotoHandler.Handle(caseId, "photo.jpg", photo, cancellationToken);

        await recordFeeHandler.Handle(
            caseId,
            new RecordFeePaymentRequest("DEMO-FEE"),
            cancellationToken);
    }

    private static async Task<Guid> EnsureRegisteredPersonAsync(
        IServiceProvider services,
        NationalRegisterPersonId registerPersonId,
        BelgianAddress domicile,
        CancellationToken cancellationToken)
    {
        var registerRepo = services.GetRequiredService<INationalRegisterRepository>();
        var registerPerson = await registerRepo.GetByIdAsync(registerPersonId, cancellationToken)
                             ?? throw new InvalidOperationException(
                                 $"Seed NR person '{registerPersonId}' was not found.");

        var personRepo = services.GetRequiredService<IPersonRepository>();
        var existing = await personRepo.GetByRegisterRecordIdAsync(registerPersonId, cancellationToken);
        if (existing is not null) return existing.Id.Value;

        var person = Person.CreateFromRegisterRecord(registerPerson);
        if (person.NationalRegisterNumber is null && registerPerson.NationalRegisterNumber is { } nr)
            person.AssignNationalRegisterNumber(nr);

        person.UpdateDomicile(domicile);
        await personRepo.AddAsync(person, cancellationToken);
        await services.GetRequiredService<MunicipalDbContext>().SaveChangesAsync(cancellationToken);
        return person.Id.Value;
    }
}