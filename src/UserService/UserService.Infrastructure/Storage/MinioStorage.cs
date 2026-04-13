using UserService.Application.Services;

namespace UserService.Infrastructure.Storage;

public class MinioStorage : IFileStorage
{
    public Task<Uri> SaveAsync(Stream content, string fileName, string path, string contentType,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Uri fileUri, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}