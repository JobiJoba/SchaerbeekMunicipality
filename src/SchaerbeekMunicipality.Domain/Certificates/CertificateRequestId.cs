namespace SchaerbeekMunicipality.Domain.Certificates;

public readonly record struct CertificateRequestId(Guid Value)
{
    public static CertificateRequestId New() => new(Guid.NewGuid());

    public static CertificateRequestId From(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
