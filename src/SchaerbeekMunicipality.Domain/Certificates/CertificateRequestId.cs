namespace SchaerbeekMunicipality.Domain.Certificates;

public readonly record struct CertificateRequestId(Guid Value)
{
    public static CertificateRequestId New()
    {
        return new CertificateRequestId(Guid.NewGuid());
    }

    public static CertificateRequestId From(Guid value)
    {
        return new CertificateRequestId(value);
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}