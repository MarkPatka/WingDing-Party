namespace UserService.Domain.Common.Exceptions;

public class AvatarAlreadyExistsException : Exception
{
    public AvatarAlreadyExistsException(string message) : base(message) { }
}