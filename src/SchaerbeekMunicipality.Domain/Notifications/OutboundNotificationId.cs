namespace SchaerbeekMunicipality.Domain.Notifications;

public readonly record struct OutboundNotificationId(Guid Value)
{
    public static OutboundNotificationId New() => new(Guid.NewGuid());

    public static OutboundNotificationId From(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
