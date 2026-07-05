using Microsoft.Extensions.Logging;
using SchaerbeekMunicipality.Domain.Events;

namespace SchaerbeekMunicipality.Infrastructure.Events;

public sealed class RegistrationConfirmedLoggingHandler(ILogger<RegistrationConfirmedLoggingHandler> logger)
    : IRegistrationConfirmedHandler
{
    public Task HandleAsync(RegistrationConfirmed domainEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Registration confirmed for case {CaseId}, person {PersonId}, register {RegisterTarget}. " +
            "Stub outbound: notify tax administration and social security.",
            domainEvent.CaseId,
            domainEvent.PersonId,
            domainEvent.RegisterTarget);

        return Task.CompletedTask;
    }
}
