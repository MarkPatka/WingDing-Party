namespace WingDing_Party.IdentityProvider.Application.Common.Interfaces.Services;

public interface IDateTimeProvider
{
    public DateTime Now    { get; }    
    public DateTime UtcNow { get; }
}   
