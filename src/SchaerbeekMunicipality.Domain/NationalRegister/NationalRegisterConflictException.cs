namespace SchaerbeekMunicipality.Domain.NationalRegister;

public sealed class NationalRegisterConflictException(string message) : Exception(message);