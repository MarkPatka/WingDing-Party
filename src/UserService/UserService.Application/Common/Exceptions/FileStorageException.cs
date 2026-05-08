namespace UserService.Application.Common.Exceptions;

public class FileStorageException : Exception
{
    public FileStorageException(string message, Exception? inner = null) : base(message, inner)
    {
    }
}