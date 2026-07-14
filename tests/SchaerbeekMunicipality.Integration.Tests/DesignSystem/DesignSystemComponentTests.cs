using Bunit;
using FluentAssertions;
using MudBlazor;
using MudBlazor.Services;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Web.DesignSystem.Components.Data;
using SchaerbeekMunicipality.Web.DesignSystem.Components.Feedback;
using SchaerbeekMunicipality.Web.DesignSystem.Components.Layout;

namespace SchaerbeekMunicipality.Integration.Tests.DesignSystem;

public sealed class DesignSystemComponentTests
{
    private static BunitContext CreateContext()
    {
        var context = new BunitContext();
        context.Services.AddMudServices();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        return context;
    }

    [Fact]
    public async Task AppPage_renders_main_landmark_and_title()
    {
        await using var context = CreateContext();

        var cut = context.Render<AppPage>(parameters => parameters
            .Add(p => p.Title, "Cases")
            .Add(p => p.ChildContent, "content"));

        cut.Find("main#main").Should().NotBeNull();
        cut.Markup.Should().Contain("content");
    }

    [Fact]
    public async Task AppPageHeader_renders_single_h1()
    {
        await using var context = CreateContext();

        var cut = context.Render<AppPageHeader>(parameters => parameters
            .Add(p => p.Title, "Registration cases")
            .Add(p => p.Subtitle, "Work queue"));

        cut.FindAll("h1").Should().HaveCount(1);
        cut.Find("h1").TextContent.Should().Be("Registration cases");
        cut.Markup.Should().Contain("Work queue");
    }

    [Fact]
    public async Task AppEmptyState_renders_title_and_description()
    {
        await using var context = CreateContext();

        var cut = context.Render<AppEmptyState>(parameters => parameters
            .Add(p => p.Title, "No cases")
            .Add(p => p.Description, "Open a case to begin."));

        cut.Markup.Should().Contain("No cases");
        cut.Markup.Should().Contain("Open a case to begin.");
    }

    [Fact]
    public async Task AppStatusChip_maps_registration_status_with_text()
    {
        await using var context = CreateContext();

        var cut = context.Render<AppStatusChip>(parameters => parameters
            .Add(p => p.Status, RegistrationCaseStatus.Intake));

        cut.Markup.Should().Contain("Intake");
    }

    [Fact]
    public async Task AppAlert_applies_role_alert()
    {
        await using var context = CreateContext();

        var cut = context.Render<AppAlert>(parameters => parameters
            .Add(p => p.Severity, Severity.Error)
            .Add(p => p.Title, "Validation failed")
            .AddChildContent("Check required fields."));

        cut.Find("[role='alert']").Should().NotBeNull();
        cut.Markup.Should().Contain("Validation failed");
    }
}