namespace UserService.Application.Common.Exceptions;

public class AvatarMismatchException : Exception
{
    public AvatarMismatchException(string message) : base(message)
    {
    }
}