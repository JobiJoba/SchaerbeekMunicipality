using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SchaerbeekMunicipality.Domain.Notifications;

namespace SchaerbeekMunicipality.Infrastructure.Integrations;

internal static class IntegrationOutboxServiceCollectionExtensions
{
    public static IServiceCollection AddIntegrationOutbox(this IServiceCollection services)
    {
        services.AddOptions<IntegrationOutboxOptions>();

        foreach (var recipient in Enum.GetValues<OutboundNotificationRecipient>())
            services.AddSingleton<IIntegrationAdapter>(sp => new SimulatedIntegrationAdapter(
                recipient,
                sp.GetRequiredService<IOptions<IntegrationOutboxOptions>>(),
                sp.GetRequiredService<ILogger<SimulatedIntegrationAdapter>>()));

        services.AddSingleton<IntegrationAdapterRegistry>();
        services.AddScoped<IIntegrationOutboxProcessor, IntegrationOutboxProcessor>();
        services.AddHostedService<IntegrationOutboxWorker>();

        return services;
    }
}