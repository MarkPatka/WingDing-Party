namespace UserService.Application.Common.Exceptions;

public class AlreadyDoneException : Exception
{
    public AlreadyDoneException(string message) : base(message) { }
}