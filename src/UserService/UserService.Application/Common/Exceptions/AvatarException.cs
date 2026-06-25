namespace UserService.Application.Common.Exceptions;

public class AvatarException : Exception
{
    public AvatarException(string message, Exception? inner) : base(message, inner)
    {
    }
}