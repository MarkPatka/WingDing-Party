namespace WingDing_Party.AuthenticationService.Application.Common.Interfaces.Services;

public interface IDateTimeProvider
{
    public DateTime Now    { get; }    
    public DateTime UtcNow { get; }
}   
