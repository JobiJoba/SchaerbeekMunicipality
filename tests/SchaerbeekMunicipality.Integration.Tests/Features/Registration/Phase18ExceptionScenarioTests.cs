using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SchaerbeekMunicipality.Application.Features.Registration.ApproveCase;
using SchaerbeekMunicipality.Application.Features.Registration.AttachDocument;
using SchaerbeekMunicipality.Application.Features.Registration.DeclareAddress;
using SchaerbeekMunicipality.Application.Features.Registration.DeclareReferenceAddress;
using SchaerbeekMunicipality.Application.Features.Registration.GetCaseReviewChecklist;
using SchaerbeekMunicipality.Application.Features.Registration.GetRegistrationCase;
using SchaerbeekMunicipality.Application.Features.Registration.RecordIdentity;
using SchaerbeekMunicipality.Application.Features.Registration.RequestPoliceVerification;
using SchaerbeekMunicipality.Application.Features.Registration.SetResidenceCategory;
using SchaerbeekMunicipality.Application.Features.Registration.WaivePoliceVerification;
using SchaerbeekMunicipality.Domain.Address;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Immigration;
using SchaerbeekMunicipality.Domain.Police;
using SchaerbeekMunicipality.Domain.ReferenceData;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Integration.Tests.Features.Registration;

public sealed class Phase18ExceptionScenarioTests
{
    [Fact]
    public async Task Diplomat_ApprovesOntoSpecialRegister_AfterPoliceWaiver()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var services = scope.ServiceProvider;

        var caseId = await RegistrationTestHelpers.OpenAndClaimCaseAsync(services);

        await services.GetRequiredService<RecordIdentityHandler>().Handle(
            caseId,
            new RecordIdentityRequest("Elena", "Morales", new DateOnly(1979, 3, 8), "Spanish"),
            CancellationToken.None);

        await services.GetRequiredService<SetResidenceCategoryHandler>().Handle(
            caseId,
            new SetResidenceCategoryRequest(ResidenceCategory.Diplomat),
            CancellationToken.None);

        await using (var stream = new MemoryStream([0x25, 0x50, 0x44, 0x46]))
        {
            await services.GetRequiredService<AttachDocumentHandler>().Handle(
                caseId,
                DocumentType.DiplomaticCard,
                "diplomatic-card.pdf",
                stream,
                CancellationToken.None);
        }

        await RegistrationTestHelpers.SatisfyPhase9RequirementsAsync(services, caseId);

        await services.GetRequiredService<DeclareAddressHandler>().Handle(
            caseId,
            new DeclareAddressRequest("Chaussée de Louvain", "10", null, "1030", "Schaerbeek"),
            CancellationToken.None);

        var waive = await services.GetRequiredService<WaivePoliceVerificationHandler>()
            .Handle(caseId, CancellationToken.None);

        waive.AddressConfirmed.Should().BeTrue();

        var detail = await services.GetRequiredService<GetRegistrationCaseHandler>()
            .Handle(caseId, CancellationToken.None);
        detail!.Checklist.LegalResidenceEstablished.Should().BeTrue();
        detail.SuggestedRegisterTarget.Should().Be(nameof(RegisterTarget.SpecialRegister));
        detail.IllegalStayDetected.Should().BeFalse();

        var checklist = await services.GetRequiredService<GetCaseReviewChecklistHandler>()
            .Handle(caseId, CancellationToken.None);
        checklist!.IsReadyForApproval.Should().BeTrue();
        checklist.SuggestedRegisterTarget.Should().Be(nameof(RegisterTarget.SpecialRegister));

        await services.GetRequiredService<ApproveCaseHandler>().Handle(
            caseId,
            new ApproveCaseRequest(RegisterTarget.SpecialRegister),
            CancellationToken.None);

        var approved = await services.GetRequiredService<IRegistrationCaseRepository>()
            .GetByIdAsync(caseId, CancellationToken.None);
        approved!.Status.Should().Be(RegistrationCaseStatus.Approved);
        approved.SelectedRegisterTarget.Should().Be(RegisterTarget.SpecialRegister);
    }

    [Fact]
    public async Task ReferenceAddress_MarksAddressDeclared_AndStillRequiresPolice()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var services = scope.ServiceProvider;

        var caseId = await RegistrationTestHelpers.OpenAndClaimCaseAsync(services);

        await services.GetRequiredService<RecordIdentityHandler>().Handle(
            caseId,
            new RecordIdentityRequest("Marc", "Dupont", new DateOnly(1975, 9, 12), "Belgian"),
            CancellationToken.None);

        await services.GetRequiredService<SetResidenceCategoryHandler>().Handle(
            caseId,
            new SetResidenceCategoryRequest(ResidenceCategory.EuCitizen),
            CancellationToken.None);

        await RegistrationTestHelpers.SatisfyPhase9RequirementsAsync(services, caseId);

        var declared = await services.GetRequiredService<DeclareReferenceAddressHandler>()
            .Handle(caseId, CancellationToken.None);

        declared.AddressDeclared.Should().BeTrue();
        declared.AddressDeclarationType.Should().Be(nameof(AddressDeclarationType.ReferenceAddress));
        declared.FormattedAddress.Should().Contain(SchaerbeekCommune.ReferenceStreet);

        var beforePolice = await services.GetRequiredService<GetRegistrationCaseHandler>()
            .Handle(caseId, CancellationToken.None);
        beforePolice!.Checklist.AddressDeclared.Should().BeTrue();
        beforePolice.Checklist.AddressConfirmed.Should().BeFalse();
        beforePolice.AddressDeclarationType.Should().Be(AddressDeclarationType.ReferenceAddress);
        beforePolice.IsReadyForApproval.Should().BeFalse();

        var policeRequest = await services.GetRequiredService<RequestPoliceVerificationHandler>()
            .Handle(caseId, CancellationToken.None);

        await RegistrationTestHelpers.RecordPoliceResultAsync(
            services,
            policeRequest.RequestId,
            PoliceVerificationResult.Confirmed,
            "Present at reference address");

        var checklist = await services.GetRequiredService<GetCaseReviewChecklistHandler>()
            .Handle(caseId, CancellationToken.None);
        checklist!.IsReadyForApproval.Should().BeTrue();

        await services.GetRequiredService<ApproveCaseHandler>().Handle(
            caseId,
            new ApproveCaseRequest(RegisterTarget.PopulationRegister),
            CancellationToken.None);

        var approved = await services.GetRequiredService<IRegistrationCaseRepository>()
            .GetByIdAsync(caseId, CancellationToken.None);
        approved!.Status.Should().Be(RegistrationCaseStatus.Approved);
    }
}
