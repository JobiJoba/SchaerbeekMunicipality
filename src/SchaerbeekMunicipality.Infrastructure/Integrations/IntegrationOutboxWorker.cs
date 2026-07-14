using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SchaerbeekMunicipality.Infrastructure.Integrations;

internal sealed class IntegrationOutboxWorker(
    IServiceScopeFactory scopeFactory,
    IOptions<IntegrationOutboxOptions> options,
    ILogger<IntegrationOutboxWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var settings = options.Value;

        if (!settings.Enabled)
        {
            logger.LogInformation("Integration outbox worker is disabled.");
            return;
        }

        logger.LogInformation(
            "Integration outbox worker started (poll every {PollIntervalSeconds}s, batch size {BatchSize}).",
            settings.PollIntervalSeconds,
            settings.BatchSize);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var processor = scope.ServiceProvider.GetRequiredService<IIntegrationOutboxProcessor>();
                await processor.ProcessPendingAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Integration outbox worker iteration failed.");
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(settings.PollIntervalSeconds), stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }
    }
}