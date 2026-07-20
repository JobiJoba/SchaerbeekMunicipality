using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Domain.DeathDeclaration;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Domain.Notifications;

public sealed class OutboundNotification
{
    private OutboundNotification()
    {
    }

    public OutboundNotificationId Id { get; private set; }

    public RegistrationCaseId? RegistrationCaseId { get; private set; }

    public BirthDeclarationCaseId? BirthDeclarationCaseId { get; private set; }

    public ChangeOfAddressCaseId? ChangeOfAddressCaseId { get; private set; }

    public DeathDeclarationCaseId? DeathDeclarationCaseId { get; private set; }

    public PersonId PersonId { get; private set; }

    public OutboundNotificationRecipient Recipient { get; private set; }

    public string Message { get; private set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; private set; }

    public OutboundNotificationDeliveryStatus DeliveryStatus { get; private set; }

    public int AttemptCount { get; private set; }

    public DateTimeOffset NextAttemptAt { get; private set; }

    public DateTimeOffset? SentAt { get; private set; }

    public string? LastError { get; private set; }

    public static OutboundNotification CreateForRegistration(
        RegistrationCaseId registrationCaseId,
        PersonId personId,
        OutboundNotificationRecipient recipient,
        string message,
        DateTimeOffset createdAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        return CreatePending(
            registrationCaseId,
            null,
            null,
            null,
            personId,
            recipient,
            message,
            createdAt);
    }

    public static OutboundNotification CreateForBirthDeclaration(
        BirthDeclarationCaseId birthDeclarationCaseId,
        PersonId personId,
        OutboundNotificationRecipient recipient,
        string message,
        DateTimeOffset createdAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        return CreatePending(
            null,
            birthDeclarationCaseId,
            null,
            null,
            personId,
            recipient,
            message,
            createdAt);
    }

    public static OutboundNotification CreateForChangeOfAddress(
        ChangeOfAddressCaseId changeOfAddressCaseId,
        PersonId personId,
        OutboundNotificationRecipient recipient,
        string message,
        DateTimeOffset createdAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        return CreatePending(
            null,
            null,
            changeOfAddressCaseId,
            null,
            personId,
            recipient,
            message,
            createdAt);
    }

    public static OutboundNotification CreateForDeathDeclaration(
        DeathDeclarationCaseId deathDeclarationCaseId,
        PersonId personId,
        OutboundNotificationRecipient recipient,
        string message,
        DateTimeOffset createdAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        return CreatePending(
            null,
            null,
            null,
            deathDeclarationCaseId,
            personId,
            recipient,
            message,
            createdAt);
    }

    public static OutboundNotification Create(
        RegistrationCaseId registrationCaseId,
        PersonId personId,
        OutboundNotificationRecipient recipient,
        string message,
        DateTimeOffset createdAt)
    {
        return CreateForRegistration(
            registrationCaseId,
            personId,
            recipient,
            message,
            createdAt);
    }

    public void MarkProcessing()
    {
        if (DeliveryStatus is not OutboundNotificationDeliveryStatus.Pending)
            throw new InvalidOperationException(
                $"Cannot mark notification '{Id}' as processing from status '{DeliveryStatus}'.");

        DeliveryStatus = OutboundNotificationDeliveryStatus.Processing;
    }

    public void MarkSent(DateTimeOffset sentAt)
    {
        DeliveryStatus = OutboundNotificationDeliveryStatus.Sent;
        SentAt = sentAt;
        LastError = null;
    }

    public void RecordDeliveryFailure(string error, DateTimeOffset now, int maxAttempts)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(error);

        AttemptCount++;
        LastError = error.Trim();

        if (AttemptCount >= maxAttempts)
        {
            DeliveryStatus = OutboundNotificationDeliveryStatus.Failed;
            return;
        }

        DeliveryStatus = OutboundNotificationDeliveryStatus.Pending;
        NextAttemptAt = now + CalculateRetryDelay(AttemptCount);
    }

    private static OutboundNotification CreatePending(
        RegistrationCaseId? registrationCaseId,
        BirthDeclarationCaseId? birthDeclarationCaseId,
        ChangeOfAddressCaseId? changeOfAddressCaseId,
        DeathDeclarationCaseId? deathDeclarationCaseId,
        PersonId personId,
        OutboundNotificationRecipient recipient,
        string message,
        DateTimeOffset createdAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        return new OutboundNotification
        {
            Id = OutboundNotificationId.New(),
            RegistrationCaseId = registrationCaseId,
            BirthDeclarationCaseId = birthDeclarationCaseId,
            ChangeOfAddressCaseId = changeOfAddressCaseId,
            DeathDeclarationCaseId = deathDeclarationCaseId,
            PersonId = personId,
            Recipient = recipient,
            Message = message.Trim(),
            CreatedAt = createdAt,
            DeliveryStatus = OutboundNotificationDeliveryStatus.Pending,
            AttemptCount = 0,
            NextAttemptAt = createdAt
        };
    }

    private static TimeSpan CalculateRetryDelay(int attemptCount)
    {
        return TimeSpan.FromSeconds(30 * Math.Pow(2, Math.Max(0, attemptCount - 1)));
    }
}