using WingDing_Party.IdentityProvider.Domain.Common.Abstract;

namespace WingDing_Party.IdentityProvider.Application.Common.Errors.Abstract;

public sealed class ErrorType(int id, string name, string? description = null)
    : Enumeration(id, name, description)
{
    public static readonly ErrorType VALIDATION = new(1, nameof(VALIDATION), "Error occured while request validation.");
    public static readonly ErrorType AUTHENTICATION = new(2, nameof(AUTHENTICATION), "Authentication failed.");
}
