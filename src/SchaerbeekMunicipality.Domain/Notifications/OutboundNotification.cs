using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.Domain.ChangeOfAddress;
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

    public PersonId PersonId { get; private set; }

    public OutboundNotificationRecipient Recipient { get; private set; }

    public string Message { get; private set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; private set; }

    public static OutboundNotification CreateForRegistration(
        RegistrationCaseId registrationCaseId,
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
            PersonId = personId,
            Recipient = recipient,
            Message = message.Trim(),
            CreatedAt = createdAt,
        };
    }

    public static OutboundNotification CreateForBirthDeclaration(
        BirthDeclarationCaseId birthDeclarationCaseId,
        PersonId personId,
        OutboundNotificationRecipient recipient,
        string message,
        DateTimeOffset createdAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        return new OutboundNotification
        {
            Id = OutboundNotificationId.New(),
            BirthDeclarationCaseId = birthDeclarationCaseId,
            PersonId = personId,
            Recipient = recipient,
            Message = message.Trim(),
            CreatedAt = createdAt,
        };
    }

    public static OutboundNotification CreateForChangeOfAddress(
        ChangeOfAddressCaseId changeOfAddressCaseId,
        PersonId personId,
        OutboundNotificationRecipient recipient,
        string message,
        DateTimeOffset createdAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        return new OutboundNotification
        {
            Id = OutboundNotificationId.New(),
            ChangeOfAddressCaseId = changeOfAddressCaseId,
            PersonId = personId,
            Recipient = recipient,
            Message = message.Trim(),
            CreatedAt = createdAt,
        };
    }

    public static OutboundNotification Create(
        RegistrationCaseId registrationCaseId,
        PersonId personId,
        OutboundNotificationRecipient recipient,
        string message,
        DateTimeOffset createdAt) =>
        CreateForRegistration(
            registrationCaseId,
            personId,
            recipient,
            message,
            createdAt);
}
