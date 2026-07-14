namespace SchaerbeekMunicipality.Domain.Notifications;

public readonly record struct OutboundNotificationId(Guid Value)
{
    public static OutboundNotificationId New()
    {
        return new OutboundNotificationId(Guid.NewGuid());
    }

    public static OutboundNotificationId From(Guid value)
    {
        return new OutboundNotificationId(value);
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}