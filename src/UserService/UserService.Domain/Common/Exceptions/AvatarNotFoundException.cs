namespace UserService.Domain.Common.Exceptions;

public class AvatarNotFoundException : Exception
{
    public AvatarNotFoundException(string message) : base(message) { }
}