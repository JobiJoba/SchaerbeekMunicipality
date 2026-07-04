using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Web.DesignSystem.Components.Data;
using SchaerbeekMunicipality.Web.DesignSystem.Components.Feedback;
using SchaerbeekMunicipality.Web.DesignSystem.Components.Layout;

namespace SchaerbeekMunicipality.Integration.Tests.DesignSystem;

public sealed class DesignSystemComponentTests : TestContext
{
    public DesignSystemComponentTests()
    {
        Services.AddMudServices();
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void AppPage_renders_main_landmark_and_title()
    {
        var cut = RenderComponent<AppPage>(parameters => parameters
            .Add(p => p.Title, "Cases")
            .Add(p => p.ChildContent, "content"));

        cut.Find("main#main").Should().NotBeNull();
        cut.Markup.Should().Contain("content");
    }

    [Fact]
    public void AppPageHeader_renders_single_h1()
    {
        var cut = RenderComponent<AppPageHeader>(parameters => parameters
            .Add(p => p.Title, "Registration cases")
            .Add(p => p.Subtitle, "Work queue"));

        cut.FindAll("h1").Should().HaveCount(1);
        cut.Find("h1").TextContent.Should().Be("Registration cases");
        cut.Markup.Should().Contain("Work queue");
    }

    [Fact]
    public void AppEmptyState_renders_title_and_description()
    {
        var cut = RenderComponent<AppEmptyState>(parameters => parameters
            .Add(p => p.Title, "No cases")
            .Add(p => p.Description, "Open a case to begin."));

        cut.Markup.Should().Contain("No cases");
        cut.Markup.Should().Contain("Open a case to begin.");
    }

    [Fact]
    public void AppStatusChip_maps_registration_status_with_text()
    {
        var cut = RenderComponent<AppStatusChip>(parameters => parameters
            .Add(p => p.Status, RegistrationCaseStatus.Intake));

        cut.Markup.Should().Contain("Intake");
    }

    [Fact]
    public void AppAlert_applies_role_alert()
    {
        var cut = RenderComponent<AppAlert>(parameters => parameters
            .Add(p => p.Severity, Severity.Error)
            .Add(p => p.Title, "Validation failed")
            .AddChildContent("Check required fields."));

        cut.Find("[role='alert']").Should().NotBeNull();
        cut.Markup.Should().Contain("Validation failed");
    }
}
